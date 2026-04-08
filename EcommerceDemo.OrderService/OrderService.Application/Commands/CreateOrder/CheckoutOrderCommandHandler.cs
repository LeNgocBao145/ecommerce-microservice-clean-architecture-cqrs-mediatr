using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.WebAPI.GrpcClients.Interfaces;

namespace OrderService.Application.Commands.CreateOrder
{
    /// <summary>
    /// Handler for processing checkout order commands.
    /// Manages order creation, coupon validation, and event publishing.
    /// </summary>
    public class CheckoutOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IPromotionGrpcClient promotionGrpcClient,
        ILogger<CheckoutOrderCommandHandler> logger)
        : IRequestHandler<CheckoutOrderCommand, OrderDTO>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        private readonly IPromotionGrpcClient _promotionGrpcClient = promotionGrpcClient ?? throw new ArgumentNullException(nameof(promotionGrpcClient));
        private readonly ILogger<CheckoutOrderCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the checkout order command by creating an order from cart items,
        /// validating coupon codes, and publishing order events.
        /// </summary>
        /// <param name="request">The checkout order command containing user ID, notes, and coupon code</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>OrderDTO containing the created order information</returns>
        /// <exception cref="Exception">Thrown when cart is not found or is empty</exception>
        public async Task<OrderDTO> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing checkout for user: {UserId}", request.UserId);

                // Retrieve user's cart
                var cart = await _unitOfWork.CartRepository.GetCartByAsync(c => c.UserId == request.UserId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for user: {UserId}", request.UserId);
                    throw new InvalidOperationException("Cart not found for the user.");
                }

                if (cart.CartItems == null || cart.CartItems.Count == 0)
                {
                    _logger.LogWarning("Cart is empty for user: {UserId}", request.UserId);
                    throw new InvalidOperationException("Cart is empty. Cannot proceed with checkout.");
                }

                // Calculate subtotal from cart items
                decimal subtotal = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
                decimal discountAmount = 0;

                // Validate coupon code and get discount if provided
                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    _logger.LogInformation("Validating coupon code: {CouponCode} for user: {UserId}", request.CouponCode, request.UserId);

                    discountAmount = await ValidateAndGetDiscountAsync(request.CouponCode, (int)subtotal, cancellationToken);
                }

                // Create order
                var totalAmount = subtotal - discountAmount;
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    CouponCode = request.CouponCode,
                    Status = Domain.Enums.OrderStatus.Pending,
                    Subtotal = subtotal,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                // Create order items from cart items
                order.OrderItems = new List<OrderItem>();
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        TotalPrice = cartItem.UnitPrice * cartItem.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    order.OrderItems.Add(orderItem);
                }

                // Save order to database
                await _unitOfWork.OrderRepository.CreateAsync(order);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Order created successfully with ID: {OrderId} for user: {UserId}", order.Id, request.UserId);

                // Publish order event
                await PublishOrderEventAsync(order, cancellationToken);

                // Clear cart after successful order creation
                foreach (var cartItem in cart.CartItems)
                {
                    _unitOfWork.CartRepository.DeleteAsync(cartItem.Id);
                }
                await _unitOfWork.SaveAsync();

                // Map to DTO and return
                return MapToOrderDTO(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing checkout for user: {UserId}", request.UserId);
                throw;
            }
        }

        /// <summary>
        /// Validates coupon code and retrieves discount amount from Promotion Service.
        /// </summary>
        /// <param name="couponCode">The coupon code to validate</param>
        /// <param name="totalAmount">The total purchase amount</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Discount amount if coupon is valid; otherwise 0</returns>
        private async Task<decimal> ValidateAndGetDiscountAsync(string couponCode, int totalAmount, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _promotionGrpcClient.GetDiscount(couponCode, totalAmount);

                if (response.Success && response.DiscountAmount > 0)
                {
                    _logger.LogInformation("Coupon {CouponCode} is valid. Discount amount: {DiscountAmount}",
                        couponCode, response.DiscountAmount);
                    return (decimal)response.DiscountAmount;
                }

                _logger.LogWarning("Coupon {CouponCode} validation failed: {Message}", couponCode, response.Message);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon {CouponCode}", couponCode);
                // Return 0 discount on error to allow checkout to proceed without discount
                return 0;
            }
        }

        /// <summary>
        /// Publishes order creation event to message broker for other services.
        /// </summary>
        /// <param name="order">The created order</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        private async Task PublishOrderEventAsync(Order order, CancellationToken cancellationToken)
        {
            try
            {
                // Event for review eligibility with userId and productIds
                var reviewEligibilityEvent = new
                {
                    order.UserId,
                    ProductIds = order.OrderItems?.Select(oi => oi.ProductId).ToList() ?? []
                };

                // Event for loyalty points with userId and totalAmount
                var loyaltyPointsEvent = new
                {
                    order.UserId,
                    order.TotalAmount
                };

                await _eventBus.PublishCompletedOrderAsync(order.Id.ToString(), "review-egilibities", reviewEligibilityEvent);
                await _eventBus.PublishCompletedOrderAsync(order.Id.ToString(), "update-user-loyalty-points", loyaltyPointsEvent);
                _logger.LogInformation("Order event published for order: {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing order event for order: {OrderId}", order.Id);
                // Don't throw - order is already created, event publishing failure shouldn't rollback the order
            }
        }

        /// <summary>
        /// Maps Order entity to OrderDTO.
        /// </summary>
        /// <param name="order">The order entity to map</param>
        /// <returns>OrderDTO representation of the order</returns>
        private static OrderDTO MapToOrderDTO(Order order)
        {
            return new OrderDTO(
                order.Id,
                order.UserId,
                order.Status,
                order.CouponCode,
                order.Subtotal,
                order.DiscountAmount,
                order.TotalAmount,
                order.Notes,
                order.CreatedAt,
                order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    CreatedAt = oi.CreatedAt
                }).ToList() ?? new List<OrderItemDTO>()
            );
        }
    }
}
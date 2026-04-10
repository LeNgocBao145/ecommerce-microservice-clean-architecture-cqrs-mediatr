using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Commands.DeleteCart;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.WebAPI.GrpcClients.Interfaces;

namespace OrderService.Application.Commands.CreateOrder
{
    /// <summary>
    /// Handler for processing checkout order commands.
    /// Manages order creation, inventory validation, coupon validation, and event publishing.
    /// Implements Clean Architecture: Orchestrates use cases from domain and application layers.
    /// </summary>
    public class CheckoutOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IMapper mapper,
        IPromotionGrpcClient promotionGrpcClient,
        IStockValidationService stockValidationService,
        IMediator mediator,
        ILogger<CheckoutOrderCommandHandler> logger)
        : IRequestHandler<CheckoutOrderCommand, OrderDTO>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IEventBus _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IPromotionGrpcClient _promotionGrpcClient = promotionGrpcClient ?? throw new ArgumentNullException(nameof(promotionGrpcClient));
        private readonly IStockValidationService _stockValidationService = stockValidationService ?? throw new ArgumentNullException(nameof(stockValidationService));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ILogger<CheckoutOrderCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the checkout order command.
        /// Flow: Retrieve Cart → Validate Stock → Validate Coupon → Create Order → Delete Cart → Publish Events
        /// </summary>
        public async Task<OrderDTO> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting checkout process for user: {UserId}", request.UserId);

                // Step 1: Retrieve cart
                var cart = await GetAndValidateCartAsync(request.UserId, cancellationToken);

                // Step 2: Validate inventory
                await ValidateStockAsync(cart, cancellationToken);

                // Step 3: Calculate totals and validate coupon
                var (subtotal, discountAmount) = await CalculateTotalsAsync(request.CouponCode, request.UserId, cart, cancellationToken);

                // Step 4: Create order
                var order = CreateOrderFromCart(request, cart, subtotal, discountAmount);

                // Step 5: Track order (add to DbContext - chưa save)
                await PersistOrderAsync(order, cancellationToken);

                // Step 6: Delete cart (track deletion - chưa save)
                await DeleteCartAsync(request.UserId, cancellationToken);

                // Step 7: Save tất cả thay đổi (Order create + Cart delete)
                await _unitOfWork.SaveAsync(cancellationToken);

                _logger.LogInformation("Order created successfully. OrderId: {OrderId}, UserId: {UserId}", order.Id, request.UserId);

                // Step 8: Publish events (fire-and-forget)
                await PublishOrderEventsAsync(order, cancellationToken);

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout process failed for user: {UserId}. Error: {ErrorMessage}", request.UserId, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves and validates user's cart (Single Responsibility).
        /// </summary>
        private async Task<Cart> GetAndValidateCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetCartByAsync(c => c.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user: {UserId}", userId);
                throw new InvalidOperationException($"Cart not found for user {userId}.");
            }

            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                _logger.LogWarning("Cart is empty for user: {UserId}", userId);
                throw new InvalidOperationException("Cannot proceed with checkout. Cart is empty.");
            }

            return cart;
        }

        /// <summary>
        /// Validates stock availability for all items in the cart (Single Responsibility).
        /// Uses gRPC to check stock levels from Product Service.
        /// </summary>
        private async Task ValidateStockAsync(Cart cart, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating stock for {ItemCount} cart items", cart.CartItems.Count);

            foreach (var cartItem in cart.CartItems)
            {
                await _stockValidationService.ValidateStockAsync(
                    cartItem.ProductId.ToString(),
                    cartItem.Quantity,
                    cancellationToken);
            }

            _logger.LogInformation("All cart items passed stock validation");
        }

        /// <summary>
        /// Calculates order totals and validates coupon (Single Responsibility).
        /// </summary>
        private async Task<(decimal Subtotal, decimal DiscountAmount)> CalculateTotalsAsync(
            string? couponCode,
            Guid userId,
            Cart cart,
            CancellationToken cancellationToken)
        {
            decimal subtotal = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
            decimal discountAmount = 0m; // Explicitly initialize to 0

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                _logger.LogInformation("Validating coupon: {CouponCode} for user: {UserId}", couponCode, userId);
                discountAmount = await ValidateAndGetDiscountAsync(couponCode, subtotal, cancellationToken);
            }
            else
            {
                _logger.LogInformation("No coupon code provided for user: {UserId}, discount amount set to 0", userId);
            }

            return (subtotal, discountAmount);
        }

        /// <summary>
        /// Creates order entity from cart (Domain Entity Factory Pattern).
        /// </summary>
        private Order CreateOrderFromCart(
            CheckoutOrderCommand request,
            Cart cart,
            decimal subtotal,
            decimal discountAmount)
        {
            var totalAmount = subtotal - discountAmount;

            var order = new Order
            {
                UserId = request.UserId,
                CouponCode = string.IsNullOrWhiteSpace(request.CouponCode) ? string.Empty : request.CouponCode,
                Subtotal = subtotal,
                DiscountAmount = discountAmount >= 0 ? discountAmount : 0m, // Ensure non-negative value
                TotalAmount = totalAmount >= 0 ? totalAmount : 0m,
                Notes = request.Notes,
                OrderItems = []
            };

            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalPrice = cartItem.UnitPrice * cartItem.Quantity,
                };
                order.OrderItems.Add(orderItem);
            }

            return order;
        }

        /// <summary>
        /// Persists order to database (Single Responsibility).
        /// </summary>
        private async Task PersistOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _unitOfWork.OrderRepository.CreateAsync(order);
            //order.Status = OrderStatus.
            //await _unitOfWork.SaveAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes cart using mediator pattern (Decoupling from delete logic).
        /// Follows Dependency Inversion - uses mediator instead of direct repository calls.
        /// </summary>
        private async Task DeleteCartAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var deleteCartCommand = new DeleteCartCommand(userId);
                var deleteResult = await _mediator.Send(deleteCartCommand, cancellationToken);

                if (deleteResult.Success)
                {
                    _logger.LogInformation("Cart deleted successfully for user: {UserId}. Items deleted: {Count}",
                        userId, deleteResult.DeletedItemCount);
                }
                else
                {
                    _logger.LogWarning("Failed to delete cart for user: {UserId}. Message: {Message}",
                        userId, deleteResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart for user: {UserId} after order creation", userId);
                // Don't throw - order already created, cart deletion failure shouldn't rollback order
            }
        }

        /// <summary>
        /// Validates coupon and retrieves discount amount (Single Responsibility).
        /// </summary>
        private async Task<decimal> ValidateAndGetDiscountAsync(
            string couponCode,
            decimal totalAmount,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _promotionGrpcClient.GetDiscount(
                    couponCode,
                    totalAmount.ToString("F2")
                );

                if (response.Success && decimal.TryParse(response.DiscountAmount, out var discountAmount) && discountAmount > 0)
                {
                    _logger.LogInformation("Coupon valid: {CouponCode}, Discount: {DiscountAmount}",
                        couponCode, discountAmount);
                    return discountAmount;
                }

                _logger.LogWarning("Coupon validation failed: {CouponCode}, Message: {Message}",
                    couponCode, response.Message);
                return 0m;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon: {CouponCode}", couponCode);
                return 0m;
            }
        }

        /// <summary>
        /// Publishes order events to message broker (Fire-and-forget pattern).
        /// </summary>
        private async Task PublishOrderEventsAsync(Order order, CancellationToken cancellationToken)
        {
            try
            {
                var reviewEligibilityEvent = new
                {
                    order.UserId,
                    ProductIds = order.OrderItems?.Select(oi => oi.ProductId).ToList() ?? []
                };

                var loyaltyPointsEvent = new
                {
                    order.UserId,
                    order.TotalAmount
                };

                await _eventBus.PublishCompletedOrderAsync(
                    order.Id.ToString(),
                    "review-eligibilities",
                    reviewEligibilityEvent);

                await _eventBus.PublishCompletedOrderAsync(
                    order.Id.ToString(),
                    "update-user-loyalty-points",
                    loyaltyPointsEvent);

                _logger.LogInformation("Order events published for order: {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing order events for order: {OrderId}", order.Id);
                // Don't throw - order already created
            }
        }
    }
}
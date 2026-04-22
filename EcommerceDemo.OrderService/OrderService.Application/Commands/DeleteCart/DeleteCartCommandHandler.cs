using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.DeleteCart
{
    /// <summary>
    /// Handler for deleting user cart after order placement.
    /// Implements Single Responsibility Principle - only handles cart deletion.
    /// </summary>
    public class DeleteCartCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCartCommandHandler> logger)
        : IRequestHandler<DeleteCartCommand, DeleteCartResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<DeleteCartCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<DeleteCartResult> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting cart for user: {UserId}", request.UserId);

                var cart = await _unitOfWork.CartRepository.GetCartByAsync(c => c.UserId == request.UserId);

                if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
                {
                    _logger.LogWarning("Cart not found or empty for user: {UserId}", request.UserId);
                    return new DeleteCartResult(
                        Success: true,
                        DeletedItemCount: 0,
                        Message: "Cart was already empty.");
                }

                int itemCount = cart.CartItems.Count;

                foreach (var cartItem in cart.CartItems.ToList())
                {
                    await _unitOfWork.CartRepository.DeleteCartItemAsync(cartItem.Id);
                }

                _logger.LogInformation("Successfully deleted {ItemCount} cart items for user: {UserId}", itemCount, request.UserId);

                return new DeleteCartResult(
                    Success: true,
                    DeletedItemCount: itemCount,
                    Message: $"Successfully deleted {itemCount} items from cart.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart for user: {UserId}", request.UserId);
                return new DeleteCartResult(
                    Success: false,
                    DeletedItemCount: 0,
                    Message: $"Error deleting cart: {ex.Message}");
            }
        }
    }
}

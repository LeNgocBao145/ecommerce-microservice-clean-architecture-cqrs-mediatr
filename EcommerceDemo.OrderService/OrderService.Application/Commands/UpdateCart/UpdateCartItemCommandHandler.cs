using MapsterMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.UpdateCart
{
    public class UpdateCartItemCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<UpdateCartItemCommand, CartItemDTO>
    {
        public async Task<CartItemDTO> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await unitOfWork.CartRepository.GetCartItemByAsync(ci => ci.Id == request.Id);
            if (cartItem == null)
            {
                throw new KeyNotFoundException($"Cart item with ID {request.Id} not found.");
            }
            cartItem.CartId = request.CartId;
            cartItem.ProductId = request.ProductId;
            cartItem.Quantity = request.Quantity;
            cartItem.UnitPrice = request.UnitPrice;
            unitOfWork.CartRepository.UpdateAsync(cartItem);
            await unitOfWork.SaveAsync(cancellationToken);
            return mapper.Map<CartItemDTO>(cartItem);
        }
    }
}

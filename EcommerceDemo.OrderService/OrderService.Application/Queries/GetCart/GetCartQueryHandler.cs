using MapsterMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetCart
{
    public class GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCartQuery, CartDTO>
    {
        public async Task<CartDTO> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            // Logic to handle the query and return the cart
            var cart = await unitOfWork.CartRepository.GetCartByAsync(c => c.UserId == request.UserId);
            if (cart == null)
            {
                // Handle case where cart is not found, e.g., return null or throw an exception
                return null;
            }
            return mapper.Map<CartDTO>(cart);
        }
    }
}

using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateCart
{
    public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, CartDTO>
    {
        public Task<CartDTO> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            // Logic to create a new cart
            var cart = new CartDTO();
            return Task.FromResult(cart);
        }
    }
}

using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateCart
{
    public record CreateCartCommand(Guid UserId) : IRequest<CartDTO>;
}

using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetCart
{
    public record GetCartQuery(Guid UserId) : IRequest<CartDTO>;
}

using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrderById
{
    public record GetOrdersByUserIdQuery(Guid UserId) : IRequest<IEnumerable<OrderDTO>>;
}

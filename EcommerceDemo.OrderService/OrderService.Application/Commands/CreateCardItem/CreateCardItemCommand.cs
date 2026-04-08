using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateCardItem
{
    public record CreateCardItemCommand(
         Guid CartId,
         Guid ProductId,
         int Quantity,
         decimal UnitPrice
        ) : IRequest<CartItemDTO>;
}

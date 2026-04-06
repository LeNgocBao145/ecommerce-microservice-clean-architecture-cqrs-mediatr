using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.UpdateCart
{
    public record UpdateCartItemCommand(
        Guid Id,
        Guid CartId,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice
        ) : IRequest<CartItemDTO>;
}

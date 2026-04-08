using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder
{
    public record CheckoutOrderCommand(Guid UserId,
        string? Notes,
        String? CouponCode
        ) : IRequest<OrderDTO>;
}

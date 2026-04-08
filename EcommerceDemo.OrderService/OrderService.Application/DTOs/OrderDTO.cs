using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public record OrderDTO
    (
        Guid Id,
        Guid UserId,
        OrderStatus Status,
        string? CouponCode,
        decimal Subtotal,
        decimal DiscountAmount,
        decimal TotalAmount,
        string? Notes,
        DateTime CreatedAt,
        ICollection<OrderItemDTO> OrderItems
    );
}

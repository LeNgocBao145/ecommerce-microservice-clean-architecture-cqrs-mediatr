using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public record OrderDTO
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public OrderStatus Status { get; init; }
        public decimal Subtotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Notes { get; init; }
        public DateTime CreatedAt { get; init; }
        public ICollection<OrderItemDTO> OrderItems { get; init; } = [];
    }
}

namespace OrderService.Application.DTOs
{
    public record OrderItemDTO
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}

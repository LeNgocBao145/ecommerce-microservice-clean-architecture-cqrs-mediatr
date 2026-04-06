namespace OrderService.Application.DTOs
{
    public record CartItemDTO
    {
        public Guid Id { get; init; }
        public Guid CartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public DateTime CreatedDate { get; init; }
    }
}

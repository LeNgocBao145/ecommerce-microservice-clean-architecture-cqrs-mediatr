namespace OrderService.Application.DTOs
{
    public record CartDTO
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public ICollection<CartItemDTO> CartItems { get; init; } = [];
    }
}

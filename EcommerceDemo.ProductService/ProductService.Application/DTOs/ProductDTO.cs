namespace ProductService.Application.DTOs
{
    public record ProductDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public int Stock { get; init; }
        public decimal Price { get; init; }
        public List<string> CategoryNames { get; init; } = [];
    }
}

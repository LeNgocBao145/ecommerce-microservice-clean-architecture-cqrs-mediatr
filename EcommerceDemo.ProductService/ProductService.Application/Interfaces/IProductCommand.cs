namespace ProductService.Application.Interfaces
{
    public interface IProductCommand
    {
        string Name { get; }
        string Description { get; }
        int Stock { get; }
        decimal Price { get; }
        List<Guid>? CategoryIds { get; }
    }
}

using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Commands.CreateProduct
{
    public record CreateProductCommand
    (
        string Name,
        string Description,
        int Stock,
        decimal Price,
        List<Guid>? CategoryIds
    ) : IProductCommand, IRequest<ProductDTO>;
}

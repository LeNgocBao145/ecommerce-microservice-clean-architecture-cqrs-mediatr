using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.DeleteProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.Common;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.GetProductById;
using ProductService.Application.Queries.GetProducts;
using ProductService.Application.Queries.GetReviewsById;
using ProductService.Domain.Entities;

namespace ProductService.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProductDTO>>> GetAll(string? filter, string? categoryName, int pageNumber = 1, int pageSize = 10)
        {
            // Implementation for getting all products
            var products = await mediator.Send(new GetProductsQuery(filter ?? "", categoryName, pageNumber, pageSize));
            return Ok(products);
        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDTO>> GetById([FromRoute] Guid id)
        {
            // Implementation for getting a product by id
            var product = await mediator.Send(new GetProductByIdQuery(id));
            return Ok(product);
        }
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductDTO>> Create([FromBody] CreateProductCommand command)
        {
            // Implementation for creating a product
            var createdProduct = await mediator.Send(command);
            return Ok(createdProduct);
        }
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Product>> Update([FromRoute] Guid id, [FromBody] UpdateProductCommand command)
        {
            // Implementation for updating a product
            command = command with { Id = id }; // Ensure the command has the correct product ID
            var updatedProduct = await mediator.Send(command);
            return Ok(updatedProduct);
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<int>> Delete([FromRoute] Guid id)
        {
            // Implementation for deleting a product
            var result = await mediator.Send(new DeleteProductCommand(id));
            return Ok(result);
        }
        [HttpGet("{productId:guid}/reviews")]
        public async Task<ActionResult<PagedResponse<ReviewDTO>>> GetReviewsByProductId([FromRoute] Guid productId)
        {
            // Implementation for getting reviews of a product by product id
            // This will likely involve sending a query to get the reviews for the specified product
            var reviews = await mediator.Send(new GetReviewsByProductIdQuery(productId));
            if (reviews == null)
            {
                return NotFound();
            }
            return Ok(reviews);
        }
    }
}

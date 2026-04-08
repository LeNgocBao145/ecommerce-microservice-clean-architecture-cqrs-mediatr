using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.CreateProduct
{
    /// <summary>
    /// Handler for CreateProductCommand.
    /// Handles the creation of a new product with validation and error handling.
    /// </summary>
    public class CreateProductCommandHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
        : IRequestHandler<CreateProductCommand, ProductDTO>
    {
        /// <summary>
        /// Handles the product creation command.
        /// </summary>
        /// <param name="request">The create product command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created product DTO.</returns>
        /// <exception cref="InvalidOperationException">Thrown when product creation fails.</exception>
        public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map command to domain entity
                var product = mapper.Map<Product>(request);

                // Create product in repository
                var createdProduct = await productRepository.CreateAsync(product);

                // Validate creation result
                if (createdProduct == null)
                {
                    logger.LogError("Failed to create product: {ProductName}", request.Name);
                    throw new InvalidOperationException("Product creation failed.");
                }

                // Map domain entity to DTO
                var productDto = mapper.Map<ProductDTO>(createdProduct);
                logger.LogInformation("Product created successfully: {ProductId}", productDto.Id);

                return productDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
                throw;
            }
        }
    }
}
using Grpc.Core;
using MediatR;
using ProductService.Application.Queries.GetProductById;

namespace ProductService.WebAPI.Grpc
{
    /// <summary>
    /// gRPC service for handling product-related operations.
    /// Implements the Product service contract using CQRS pattern.
    /// </summary>
    public class ProductGrpcService : Product.ProductBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductGrpcService> _logger;

        /// <summary>
        /// Initializes a new instance of the ProductGrpcService class.
        /// </summary>
        /// <param name="mediator">MediatR mediator for handling queries and commands.</param>
        /// <param name="logger">Logger for service diagnostics.</param>
        public ProductGrpcService(IMediator mediator, ILogger<ProductGrpcService> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the stock information for a specific product.
        /// </summary>
        /// <param name="request">The product stock request containing the product ID.</param>
        /// <param name="context">The gRPC server call context.</param>
        /// <returns>A task that represents the asynchronous operation, containing the product stock response.</returns>
        public override async Task<ProductStockResponse> GetProductStock(ProductStockRequest request, ServerCallContext context)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.ProductId))
                {
                    _logger.LogWarning("Invalid product ID received: {ProductId}", request.ProductId);
                    return new ProductStockResponse
                    {
                        Success = false,
                        Stock = 0,
                        Message = "Invalid product ID provided."
                    };
                }

                // Execute CQRS Query through MediatR
                var query = new GetProductByIdQuery(new Guid(request.ProductId.ToString()));
                var productDto = await _mediator.Send(query, context.CancellationToken);

                // Extract stock from product and build response
                return new ProductStockResponse
                {
                    Success = productDto != null,
                    Stock = productDto?.Stock ?? 0,
                    Message = productDto != null ? "Stock retrieved successfully." : "Product not found."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock for product ID: {ProductId}", request.ProductId);
                return new ProductStockResponse
                {
                    Success = false,
                    Stock = 0,
                    Message = "An error occurred while retrieving product stock."
                };
            }
        }
    }
}

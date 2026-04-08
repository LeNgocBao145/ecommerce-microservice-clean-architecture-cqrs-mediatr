using Grpc.Core;
using OrderService.GrpcClients.Interfaces;

namespace OrderService.GrpcClients.Services
{
    /// <summary>
    /// Implementation of Product gRPC client for retrieving product stock information.
    /// </summary>
    public class ProductGrpcClient : IProductGrpcClient
    {
        private readonly Product.ProductClient _client;
        private readonly ILogger<ProductGrpcClient> _logger;

        /// <summary>
        /// Initializes a new instance of the ProductGrpcClient class.
        /// </summary>
        /// <param name="client">The gRPC Product Service client</param>
        /// <param name="logger">Logger for diagnostic purposes</param>
        public ProductGrpcClient(Product.ProductClient client, ILogger<ProductGrpcClient> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves product stock information from Product Service.
        /// </summary>
        /// <param name="productId">The product ID to check stock</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>ProductStockResponse containing stock information</returns>
        /// <exception cref="ArgumentException">Thrown when product ID is invalid</exception>
        public async Task<ProductStockResponse> GetProductStockAsync(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0.", nameof(productId));
            }

            try
            {
                _logger.LogInformation("Calling Product Service to get stock for product ID: {ProductId}", productId);

                var request = new ProductStockRequest
                {
                    ProductId = productId
                };

                var response = await _client.GetProductStockAsync(request, cancellationToken: cancellationToken);

                if (response != null && response.Success)
                {
                    _logger.LogInformation(
                        "Successfully retrieved stock: {Stock} for product ID: {ProductId}",
                        response.Stock,
                        productId);
                }
                else
                {
                    _logger.LogWarning(
                        "Product Service returned unsuccessful response for product ID: {ProductId}. Message: {Message}",
                        productId,
                        response?.Message);
                }

                return response ?? new ProductStockResponse { Success = false, Message = "Empty response from service", Stock = 0 };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error occurred while fetching stock for product ID: {ProductId}. Status: {Status}", productId, ex.Status);

                return new ProductStockResponse
                {
                    Success = false,
                    Stock = 0,
                    Message = $"Service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching stock for product ID: {ProductId}", productId);

                return new ProductStockResponse
                {
                    Success = false,
                    Stock = 0,
                    Message = "An unexpected error occurred while retrieving product stock information"
                };
            }
        }
    }
}
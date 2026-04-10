using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using OrderService.GrpcClients.Interfaces;

namespace OrderService.Application.Services
{
    /// <summary>
    /// Implementation of stock validation service using gRPC client.
    /// </summary>
    public class StockValidationService(
        IProductGrpcClient productGrpcClient,
        ILogger<StockValidationService> logger)
        : IStockValidationService
    {
        private readonly IProductGrpcClient _productGrpcClient = productGrpcClient;
        private readonly ILogger<StockValidationService> _logger = logger;

        public async Task<bool> ValidateStockAsync(string productId, int requestedQuantity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Validating stock for product {ProductId}: requested quantity {RequestedQuantity}",
                    productId, requestedQuantity);

                var stockResponse = await _productGrpcClient.GetProductStock(productId);

                if (!stockResponse.Success)
                {
                    var errorMessage = $"Unable to verify stock for product {productId}. {stockResponse.Message}";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                if (stockResponse.Stock < requestedQuantity)
                {
                    var errorMessage = $"Insufficient stock for product {productId}. Available: {stockResponse.Stock}, Requested: {requestedQuantity}";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                _logger.LogInformation(
                    "Stock validation successful for product {ProductId}",
                    productId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating stock for product {ProductId}", productId);
                throw;
            }
        }
    }
}
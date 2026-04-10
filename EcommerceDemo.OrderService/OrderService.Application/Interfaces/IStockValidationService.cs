namespace OrderService.Application.Interfaces
{
    /// <summary>
    /// Service for validating product stock availability.
    /// </summary>
    public interface IStockValidationService
    {
        /// <summary>
        /// Validates if a product has sufficient stock for the requested quantity.
        /// </summary>
        /// <param name="productId">The product ID to check</param>
        /// <param name="requestedQuantity">The quantity requested</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if stock is available; false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown when stock check fails with a message</exception>
        Task<bool> ValidateStockAsync(string productId, int requestedQuantity, CancellationToken cancellationToken = default);
    }
}
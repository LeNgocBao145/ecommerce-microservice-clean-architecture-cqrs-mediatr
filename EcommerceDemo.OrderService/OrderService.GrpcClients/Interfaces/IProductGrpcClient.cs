namespace OrderService.GrpcClients.Interfaces
{
    /// <summary>
    /// Interface for interacting with Product Service via gRPC.
    /// </summary>
    public interface IProductGrpcClient
    {
        /// <summary>
        /// Retrieves product stock information.
        /// </summary>
        /// <param name="productId">The product ID to check stock</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Product stock information</returns>
        Task<ProductStockResponse> GetProductStock(string productId, CancellationToken cancellationToken = default);
    }
}
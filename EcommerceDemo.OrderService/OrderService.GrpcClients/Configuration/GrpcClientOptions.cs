namespace OrderService.GrpcClients.Configuration
{
    /// <summary>
    /// Configuration options for gRPC clients.
    /// </summary>
    public class GrpcClientOptions
    {
        /// <summary>
        /// Gets or sets the URL for the Promotion Service gRPC endpoint.
        /// </summary>
        public string? PromotionServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL for the Product Service gRPC endpoint.
        /// </summary>
        public string? ProductServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the timeout duration in seconds for gRPC calls.
        /// Default is 30 seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
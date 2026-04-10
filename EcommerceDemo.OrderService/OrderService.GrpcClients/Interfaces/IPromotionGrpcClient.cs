using PromotionService.WebAPI.Protos;

namespace OrderService.WebAPI.GrpcClients.Interfaces
{
    /// <summary>
    /// Interface for interacting with Promotion Service via gRPC.
    /// </summary>
    public interface IPromotionGrpcClient
    {
        /// <summary>
        /// Retrieves discount amount for a given coupon code and total amount.
        /// </summary>
        /// <param name="couponCode">The coupon code to validate and get discount for</param>
        /// <param name="totalAmount">The total purchase amount</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Discount amount if coupon is valid; otherwise 0</returns>
        Task<GetDiscountResponse> GetDiscount(string couponCode, string totalAmount);
    }
}
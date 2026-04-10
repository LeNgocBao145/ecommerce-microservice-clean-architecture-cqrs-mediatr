using Grpc.Core;
using OrderService.WebAPI.GrpcClients.Interfaces;
using PromotionService.WebAPI.Protos;

namespace OrderService.WebAPI.GrpcClients.Services
{
    /// <summary>
    /// Implementation of Promotion gRPC client for retrieving discount information.
    /// </summary>
    public class PromotionGrpcClient : IPromotionGrpcClient
    {
        private readonly CouponService.CouponServiceClient _client;
        private readonly ILogger<PromotionGrpcClient> _logger;

        /// <summary>
        /// Initializes a new instance of the PromotionGrpcClient class.
        /// </summary>
        /// <param name="client">The gRPC Coupon Service client</param>
        /// <param name="logger">Logger for diagnostic purposes</param>
        public PromotionGrpcClient(CouponService.CouponServiceClient client, ILogger<PromotionGrpcClient> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves discount amount from Promotion Service based on coupon code and total amount.
        /// </summary>
        /// <param name="couponCode">The coupon code to validate</param>
        /// <param name="totalAmount">The total purchase amount</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>GetDiscountResponse containing discount information</returns>
        /// <exception cref="ArgumentException">Thrown when coupon code is null or empty</exception>
        public async Task<GetDiscountResponse> GetDiscount(string couponCode, string totalAmount)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                throw new ArgumentException("Coupon code cannot be null or empty.", nameof(couponCode));
            }

            if (decimal.Parse(totalAmount) < 0)
            {
                throw new ArgumentException("Total amount cannot be negative.", nameof(totalAmount));
            }

            try
            {
                _logger.LogInformation("Calling Promotion Service to get discount for coupon code: {CouponCode}", couponCode);

                var request = new GetDiscountRequest
                {
                    CouponCode = couponCode,
                    TotalAmount = totalAmount
                };

                var response = _client.GetDiscount(request);

                if (response != null)
                {
                    _logger.LogInformation(
                        "Successfully retrieved discount: {DiscountAmount} for coupon {CouponCode}",
                        response.DiscountAmount,
                        couponCode);
                }

                return response ?? new GetDiscountResponse { Success = false, Message = "Empty response from service" };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error occurred while fetching discount for coupon {CouponCode}. Status: {Status}", couponCode, ex.Status);

                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = $"Service error: {ex.Status.Detail}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching discount for coupon {CouponCode}", couponCode);

                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = "An unexpected error occurred while retrieving discount information"
                };
            }
        }
    }
}
using Grpc.Core;
using PromotionService.Domain.Entities;
using PromotionService.Domain.Enums;
using PromotionService.Domain.Interfaces;
using PromotionService.WebAPI.Protos;

namespace PromotionService.WebAPI.Grpc
{
    /// <summary>
    /// Handles coupon validation and discount calculation for gRPC requests.
    /// Follows Dependency Injection and Single Responsibility principles.
    /// </summary>
    public class CouponGrpcService(ICouponRepository couponRepository) : CouponService.CouponServiceBase
    {
        private readonly ICouponRepository _couponRepository = couponRepository;

        /// <summary>
        /// Validates coupon and calculates discount amount asynchronously.
        /// </summary>
        /// <param name="request">Coupon code and order total amount</param>
        /// <returns>Discount response with validation result and calculated amount</returns>
        public override async Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            // Validation 1: Check if coupon exists
            var coupon = await _couponRepository.GetCouponByAsync(c => c.Code == request.CouponCode);
            if (coupon == null)
            {
                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = "Coupon code not found."
                };
            }

            // Validation 2: Check expiration date (coupon must be within valid period)
            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = "Coupon has expired or is not yet valid."
                };
            }

            // Validation 3: Check usage limit (must have remaining uses)
            if (coupon.UsageLimit <= 0)
            {
                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = "Coupon usage limit exceeded."
                };
            }

            decimal totalAmount = decimal.Parse(request.TotalAmount);

            // Validation 4: Check minimum order value requirement
            if (totalAmount < coupon.MinOrderValue)
            {
                return new GetDiscountResponse
                {
                    Success = false,
                    DiscountAmount = "0.00",
                    Message = $"Order total must be at least {coupon.MinOrderValue} to use this coupon."
                };
            }

            // Calculate discount amount based on coupon type
            decimal discountAmount = CalculateDiscount(coupon, totalAmount);

            return new GetDiscountResponse
            {
                Success = true,
                DiscountAmount = discountAmount.ToString("F2"),
                Message = "Discount applied successfully."
            };
        }

        /// <summary>
        /// Calculates discount based on coupon type (fixed amount or percentage).
        /// Follows Single Responsibility Principle - handles only discount calculation.
        /// </summary>
        /// <param name="coupon">The coupon entity with discount type and value</param>
        /// <param name="totalAmount">The order total amount to apply discount to</param>
        /// <returns>Calculated discount amount</returns>
        private decimal CalculateDiscount(Coupon coupon, decimal totalAmount)
        {
            return coupon.Type switch
            {
                DiscountType.FIXED_AMOUNT => coupon.Value,
                DiscountType.PERCENTAGE => (totalAmount * coupon.Value) / 100,
                _ => 0
            };
        }
    }
}
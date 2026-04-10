using MapsterMapper;
using PromotionService.Application.DTOs.Requests;
using PromotionService.Application.DTOs.Responses;
using PromotionService.Application.Interfaces;
using PromotionService.Domain.Entities;
using PromotionService.Domain.Enums;
using PromotionService.Domain.Interfaces;

namespace PromotionService.Application.Services
{
    public class CouponService(ICouponRepository couponRepository, IMapper mapper) : ICouponService
    {
        public async Task<CouponResponse> CreateCouponAsync(CreateCouponRequest request)
        {
            // Validate input
            ValidateCreateCouponRequest(request);

            // Check if coupon code already exists
            var codeExists = await couponRepository.CouponCodeExistsAsync(request.Code);
            if (codeExists)
                throw new InvalidOperationException($"Coupon code '{request.Code}' already exists.");

            // Parse discount type
            if (!Enum.TryParse<DiscountType>(request.DiscountType.ToString(), out var discountType))
                throw new ArgumentException($"Invalid discount type '{request.DiscountType}'.");

            var coupon = mapper.Map<Coupon>(request);  // ← This maps before setting Type
            coupon.Type = discountType;  // ← This should override, but may not

            var createdCoupon = await couponRepository.CreateAsync(coupon);
            return mapper.Map<CouponResponse>(createdCoupon);
        }

        public async Task<ApplyCouponResponse> ApplyCouponAsync(ApplyCouponRequest request)
        {
            var coupon = await couponRepository.GetCouponByAsync(c => c.Code == request.Code);

            if (coupon == null)
                return new ApplyCouponResponse
                {
                    IsValid = false,
                    Message = "Coupon not found.",
                    DiscountAmount = 0,
                    FinalOrderValue = request.OrderValue
                };

            // Check if coupon is expired
            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
                return new ApplyCouponResponse
                {
                    IsValid = false,
                    Message = "Coupon has expired or not yet active.",
                    DiscountAmount = 0,
                    FinalOrderValue = request.OrderValue
                };

            // Check usage limit
            if (coupon.UsageCount >= coupon.UsageLimit)
                return new ApplyCouponResponse
                {
                    IsValid = false,
                    Message = "Coupon usage limit exceeded.",
                    DiscountAmount = 0,
                    FinalOrderValue = request.OrderValue
                };

            // Check minimum order value
            if (request.OrderValue < coupon.MinOrderValue)
                return new ApplyCouponResponse
                {
                    IsValid = false,
                    Message = $"Order value must be at least {coupon.MinOrderValue}.",
                    DiscountAmount = 0,
                    FinalOrderValue = request.OrderValue
                };

            // Calculate discount
            var discountAmount = CalculateDiscount(coupon.Type, coupon.Value, request.OrderValue);
            var finalOrderValue = request.OrderValue - discountAmount;

            // Increment usage count
            coupon.UsageCount++;
            await couponRepository.UpdateCouponAsync(coupon);

            return new ApplyCouponResponse
            {
                IsValid = true,
                DiscountAmount = discountAmount,
                FinalOrderValue = Math.Max(0, finalOrderValue),
                Message = "Coupon applied successfully."
            };
        }

        public async Task<CouponResponse?> GetCouponByCodeAsync(string code)
        {
            var coupon = await couponRepository.GetCouponByAsync(c => c.Code == code.ToUpper());
            return coupon != null ? mapper.Map<CouponResponse>(coupon) : null;
        }

        private decimal CalculateDiscount(DiscountType type, decimal value, decimal orderValue)
        {
            return type == DiscountType.PERCENTAGE
                ? (orderValue * value) / 100
                : value;
        }

        private void ValidateCreateCouponRequest(CreateCouponRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("Coupon code is required.");

            if (request.Value <= 0)
                throw new ArgumentException("Discount value must be greater than 0.");

            if (request.MinOrderValue < 0)
                throw new ArgumentException("Minimum order value cannot be negative.");

            if (request.StartDate >= request.EndDate)
                throw new ArgumentException("Start date must be before end date.");

            if (request.UsageLimit <= 0)
                throw new ArgumentException("Usage limit must be greater than 0.");
        }
    }
}
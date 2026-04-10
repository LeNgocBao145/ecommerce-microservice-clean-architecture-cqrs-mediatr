using PromotionService.Application.DTOs.Requests;
using PromotionService.Application.DTOs.Responses;

namespace PromotionService.Application.Interfaces
{
    public interface ICouponService
    {
        Task<CouponResponse> CreateCouponAsync(CreateCouponRequest request);
        Task<ApplyCouponResponse> ApplyCouponAsync(ApplyCouponRequest request);
        Task<CouponResponse?> GetCouponByCodeAsync(string code);
    }
}
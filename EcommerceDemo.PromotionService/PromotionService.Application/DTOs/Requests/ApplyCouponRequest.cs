namespace PromotionService.Application.DTOs.Requests
{
    public class ApplyCouponRequest
    {
        public string Code { get; set; } = null!;
        public decimal OrderValue { get; set; }
    }
}
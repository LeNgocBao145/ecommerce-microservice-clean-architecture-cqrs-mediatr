using PromotionService.Domain.Enums;

namespace PromotionService.Application.DTOs.Responses
{
    public class CouponResponse
    {
        public string Code { get; set; } = null!;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
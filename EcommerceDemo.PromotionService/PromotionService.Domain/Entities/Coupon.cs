using PromotionService.Domain.Enums;

namespace PromotionService.Domain.Entities
{
    public class Coupon
    {
        public string Code { get; set; } = null!;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

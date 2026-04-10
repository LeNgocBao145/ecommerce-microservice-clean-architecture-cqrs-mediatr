namespace PromotionService.Application.DTOs.Requests
{
    public class CreateCouponRequest
    {
        public string Code { get; set; } = null!;
        public string DiscountType { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
    }
}
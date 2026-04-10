namespace PromotionService.Application.DTOs.Responses
{
    public class ApplyCouponResponse
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Message { get; set; }
        public decimal FinalOrderValue { get; set; }
    }
}
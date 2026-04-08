namespace PromotionService.Application.Events
{
    public record OrderCompletedEvent(Guid UserId, decimal TotalAmount);
}

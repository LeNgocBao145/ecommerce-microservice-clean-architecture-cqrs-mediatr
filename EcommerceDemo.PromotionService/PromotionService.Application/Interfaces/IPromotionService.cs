namespace PromotionService.Application.Interfaces
{
    public interface IPromotionService
    {
        Task<bool> UpdateLoyaltyPoints(Guid userId, decimal points);
    }
}

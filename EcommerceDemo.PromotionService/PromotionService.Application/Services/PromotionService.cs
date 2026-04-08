using PromotionService.Application.Interfaces;
using PromotionService.Domain.Interfaces;

namespace PromotionService.Application.Services
{
    public class PromotionService(ICouponRepository couponRepository) : IPromotionService
    {
        private readonly ICouponRepository _couponRepository = couponRepository;

        public async Task<bool> UpdateLoyaltyPoints(Guid userId, decimal totalAmount)
        {
            var userLoyaltyPoints = await _couponRepository.GetLoyaltyByAsync(c => c.UserId == userId);

            if (userLoyaltyPoints == null)
            {
                return false;
            }

            int points = CalculatePoints(totalAmount);
            userLoyaltyPoints.AddPoints(points);

            var updatedLoyalty = await _couponRepository.UpdateLoyaltyAsync(userLoyaltyPoints);

            if (updatedLoyalty == null)
                return false;

            return true;
        }

        private int CalculatePoints(decimal totalAmount)
        {
            if (totalAmount <= 0) return 0;

            return (int)(totalAmount / 100000); // 1 điểm cho mỗi 100.000 VND
        }
    }
}

using PromotionService.Application.Interfaces;
using PromotionService.Domain.Entities;
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
                int points = CalculatePoints(totalAmount);
                userLoyaltyPoints = new Loyalty
                {
                    UserId = userId
                };

                userLoyaltyPoints.AddPoints(points);

                var createdLoyalty = await _couponRepository.CreateLoyaltyAsync(userLoyaltyPoints);
                return createdLoyalty != null;
            }

            int pointsToAdd = CalculatePoints(totalAmount);
            userLoyaltyPoints.AddPoints(pointsToAdd);

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

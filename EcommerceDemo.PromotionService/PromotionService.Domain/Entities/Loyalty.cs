using PromotionService.Domain.Enums;
using PromotionService.Domain.Policies;

namespace PromotionService.Domain.Entities
{
    public class Loyalty
    {
        public Guid UserId { get; set; }
        public int Points { get; set; }
        public UserRank Rank { get; set; }

        public void AddPoints(int points)
        {
            Points += points;
            UpdateRank();
        }

        private void UpdateRank()
        {
            Rank = UserRankPolicy.CalculateRank(Points);
        }
    }
}

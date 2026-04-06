using PromotionService.Domain.Enums;

namespace PromotionService.Application.DTOs
{
    public record CreateDTO(
         Guid Code,
         DiscountType Type,
         decimal Value,
         decimal MinOrderValue,
         DateTime StartDate,
         DateTime EndDate,
         int UsageLimit
        );
}

using Mapster;
using PromotionService.Application.DTOs.Requests;
using PromotionService.Application.DTOs.Responses;
using PromotionService.Domain.Entities;

namespace PromotionService.Application.Mappings
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Coupon, CouponResponse>();

            config.NewConfig<CreateCouponRequest, Coupon>()
                .Map(dest => dest.Code, src => src.Code.ToUpper())
                .Map(dest => dest.UsageCount, src => 0);
        }
    }
}

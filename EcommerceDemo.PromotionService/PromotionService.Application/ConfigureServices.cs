using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using PromotionService.Application.Interfaces;
using PromotionService.Application.Mappings;
using PromotionService.Application.Services;

namespace PromotionService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(MappingProfile).Assembly);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            // Register Application Services
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IPromotionService, PromotionService.Application.Services.PromotionService>();

            return services;
        }
    }
}

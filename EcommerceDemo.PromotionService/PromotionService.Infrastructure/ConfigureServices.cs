using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromotionService.Domain.Interfaces;
using PromotionService.Infrastructure.Repositories;

namespace PromotionService.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register repositories
            services.AddScoped<ICouponRepository, CouponRepository>();

            services.AddSingleton(new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            });

            services.AddSingleton<IConsumer<string, string>>(sp =>
            {
                var config = sp.GetRequiredService<ConsumerConfig>();
                return new ConsumerBuilder<string, string>(config).Build();
            });

            return services;
        }
    }
}

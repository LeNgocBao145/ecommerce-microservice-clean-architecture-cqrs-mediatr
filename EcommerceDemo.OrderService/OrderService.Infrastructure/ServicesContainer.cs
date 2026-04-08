using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging.Kafka.Producers;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure
{
    public static class ServicesContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            });

            services.AddSingleton<IProducer<string, string>>(sp =>
            {
                var config = sp.GetRequiredService<ProducerConfig>();
                return new ProducerBuilder<string, string>(config).Build();
            });

            services.AddScoped<IEventBus, OrderProducer>();

            return services;
        }
    }
}

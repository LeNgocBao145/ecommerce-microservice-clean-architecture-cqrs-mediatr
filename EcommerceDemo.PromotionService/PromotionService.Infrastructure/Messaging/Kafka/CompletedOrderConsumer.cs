using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PromotionService.Application.Events;
using PromotionService.Application.Interfaces;
using System.Text.Json;

namespace PromotionService.Infrastructure.Messaging.Kafka
{
    public class CompletedOrderConsumer(
        IServiceProvider serviceProvider,
        ILogger<CompletedOrderConsumer> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<CompletedOrderConsumer> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var promotionService = scope.ServiceProvider.GetRequiredService<IPromotionService>();
            var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();

            consumer.Subscribe("update-user-loyalty-points");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    var orderCompletedEvent = JsonSerializer.Deserialize<OrderCompletedEvent>(consumeResult.Message.Value);

                    if (orderCompletedEvent != null)
                    {
                        await promotionService.UpdateLoyaltyPoints(orderCompletedEvent.UserId, orderCompletedEvent.TotalAmount);
                        _logger.LogInformation("Loyalty points updated for user {UserId} with order total {TotalAmount}",
                            orderCompletedEvent.UserId, orderCompletedEvent.TotalAmount);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming order event: {ErrorReason}", ex.Error.Reason);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

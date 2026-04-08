using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using PromotionService.Application.Events;
using PromotionService.Application.Interfaces;
using System.Text.Json;

namespace PromotionService.Infrastructure.Messaging.Kafka
{
    public class CompletedOrderConsumer(
        IConsumer<string, string> consumer,
        IPromotionService promotionService,
        ILogger<CompletedOrderConsumer> logger)
    {
        private readonly IConsumer<string, string> _consumer = consumer;
        private readonly IPromotionService _promotionService = promotionService;
        private readonly ILogger<CompletedOrderConsumer> _logger = logger;

        public async Task ConsumeOrderEvents()
        {
            _consumer.Subscribe("update-user-loyalty-points");

            while (true)
            {
                try
                {
                    var consumeResult = _consumer.Consume();
                    var orderCompletedEvent = JsonSerializer.Deserialize<OrderCompletedEvent>(consumeResult.Message.Value);

                    if (orderCompletedEvent != null)
                    {
                        // Calculate loyalty points based on the order total
                        await _promotionService.UpdateLoyaltyPoints(orderCompletedEvent.UserId, orderCompletedEvent.TotalAmount);

                        _logger.LogInformation("Loyalty points updated for user {UserId} with order total {TotalAmount}",
                            orderCompletedEvent.UserId, orderCompletedEvent.TotalAmount);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize order completed event: null result");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming order event: {ErrorReason}", ex.Error.Reason);
                }
            }
        }
    }
}

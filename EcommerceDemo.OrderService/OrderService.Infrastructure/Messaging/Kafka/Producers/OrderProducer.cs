using Confluent.Kafka;
using OrderService.Application.Interfaces;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging.Kafka.Producers
{
    public class OrderProducer(IProducer<string, string> producer) : IEventBus
    {
        private readonly IProducer<string, string> _producer = producer;

        public async Task PublishCompletedOrderAsync<T>(string? id, string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = id ?? String.Empty,
                Value = json
            });
        }
    }
}

using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Application.Commands.CreateReviewEligibility;
using ProductService.Application.Events;
using System.Text.Json;

namespace ProductService.Infrastructure.Messaging.Kafka.Consumers
{
    public class CompletedOrderConsumer(IConsumer<string, string> consumer, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer = consumer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("review-eligibilities");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var orderCompletedEvent = JsonSerializer.Deserialize<OrderCompletedEvent>(consumeResult.Message.Value);

                    if (orderCompletedEvent != null)
                    {
                        // Create a scope for resolving scoped services
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
                            await mediator.Send(new CreateReviewEligibilityCommand(
                                Guid.Parse(orderCompletedEvent.UserId),
                                orderCompletedEvent.ProductIds
                            ));
                        }
                        Console.WriteLine($"Notification sent for order completion to user: {orderCompletedEvent.UserId}");
                        _consumer.Commit(consumeResult);
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize order completed event: null result");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Error.Reason}");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}

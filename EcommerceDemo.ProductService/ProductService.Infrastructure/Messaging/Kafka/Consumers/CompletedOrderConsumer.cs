using Confluent.Kafka;
using MediatR;
using ProductService.Application.Commands.CreateReviewEligibility;
using ProductService.Application.Events;
using System.Text.Json;

namespace ProductService.Infrastructure.Messaging.Kafka.Consumers
{
    public class CompletedOrderConsumer(IConsumer<string, string> consumer, ISender mediator)
    {
        private readonly IConsumer<string, string> _consumer = consumer;

        public async Task ConsumeOrderEvents()
        {
            _consumer.Subscribe("review-egilibities");

            while (true)
            {
                try
                {
                    var consumeResult = _consumer.Consume();
                    var orderCompletedEvent = JsonSerializer.Deserialize<OrderCompletedEvent>(consumeResult.Message.Value);

                    if (orderCompletedEvent != null)
                    {
                        // Update product review eligibility based on the order completion event
                        await mediator.Send(new CreateReviewEligibilityCommand(
                            Guid.Parse(orderCompletedEvent.UserId),
                            orderCompletedEvent.ProductIds
                        ));
                        Console.WriteLine($"Notification sent for order completion to user: {orderCompletedEvent.UserId}");
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
    }
}

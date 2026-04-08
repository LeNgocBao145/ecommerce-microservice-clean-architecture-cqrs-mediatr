namespace OrderService.Application.Interfaces
{
    public interface IEventBus
    {
        Task PublishCompletedOrderAsync<T>(string? id, string topic, T message);
    }
}

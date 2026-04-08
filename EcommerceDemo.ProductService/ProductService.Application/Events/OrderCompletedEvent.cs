namespace ProductService.Application.Events
{
    public record OrderCompletedEvent(
        string UserId,
        ICollection<Guid> ProductIds
        );
}

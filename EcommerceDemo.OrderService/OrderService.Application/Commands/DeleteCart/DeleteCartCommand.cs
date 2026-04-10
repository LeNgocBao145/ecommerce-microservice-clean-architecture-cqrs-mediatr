using MediatR;

namespace OrderService.Application.Commands.DeleteCart
{
    /// <summary>
    /// Command to delete cart items for a user after successful order placement.
    /// </summary>
    public record DeleteCartCommand(Guid UserId) : IRequest<DeleteCartResult>;

    /// <summary>
    /// Result DTO for delete cart operation.
    /// </summary>
    public record DeleteCartResult(bool Success, int DeletedItemCount, string Message);
}

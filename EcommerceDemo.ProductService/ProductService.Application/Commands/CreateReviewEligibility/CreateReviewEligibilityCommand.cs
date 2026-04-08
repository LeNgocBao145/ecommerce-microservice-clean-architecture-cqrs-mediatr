using MediatR;

namespace ProductService.Application.Commands.CreateReviewEligibility
{
    public record CreateReviewEligibilityCommand(Guid UserId, ICollection<Guid> ProductIds) : IRequest<bool>;
}

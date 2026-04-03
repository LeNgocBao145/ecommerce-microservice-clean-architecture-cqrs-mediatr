using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid UserId,
        Guid ProductId,
        string Comment,
        int Rating
    ) : IRequest<ReviewDTO>;
}

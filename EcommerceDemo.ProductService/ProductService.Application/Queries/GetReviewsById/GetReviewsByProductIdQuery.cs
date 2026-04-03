using MediatR;
using ProductService.Application.Common;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetReviewsById
{
    public record GetReviewsByProductIdQuery(
        Guid ProductId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PagedResponse<ReviewDTO>>;
}

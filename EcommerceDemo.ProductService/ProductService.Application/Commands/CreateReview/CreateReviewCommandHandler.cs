using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.CreateReview
{
    public class CreateReviewCommandHandler(IReviewRepository reviewRepository,
        IMapper mapper) : IRequestHandler<CreateReviewCommand, ReviewDTO>
    {
        public async Task<ReviewDTO> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            if (!(await reviewRepository.CheckIsEligibleForReviewAsync(request.ProductId, request.UserId)))
            {
                throw new InvalidOperationException("User is not eligible to review this product.");
            }

            var review = mapper.Map<ProductReview>(request);
            var createdReview = await reviewRepository.CreateAsync(review);
            if (createdReview == null)
            {
                return null;
            }
            return mapper.Map<ReviewDTO>(createdReview);
        }
    }
}

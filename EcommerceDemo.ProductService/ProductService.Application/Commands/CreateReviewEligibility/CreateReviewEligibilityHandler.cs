using MediatR;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.CreateReviewEligibility
{
    public class CreateReviewEligibilityHandler(IReviewRepository reviewRepository) : IRequestHandler<CreateReviewEligibilityCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;

        public async Task<bool> Handle(CreateReviewEligibilityCommand request, CancellationToken cancellationToken)
        {
            foreach (var productId in request.ProductIds)
            {
                var isEligible = await _reviewRepository.CheckIsEligibleForReviewAsync(productId, request.UserId);
                if (!isEligible)
                {
                    var reviewEligibility = new ReviewEligibility
                    {
                        UserId = request.UserId,
                        ProductId = productId
                    };
                    await _reviewRepository.AddReviewEligibilityAsync(reviewEligibility);
                }
            }
            return true;
        }
    }
}

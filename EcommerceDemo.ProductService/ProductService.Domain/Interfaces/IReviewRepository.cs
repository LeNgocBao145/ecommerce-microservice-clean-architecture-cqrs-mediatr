using Ecommerce.SharedLibrary.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<ProductReview>
    {
        public IQueryable<ProductReview> GetQueryable();
        Task<int> CountAsync();
        Task<bool> CheckIsEligibleForReviewAsync(Guid productId, Guid userId);
    }
}

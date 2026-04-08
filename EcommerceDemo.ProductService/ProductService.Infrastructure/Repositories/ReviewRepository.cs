using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace ProductService.Infrastructure.Repositories
{
    public class ReviewRepository(AppDbContext _context) : IReviewRepository
    {
        public IQueryable<ProductReview> GetQueryable()
        {
            return _context.Reviews.AsNoTracking(); // Trả về câu truy vấn "chưa thực thi"
        }
        public async Task<ProductReview> CreateAsync(ProductReview entity)
        {
            _context.Reviews.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> AddReviewEligibilityAsync(ReviewEligibility entity)
        {
            await _context.ReviewEligibilities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var product = await _context.Reviews.FindAsync(id);
            if (product == null)
                return 0;

            _context.Reviews.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<ProductReview?> FindByIdAsync(Guid id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ProductReview>> GetAllAsync()
        {
            return await _context.Reviews
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductReview?> GetByAsync(Expression<Func<ProductReview, bool>> predicate)
        {
            return await _context.Reviews
                .Where(predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ProductReview?> UpdateAsync(ProductReview entity)
        {
            var existingProduct = await _context.Reviews.FirstOrDefaultAsync(p => p.Id == entity.Id);
            if (existingProduct == null)
                return null;

            _context.Entry(existingProduct).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Reviews.CountAsync();
        }

        public async Task<bool> CheckIsEligibleForReviewAsync(Guid productId, Guid userId)
        {
            // Kiểm tra nếu người dùng đã đánh giá sản phẩm này chưa
            var existingReview = await _context.ReviewEligibilities
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
            if (existingReview == null) return false;
            return true; // Nếu chưa có đánh giá nào, người dùng có thể đánh giá
        }
    }
}

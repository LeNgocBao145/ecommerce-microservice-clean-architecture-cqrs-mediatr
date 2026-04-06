using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class UnitOfWork(AppDbContext context,
        ICartRepository cartRepository, IOrderRepository orderRepository) : IUnitOfWork, IDisposable
    {
        private bool _disposed = false;

        public ICartRepository CartRepository => cartRepository;
        public IOrderRepository OrderRepository => orderRepository;

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                context.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

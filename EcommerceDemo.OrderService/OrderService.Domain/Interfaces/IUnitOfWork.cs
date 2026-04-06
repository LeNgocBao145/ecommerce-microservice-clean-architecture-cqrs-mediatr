namespace OrderService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// SaveAsync is used to commit changes to the database. It wraps the call to SaveChangesAsync on the database context.
        /// </summary>
        /// <returns></returns>

        Task<int> SaveAsync(CancellationToken cancellationToken = default);

        ICartRepository CartRepository { get; }
        IOrderRepository OrderRepository { get; }
    }
}

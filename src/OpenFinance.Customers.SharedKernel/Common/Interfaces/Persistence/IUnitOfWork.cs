namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.Persistence
{
    /// <summary>
    /// Unit of Work pattern for transaction management
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Begins a new transaction
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Commits the current transaction
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves changes with an implicit transaction
        /// </summary>
        Task<bool> SaveChangesWithTransactionAsync(CancellationToken cancellationToken = default);
    }
}
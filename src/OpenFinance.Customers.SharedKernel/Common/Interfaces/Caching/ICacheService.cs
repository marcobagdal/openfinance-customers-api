namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.Caching
{
    /// <summary>
    /// Cache service abstraction
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a value from cache
        /// </summary>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sets a value in cache with optional expiration
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Removes a value from cache
        /// </summary>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Checks if a key exists in cache
        /// </summary>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets or creates a value from cache (cache-aside pattern)
        /// </summary>
        Task<T> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T>> factory, 
            TimeSpan? expiration = null, 
            CancellationToken cancellationToken = default);
    }
}
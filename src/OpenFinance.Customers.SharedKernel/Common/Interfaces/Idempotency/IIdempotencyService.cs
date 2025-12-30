namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.Idempotency
{
    /// <summary>
    /// Idempotency service to prevent duplicate processing of requests
    /// Critical for Open Finance and payment operations
    /// </summary>
    public interface IIdempotencyService
    {
        /// <summary>
        /// Checks if a request with the given idempotency key has already been processed
        /// </summary>
        Task<bool> IsProcessedAsync(string idempotencyKey, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the cached response for a previously processed idempotent request
        /// </summary>
        Task<TResponse> GetCachedResponseAsync<TResponse>(
            string idempotencyKey, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Stores the result of an idempotent request for future reference
        /// </summary>
        Task StoreResultAsync<TResponse>(
            string idempotencyKey, 
            TResponse response, 
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Creates or gets an idempotency key for a request
        /// </summary>
        Task<string> CreateIdempotencyKeyAsync(
            string requestType,
            object requestData,
            string? userId = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Marks a request as in progress to prevent concurrent processing
        /// </summary>
        Task<bool> TryAcquireLockAsync(
            string idempotencyKey, 
            TimeSpan lockDuration,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Releases the lock on an idempotency key
        /// </summary>
        Task ReleaseLockAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    }
}
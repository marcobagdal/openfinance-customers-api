namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.RateLimiting
{
    /// <summary>
    /// Rate limiting service to prevent API abuse
    /// </summary>
    public interface IRateLimitService
    {
        /// <summary>
        /// Checks if a request is allowed based on rate limiting rules
        /// </summary>
        Task<RateLimitResult> CheckRateLimitAsync(
            string endpoint,
            string identifier, // Could be IP, UserId, API Key, etc.
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Increments the request count for rate limiting
        /// </summary>
        Task IncrementRequestCountAsync(
            string endpoint,
            string identifier,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets current rate limit status for an identifier
        /// </summary>
        Task<RateLimitStatus> GetRateLimitStatusAsync(
            string endpoint,
            string identifier,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Resets rate limiting for an identifier
        /// </summary>
        Task ResetRateLimitAsync(
            string endpoint,
            string identifier,
            CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Rate limit result
    /// </summary>
    public record RateLimitResult(
        bool IsAllowed,
        int RemainingRequests,
        int Limit,
        int ResetAfterSeconds,
        string RetryAfter);
    
    /// <summary>
    /// Rate limit status
    /// </summary>
    public record RateLimitStatus(
        string Endpoint,
        string Identifier,
        int RequestCount,
        int Limit,
        int RemainingRequests,
        string Window, // e.g., "1m", "1h", "1d"
        DateTime ResetTime,
        bool IsBlocked);
}
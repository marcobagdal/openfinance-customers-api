namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.Audit
{
    /// <summary>
    /// Audit trail service for compliance and tracking
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Logs an audit entry for a user action
        /// </summary>
        Task LogActionAsync(
            string userId,
            string action,
            string entityType,
            string entityId,
            object? oldValues = null,
            object? newValues = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Logs an audit entry for Open Finance compliance
        /// </summary>
        Task LogOpenFinanceAuditAsync(
            string userId,
            string consentId,
            string action,
            string resource,
            object? metadata = null,
            string?fapiInteractionId = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets audit trail for a specific entity
        /// </summary>
        Task<AuditTrailResult> GetAuditTrailAsync(
            string entityType,
            string entityId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Audit trail result
    /// </summary>
    public class AuditTrailResult
    {
        public AuditEntry[] Entries { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    /// <summary>
    /// Audit entry
    /// </summary>
    public class AuditEntry
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public string FapiInteractionId { get; set; } = string.Empty;
        public string ConsentId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
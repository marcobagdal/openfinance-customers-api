namespace OpenFinance.Customers.SharedKernel.ValueObjects
{
    /// <summary>
    /// Value object for financial transaction IDs
    /// Open Finance requires traceable transaction identifiers
    /// </summary>
    public sealed record OpenFinanceResourceId
    {
        public string Value { get; }
        public DateTime CreatedAt { get; }
        public TransactionIdType Type { get; }
        
        private OpenFinanceResourceId(string value, TransactionIdType type)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Transaction ID cannot be empty", nameof(value));
            
            if (value.Length > 100)
                throw new ArgumentException("Transaction ID cannot exceed 100 characters", nameof(value));
            
            Value = value.Trim();
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }
        
        // Factory methods for different transaction types
        public static OpenFinanceResourceId CreateCustomerResourceId(string custormerDocument)
        {
            return new OpenFinanceResourceId(custormerDocument, TransactionIdType.Customer);
        }
        
        public static OpenFinanceResourceId CreateInternalId()
        {
            // Generate a GUID-based ID for internal transactions
            var id = $"INT-{Guid.NewGuid():N}";
            return new OpenFinanceResourceId(id, TransactionIdType.Internal);
        }
        
        public bool IsInternal => Type == TransactionIdType.Internal;
        public bool IsExternal => Type == TransactionIdType.External;
        
        public bool IsValidForOpenFinance => 
            Type != TransactionIdType.Internal && 
            !string.IsNullOrWhiteSpace(Value) &&
            Value.Length >= 10;
        
        public string GetTraceString() => 
            $"[{Type}] {Value} ({CreatedAt:yyyy-MM-dd HH:mm:ss})";
        
        public override string ToString() => Value;
        
        public enum TransactionIdType
        {
            Internal,
            External,
            Customer
        }
    }
}
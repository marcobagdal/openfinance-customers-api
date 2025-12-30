namespace OpenFinance.Customers.SharedKernel.Common.Interfaces.Security
{
    /// <summary>
    /// Service for authorization operations
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Checks if the current user has a specific permission
        /// </summary>
        Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Checks if the current user has any of the specified permissions
        /// </summary>
        Task<bool> HasAnyPermissionAsync(string[] permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Checks if the current user has all of the specified permissions
        /// </summary>
        Task<bool> HasAllPermissionsAsync(string[] permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the current user ID
        /// </summary>
        string GetCurrentUserId();
        
        /// <summary>
        /// Gets the current user roles
        /// </summary>
        string[] GetCurrentUserRoles();
    }
}
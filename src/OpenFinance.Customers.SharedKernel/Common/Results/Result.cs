namespace OpenFinance.Customers.SharedKernel.Common.Results
{
    /// <summary>
    /// Generic result wrapper for operation outcomes
    /// </summary>
    /// <typeparam name="T">Type of the result value</typeparam>
    public class Result<T> : Result
    {
        public T? Value { get; }
        
        protected internal Result(T? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            Value = value;
        }
        
        public static Result<T> Success(T value) => new(value, true, Error.None);
        public static new Result<T> Failure(Error error) => new(default, false, error);
        public static Result<T> Failure(string message) => new(default, false, Error.Generic(message));
    }
    
    /// <summary>
    /// Non-generic result for void operations
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        
        protected Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error ?? Error.None;
        }
        
        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result Failure(string message) => new(false, Error.Generic(message));
        
        // Helper methods for generic results
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
        public static Result<T> Failure<T>(string message) => Result<T>.Failure(message);
    }
    
    /// <summary>
    /// Error value object
    /// </summary>
    public sealed record Error
    {
        public string Code { get; }
        public string Message { get; }
        
        public static readonly Error None = new(string.Empty, string.Empty);
        
        private Error(string code, string message)
        {
            Code = code;
            Message = message;
        }
        
        // Common error factory methods
        public static Error NullValue => new("Error.NullValue", "The specified result value is null.");
        public static Error Validation(string[] errors) => new("Error.Validation", string.Join("; ", errors));
        public static Error Validation(string error) => new("Error.Validation", error);
        public static Error NotFound(string resource) => new("Error.NotFound", $"The requested '{resource}' was not found.");
        public static Error Unauthorized => new("Error.Unauthorized", "Access denied.");
        public static Error Forbidden => new("Error.Forbidden", "Insufficient permissions.");
        public static Error Conflict(string message) => new("Error.Conflict", message);
        public static Error RateLimited => new("Error.RateLimited", "Too many requests.");
        public static Error Generic(string message) => new("Error.Generic", message);
        
        public static implicit operator Error(string message) => Generic(message);
    }
    
    /// <summary>
    /// Extensions for Result pattern
    /// </summary>
    public static class ResultExtensions
    {
        public static Result<T> Ensure<T>(
            this Result<T> result,
            Func<T, bool> predicate,
            Error error)
        {
            if (result.IsFailure)
                return result;
            
            return predicate(result.Value) 
                ? result 
                : Result<T>.Failure(error);
        }
        
        public static Result<TOut> Map<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> mapping)
        {
            return result.IsSuccess 
                ? Result.Success(mapping(result.Value))
                : Result.Failure<TOut>(result.Error);
        }
        
        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Task<TOut>> mapping)
        {
            if (result.IsFailure)
                return Result.Failure<TOut>(result.Error);
            
            var mappedValue = await mapping(result.Value);
            return Result.Success(mappedValue);
        }
        
        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Task<Result<TOut>>> binding)
        {
            return result.IsSuccess 
                ? await binding(result.Value)
                : Result.Failure<TOut>(result.Error);
        }
        
        public static Result<T> Tap<T>(
            this Result<T> result,
            Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);
            
            return result;
        }
        
        public static async Task<Result<T>> TapAsync<T>(
            this Result<T> result,
            Func<T, Task> asyncAction)
        {
            if (result.IsSuccess)
                await asyncAction(result.Value);
            
            return result;
        }
        
        public static T Match<T>(
            this Result result,
            Func<T> onSuccess,
            Func<Error, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }
        
        public static TOut Match<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> onSuccess,
            Func<Error, TOut> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }
    }
}
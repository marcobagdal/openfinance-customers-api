using Microsoft.Extensions.Logging;

namespace OpenFinance.Customers.CrossCutting.Logging;

public static partial class LoggingDefinitions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [START] {RequestName}")]
    public static partial void LogRequestStart(this ILogger logger, DateTimeOffset timestamp, string requestName);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [SUCCESS] {RequestName} concluído em {ElapsedMs:F2}ms")]
    public static partial void LogRequestSuccess(this ILogger logger, DateTimeOffset timestamp, string requestName, double elapsedMs);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [FAILURE] {RequestName} falhou ({Code}) em {ElapsedMs:F2}ms")]
    public static partial void LogRequestFailure(this ILogger logger, DateTimeOffset timestamp, string requestName, string code, double elapsedMs);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [ERROR] {RequestName} após {ElapsedMs:F2}ms")]
    public static partial void LogRequestError(this ILogger logger, Exception ex, DateTimeOffset timestamp, string requestName, double elapsedMs);
}

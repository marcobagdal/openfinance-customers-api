using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using OpenFinance.Customers.SharedKernel.Common.Interfaces;
using OpenFinance.Customers.SharedKernel.Common.Results;

namespace OpenFinance.Customers.CrossCutting.Telemetry;

public sealed class PerformanceDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    ILogger<PerformanceDecorator<TRequest, TResponse>> logger)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private static readonly Meter Meter = new("OpenFinance.Customers.Handlers");
    private static readonly Histogram<double> ResponseTimeHistogram = 
        Meter.CreateHistogram<double>("handler.response_time", "ms", "Duração dos Handlers");

    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            return await innerHandler.Handle(request, ct);
        }
        finally
        {
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            var elapsedMs = elapsed.TotalMilliseconds;

            ResponseTimeHistogram.Record(elapsedMs, 
                new KeyValuePair<string, object?>("Request", requestName));

            if (elapsedMs > 1500)
            {
                logger.LogCritical(
                    "[SLA-VIOLATION] {RequestName} demorou {ElapsedMs}ms! Limite: 1500ms", 
                    requestName, elapsedMs);
            }
            else if (elapsedMs > 1000)
            {
                logger.LogWarning(
                    "[PERFORMANCE-WARNING] {RequestName} próximo ao limite: {ElapsedMs}ms", 
                    requestName, elapsedMs);
            }
        }
    }
}
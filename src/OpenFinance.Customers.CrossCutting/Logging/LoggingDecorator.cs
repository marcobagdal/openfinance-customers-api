using Microsoft.Extensions.Logging;
using OpenFinance.Customers.SharedKernel.Common.Interfaces;
using OpenFinance.Customers.SharedKernel.Common.Results;
using System.Diagnostics;

namespace OpenFinance.Customers.CrossCutting.Logging;

public sealed class LoggingDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    ILogger<LoggingDecorator<TRequest, TResponse>> logger,
    TimeProvider timeProvider) 
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var startTimeStamp = Stopwatch.GetTimestamp();
        
        // Chamada do Delegate gerado (Zero Parsing em Runtime)
        logger.LogRequestStart(timeProvider.GetUtcNow(), requestName);

        try
        {
            var response = await innerHandler.Handle(request, ct);
            
            var elapsedMs = Stopwatch.GetElapsedTime(startTimeStamp).TotalMilliseconds;
            var now = timeProvider.GetUtcNow();

            if (response.IsSuccess)
            {
                logger.LogRequestSuccess(now, requestName, elapsedMs);
            }
            else
            {
                logger.LogRequestFailure(now, requestName, response.Error.Code, elapsedMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTimeStamp).TotalMilliseconds;
            logger.LogRequestError(ex, timeProvider.GetUtcNow(), requestName, elapsedMs);
            throw;
        }
    }
}
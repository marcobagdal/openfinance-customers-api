using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using OpenFinance.Customers.SharedKernel.Common.Interfaces;
using OpenFinance.Customers.SharedKernel.Common.Results;

namespace OpenFinance.Customers.CrossCutting.Caching;

public sealed class CachingDecorator<TRequest, TResponse> 
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IRequestHandler<TRequest, TResponse> _innerHandler;
    private readonly HybridCache _cache;
    private readonly ILogger<CachingDecorator<TRequest, TResponse>> _logger;

    public CachingDecorator(
        IRequestHandler<TRequest, TResponse> innerHandler,
        HybridCache cache,
        ILogger<CachingDecorator<TRequest, TResponse>> logger)
    {
        _innerHandler = innerHandler;
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        // Se não for cacheável, passa direto
        if (request is not ICacheable cacheable)
        {
            return await _innerHandler.Handle(request, cancellationToken);
        }

        return await _cache.GetOrCreateAsync(
            cacheable.CacheKey,
            async (ct) => await _innerHandler.Handle(request, ct),
            new HybridCacheEntryOptions
            {
                Expiration = cacheable.Expiration ?? TimeSpan.FromMinutes(30),
                LocalCacheExpiration = cacheable.Expiration ?? TimeSpan.FromMinutes(30)
            },
            tags: cacheable.Tags,
            cancellationToken: cancellationToken
        );
    }
}

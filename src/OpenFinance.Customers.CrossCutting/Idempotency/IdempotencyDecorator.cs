using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using OpenFinance.Customers.SharedKernel.Common.Interfaces;
using OpenFinance.Customers.SharedKernel.Common.Results;
using System.IO.Hashing;
using OpenFinance.Customers.SharedKernel.Common.Interfaces.Idempotency;
using System.Buffers;
public sealed class IdempotencyDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    HybridCache cache,
    ILogger<IdempotencyDecorator<TRequest, TResponse>> logger) 
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        if (request is not IIdempotentRequest idempotent)
            return await innerHandler.Handle(request, ct);

        var requestHash = GenerateRequestHash(request);

        var cacheKey = $"idemp:{typeof(TRequest).Name}:{idempotent.IdempotencyKey}";

        var container = await cache.GetOrCreateAsync(
            cacheKey,
            async token => 
            {
                logger.LogInformation("[IDEMPOTENCY] Executando original: {Key}", idempotent.IdempotencyKey);
                var response = await innerHandler.Handle(request, token);
                return new IdempotencyContainer<TResponse>(response, requestHash);
            },
            cancellationToken: ct
        );

        if (container.RequestHash != requestHash)
        {
            logger.LogWarning("[IDEMPOTENCY] Conflito detectado para {Key}", idempotent.IdempotencyKey);
            return (TResponse)Result.Failure(Error.Conflict("Requisição com conteúdo diferente."));
        }

        return container.Response;
    }

    private static ulong GenerateRequestHash(TRequest request)
    {
        // Aluga um buffer da memória do .NET (Zero alocação nova no Heap)
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(bufferWriter);

        // Serializa o objeto diretamente para o buffer binário
        // No .NET 10, o JsonSerializer é otimizado quando usado com Utf8JsonWriter
        JsonSerializer.Serialize(writer, request);
        writer.Flush();

        // Gera o Hash XxHash64 a partir do buffer sem criar strings
        return XxHash64.HashToUInt64(bufferWriter.WrittenSpan);
    }
}

internal record IdempotencyContainer<TResponse>(TResponse Response, ulong RequestHash);

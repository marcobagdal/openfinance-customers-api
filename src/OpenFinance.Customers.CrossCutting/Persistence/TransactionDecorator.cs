using OpenFinance.Customers.SharedKernel.Common.Interfaces;
using OpenFinance.Customers.SharedKernel.Common.Results;
using Microsoft.Extensions.Logging;
using OpenFinance.Customers.SharedKernel.Common.Interfaces.Persistence;

namespace OpenFinance.Customers.CrossCutting.Persistence;

public sealed class TransactionDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    IUnitOfWork unitOfWork,
    ILogger<TransactionDecorator<TRequest, TResponse>> logger) 
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken ct)
    {
        if (!IsCommand())
        {
            return await innerHandler.Handle(request, ct);
        }

        logger.LogInformation("[TRANSACTION] Iniciando transação para {RequestName}", typeof(TRequest).Name);

        try
        {
            await unitOfWork.BeginTransactionAsync(ct);

            var response = await innerHandler.Handle(request, ct);

            if (response.IsSuccess)
            {
                await unitOfWork.CommitTransactionAsync(ct);
                logger.LogInformation("[TRANSACTION] Commit realizado com sucesso.");
            }
            else
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                logger.LogWarning("[TRANSACTION] Rollback executado devido a falha no resultado.");
            }

            return response;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            logger.LogError(ex, "[TRANSACTION] Rollback CRÍTICO devido a exceção não tratada.");
            throw;
        }
    }

    private static bool IsCommand() => 
        typeof(TRequest).Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase);
}

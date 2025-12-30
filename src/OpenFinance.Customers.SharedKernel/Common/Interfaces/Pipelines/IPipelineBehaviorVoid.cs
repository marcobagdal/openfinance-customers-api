namespace OpenFinance.Customers.SharedKernel.Common.Interfaces
{
     /// <summary>
    /// Pipeline behavior for void requests (no response)
    /// </summary>
    public interface IPipelineBehaviorVoid<TRequest>
        where TRequest : IRequest
    {
        Task Handle(
            TRequest request,
            Func<Task> next,
            CancellationToken cancellationToken);
    }
}
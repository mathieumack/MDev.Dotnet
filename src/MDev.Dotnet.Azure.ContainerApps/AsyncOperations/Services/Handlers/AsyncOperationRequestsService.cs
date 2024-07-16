using Microsoft.Extensions.DependencyInjection;
using MDev.Dotnet.AspNetCore.AsyncOperations.Abstracts;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;
using ServiceCollectionHelpers.AssemblyFinder.Attributes;

namespace MDev.Dotnet.AspNetCore.AsyncOperations.Services.Handlers;

[ServiceRegister(Scope = ServiceLifetime.Scoped)]
public class AsyncOperationRequestsService
{
    private readonly IEnumerable<IAsyncOperationRequestMessageHandler> handlers;

    public AsyncOperationRequestsService(IEnumerable<IAsyncOperationRequestMessageHandler> handlers)
    {
        this.handlers = handlers;
    }

    internal async Task HandleAsync(AsyncOperationRequestMessage item, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            if (handler.HandlerOperationName.Equals(item.OperationName))
                await handler.HandleRequest(item, cancellationToken);
        }
    }
}

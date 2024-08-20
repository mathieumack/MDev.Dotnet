using Microsoft.Extensions.DependencyInjection;
using MDev.Dotnet.AspNetCore.AsyncOperations.Abstracts;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;
using ServiceCollectionHelpers.AssemblyFinder.Attributes;
using Microsoft.Extensions.Logging;

namespace MDev.Dotnet.AspNetCore.AsyncOperations.Services.Handlers;

[ServiceRegister(Scope = ServiceLifetime.Scoped)]
public class AsyncOperationRequestsService
{
    private readonly ILogger<AsyncOperationRequestsService> logger;
    private readonly IEnumerable<IAsyncOperationRequestMessageHandler> handlers;

    public AsyncOperationRequestsService(ILogger<AsyncOperationRequestsService> logger,
                                            IEnumerable<IAsyncOperationRequestMessageHandler> handlers)
    {
        this.logger = logger;
        this.handlers = handlers;
    }

    internal async Task HandleAsync(AsyncOperationRequestMessage item, CancellationToken cancellationToken)
    {
        if(!handlers.Any(e => e.HandlerOperationName.Equals(item.OperationName)))
        {
            logger.LogInformation($"async.operation : No service registered for {item.OperationName}");
            return;
        }

        foreach (var handler in handlers.Where(e => e.HandlerOperationName.Equals(item.OperationName)))
        {
            logger.LogInformation($"call async.operation.{handler.HandlerOperationName}");
            await handler.HandleRequest(item, cancellationToken);
        }
    }
}

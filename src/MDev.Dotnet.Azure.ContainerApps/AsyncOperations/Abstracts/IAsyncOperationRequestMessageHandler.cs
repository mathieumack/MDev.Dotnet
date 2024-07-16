using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;

namespace MDev.Dotnet.AspNetCore.AsyncOperations.Abstracts;

public interface IAsyncOperationRequestMessageHandler
{
    /// <summary>
    /// Name of operation linked to the message
    /// </summary>
    string HandlerOperationName { get; }

    /// <summary>
    /// Manage handler of async completion request
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleRequest(AsyncOperationRequestMessage message, CancellationToken cancellationToken);
}

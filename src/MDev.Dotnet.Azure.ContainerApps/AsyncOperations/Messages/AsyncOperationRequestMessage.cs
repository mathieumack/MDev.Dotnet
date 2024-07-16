namespace MDev.Dotnet.AspNetCore.AsyncOperations.Messages;

public class AsyncOperationRequestMessage
{
    /// <summary>
    /// Request identifier (ex : Correlation Id) 
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Linked User id that generate the original request
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Name of requested operation.
    /// Depending on the operation name, the service that will handle the message will be different
    /// </summary>
    public string OperationName { get; set; }

    /// <summary>
    /// Metadatas fullfill by the caller.
    /// Metadatas will be sent in output message
    /// </summary>
    public Dictionary<string, string> Metadatas { get; set; }
}

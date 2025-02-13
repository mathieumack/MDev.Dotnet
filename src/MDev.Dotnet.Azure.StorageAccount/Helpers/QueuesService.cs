using Azure.Storage.Queues;

namespace MDev.Dotnet.Azure.StorageAccount.Helpers;

public class QueuesService
{
    private readonly List<QueueClient> queueClients;

    public QueuesService(List<QueueClient> queueClients)
    {
        this.queueClients = queueClients;
    }

    public async Task SendMessagesAsync<T>(List<T> messageObjects,
                                            TimeSpan? visibilityTimeout = null,
                                            TimeSpan? timeToLive = null,
                                            CancellationToken cancellationToken = default)
    {
        if (messageObjects is null)
            return;

        for (int i = 0; i < messageObjects.Count; i++)
        {
            await SendMessageAsync(messageObjects[i], 
                                    i % queueClients.Count,
                                    visibilityTimeout,
                                    timeToLive,
                                    cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Push an object into a Binary message to an available queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageAsync<T>(T messageObject,
                                            TimeSpan? visibilityTimeout = null,
                                            TimeSpan? timeToLive = null,
                                            CancellationToken cancellationToken = default)
    {
        await SendMessageAsync(messageObject,
                                Random.Shared.Next(0, queueClients.Count),
                                visibilityTimeout,
                                timeToLive,
                                cancellationToken);
    }

    /// <summary>
    /// Push an object into a Binary message to an available queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task SendMessageAsync<T>(T messageObject, 
                                            int queueIndex,
                                            TimeSpan? visibilityTimeout = null, 
                                            TimeSpan? timeToLive = null,
                                            CancellationToken cancellationToken = default)
    {
        if (messageObject is null)
            return;

        // Send async completion request :
        var queueClient = queueClients[queueIndex];
        await queueClient.SendMessageAsync(new BinaryData(messageObject),
                                            visibilityTimeout: visibilityTimeout,
                                            timeToLive: timeToLive,
                                            cancellationToken: cancellationToken);
    }
}

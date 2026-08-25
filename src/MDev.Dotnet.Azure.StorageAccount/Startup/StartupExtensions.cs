using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MDev.Dotnet.Azure.StorageAccount.Settings;
using Azure.Core;
using MDev.Dotnet.Azure.StorageAccount.Helpers;

namespace MDev.Dotnet.Azure.StorageAccount.Startup;

public static class StartupExtensions
{
    /// <summary>
    /// Register Azure storage configuration
    /// Register a <see cref="BlobServiceClient"/> as scoped for storage access.
    /// Register keyed queue services for sending and queue clients for retrieval.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterAzureStorage(this IHostApplicationBuilder builder,
                                                                TokenCredential credentials)
    {
        // Storage account
        var storageSettings = new StorageAccountSettings();
        builder.Configuration.GetRequiredSection(StorageAccountSettings.SectionName)
            .Bind(storageSettings, options =>
            {
                options.ErrorOnUnknownConfiguration = true;
            });

        builder.Services.AddScoped(sp =>
        {
            return new BlobServiceClient(new Uri(storageSettings.BlobsEndpoint), credentials);
        });

        var queueClientOptions = new QueueClientOptions();
        if (storageSettings.QueueMessagesEncodeBase64)
            queueClientOptions.MessageEncoding = QueueMessageEncoding.Base64;

        if (storageSettings.Queues != null && storageSettings.Queues.Any())
        {
            foreach (var queue in storageSettings.Queues)
            {
                var queueClients = new List<QueueClient>();
                // Register n queue clients for same key
                for (int i = 0; i < queue.Queues.Count; i++)
                {

                    var client = new QueueClient(new Uri($"{storageSettings.QueuesEndpoint}/{queue.Queues[i]}"), credentials, queueClientOptions);
                    queueClients.Add(client);
                }

                builder.Services.AddKeyedSingleton(queue.Id, new QueuesService(queueClients));
            }
        }

        foreach (var queueName in storageSettings.QueueClients)
        {
            var client = new QueueClient(new Uri($"{storageSettings.QueuesEndpoint}/{queueName}"), credentials, queueClientOptions);
            builder.Services.AddKeyedSingleton(queueName, client);
        }

        return builder;
    }
}

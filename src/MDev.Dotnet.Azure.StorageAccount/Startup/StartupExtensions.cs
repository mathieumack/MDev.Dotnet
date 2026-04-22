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
    /// Register keynamed <see cref="QueueClient"/> for each configured queue.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterAzureStorage(this IHostApplicationBuilder builder,
                                                                TokenCredential credentials)
    {
        builder.Services.RegisterAzureStorage(builder.Configuration, credentials);
        return builder;
    }

    /// <summary>
    /// Register Azure storage configuration
    /// Register a <see cref="BlobServiceClient"/> as scoped for storage access.
    /// Register keynamed <see cref="QueueClient"/> for each configured queue.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterAzureStorage(this IServiceCollection services,
                                                           IConfiguration configuration,
                                                           TokenCredential credentials)
    {
        // Storage account
        var storageSettings = new StorageAccountSettings();
        configuration.GetRequiredSection(StorageAccountSettings.SectionName)
            .Bind(storageSettings, options =>
            {
                options.ErrorOnUnknownConfiguration = true;
            });

        services.AddScoped(sp =>
        {
            return new BlobServiceClient(new Uri(storageSettings.BlobsEndpoint), credentials);
        });

        if (storageSettings.Queues != null && storageSettings.Queues.Any())
        {
            var queuClientOptions = new QueueClientOptions();
            if (storageSettings.QueueMessagesEncodeBase64)
                queuClientOptions.MessageEncoding = QueueMessageEncoding.Base64;

            foreach (var queue in storageSettings.Queues)
            {
                var queueClients = new List<QueueClient>();
                // Register n queue clients for same key
                for (int i = 0; i < queue.Queues.Count; i++)
                {

                    var client = new QueueClient(new Uri($"{storageSettings.QueuesEndpoint}/{queue.Queues[i]}"), credentials, queuClientOptions);
                    queueClients.Add(client);
                }

                services.AddKeyedSingleton(queue.Id, new QueuesService(queueClients));
            }
        }

        return services;
    }
}

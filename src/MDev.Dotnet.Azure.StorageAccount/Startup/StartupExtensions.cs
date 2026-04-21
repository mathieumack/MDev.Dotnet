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
    /// Register Azure storage configuration.
    /// Registers a scoped <see cref="BlobServiceClient"/> for blob access and
    /// a keyed-singleton <see cref="QueuesService"/> for each queue group defined in configuration.
    /// </summary>
    /// <param name="builder">The application host builder.</param>
    /// <param name="credentials">The Azure token credential used to authenticate against Storage.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> so calls can be chained.</returns>
    public static IHostApplicationBuilder RegisterAzureStorage(this IHostApplicationBuilder builder,
                                                                TokenCredential credentials)
    {
        builder.Services.RegisterAzureStorage(builder.Configuration, credentials);
        return builder;
    }

    /// <summary>
    /// Register Azure storage configuration on an <see cref="IServiceCollection"/>.
    /// Registers a scoped <see cref="BlobServiceClient"/> for blob access and
    /// a keyed-singleton <see cref="QueuesService"/> for each queue group defined in configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="credentials">The Azure token credential used to authenticate against Storage.</param>
    /// <returns>The same <see cref="IServiceCollection"/> so calls can be chained.</returns>
    public static IServiceCollection RegisterAzureStorage(this IServiceCollection services,
                                                           IConfiguration configuration,
                                                           TokenCredential credentials)
    {
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
            var queueClientOptions = new QueueClientOptions();
            if (storageSettings.QueueMessagesEncodeBase64)
                queueClientOptions.MessageEncoding = QueueMessageEncoding.Base64;

            foreach (var queue in storageSettings.Queues)
            {
                var queueClients = new List<QueueClient>();
                // Register n queue clients for same key
                for (int i = 0; i < queue.Queues.Count; i++)
                {
                    var client = new QueueClient(new Uri($"{storageSettings.QueuesEndpoint}/{queue.Queues[i]}"), credentials, queueClientOptions);
                    queueClients.Add(client);
                }

                services.AddKeyedSingleton(queue.Id, new QueuesService(queueClients));
            }
        }

        return services;
    }
}

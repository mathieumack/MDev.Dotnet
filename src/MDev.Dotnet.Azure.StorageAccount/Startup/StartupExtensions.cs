using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MDev.Dotnet.Azure.StorageAccount.Settings;
using Azure.Core;

namespace MDev.Dotnet.Azure.StorageAccount.Startup;

public static class StartupExtensions
{
    /// <summary>
    /// Register Azure storage configuration
    /// Register a <see cref="BlobServiceClient"/> as scoped for storage access.
    /// Register keynamed <see cref="QueueClient"/> for each <paramref name="queues"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="queues"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterAzureStorage(this IHostApplicationBuilder builder,
                                                                TokenCredential credentials,
                                                                Dictionary<string, int> queues = null)
    {
        // Storage account
        var storageSettings = new StorageAccountSettings();
        builder.Configuration.GetRequiredSection(StorageAccountSettings.SectionName)
            .Bind(storageSettings, options => options.ErrorOnUnknownConfiguration = true);
        builder.Services.AddScoped(sp =>
        {
            return new BlobServiceClient(new Uri(storageSettings.BlobsEndpoint), credentials);
        });

        if (queues != null)
        {
            foreach (var queue in queues.Keys)
            {
                // Register n queue clients for same key
                for (int i = 0; i < queues[queue]; i++)
                {
                    builder.Services.AddKeyedSingleton(queue,
                        new QueueClient(new Uri($"{storageSettings.QueuesEndpoint}/{queue}-{i}"), credentials)
                    );
                }
            }
        }

        return builder;
    }
}

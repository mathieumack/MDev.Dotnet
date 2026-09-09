using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using MDev.Dotnet.Azure.StorageAccount.Helpers;
using MDev.Dotnet.Azure.StorageAccount.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MDev.Dotnet.Azure.StorageAccount.Tests;

public class StorageAccountRegistrationTests
{
    [Fact]
    public void RegisterAzureStorage_RegistersConfiguredClients()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["StorageAccount:BlobsEndpoint"] = "https://storage.example.com",
            ["StorageAccount:QueuesEndpoint"] = "https://storage.example.com",
            ["StorageAccount:QueueClients:0"] = "notifications",
            ["StorageAccount:Queues:0:Id"] = "workers",
            ["StorageAccount:Queues:0:Queues:0"] = "worker-1"
        });

        builder.RegisterAzureStorage(new TestCredential());

        using var provider = builder.Services.BuildServiceProvider();
        Assert.Equal(
            new Uri("https://storage.example.com/"),
            provider.GetRequiredService<BlobServiceClient>().Uri);
        Assert.Equal(
            new Uri("https://storage.example.com/notifications"),
            provider.GetRequiredKeyedService<QueueClient>("notifications").Uri);
        Assert.NotNull(provider.GetRequiredKeyedService<QueuesService>("workers"));
    }

    private sealed class TestCredential : TokenCredential
    {
        public override AccessToken GetToken(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken) =>
            new("token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult(new AccessToken("token", DateTimeOffset.MaxValue));
    }
}

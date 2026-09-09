using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs.Tests;

public class ManagedIdentityExtensionsTests
{
    [Fact]
    public void GetManagedIdentity_UsesSystemAssignedIdentityWhenClientIdIsAbsent()
    {
        var configuration = new ConfigurationBuilder().Build();

        var credential = configuration.GetManagedIdentity();

        Assert.IsType<ManagedIdentityCredential>(credential);
    }

    [Fact]
    public void GetManagedIdentity_UsesConfiguredUserAssignedClientId()
    {
        var configuration = Configuration("22f239ba-1c7b-4cf1-898c-15c6a6c49647");

        var credential = new ServiceCollection()
            .GetManagedIdentity(configuration);

        Assert.IsType<ManagedIdentityCredential>(credential);
    }

    [Fact]
    public void GetManagedIdentity_RejectsMalformedClientId()
    {
        const string invalidClientId = "not-a-client-id";

        var exception = Assert.Throws<InvalidOperationException>(() =>
            Configuration(invalidClientId).GetManagedIdentity());

        Assert.Contains("AZURE_CLIENT_ID", exception.Message);
        Assert.DoesNotContain(invalidClientId, exception.Message);
    }

    private static IConfiguration Configuration(string clientId) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AZURE_CLIENT_ID"] = clientId
            })
            .Build();
}

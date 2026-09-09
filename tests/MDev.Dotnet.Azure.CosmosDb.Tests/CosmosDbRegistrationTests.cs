using Azure.Core;
using MDev.Dotnet.Azure.CosmosDb.Startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MDev.Dotnet.Azure.CosmosDb.Tests;

public class CosmosDbRegistrationTests
{
    [Fact]
    public void RegisterCosmosDb_ConfiguresTheCosmosProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CosmosDb:Endpoint"] = "https://localhost:8081",
                ["CosmosDb:DatabaseName"] = "unit-tests"
            })
            .Build();
        var services = new ServiceCollection();

        services.RegisterCosmosDb<TestDbContext>(configuration, new TestCredential());

        using var provider = services.BuildServiceProvider();
        using var context = provider.GetRequiredService<TestDbContext>();
        Assert.Equal("Microsoft.EntityFrameworkCore.Cosmos", context.Database.ProviderName);
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options);

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

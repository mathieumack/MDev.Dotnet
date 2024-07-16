using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MDev.Dotnet.Azure.CosmosDb.Settings;
using Azure.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace MDev.Dotnet.Azure.CosmosDb.Startup;

public static class StartupExtensions
{
    /// <summary>
    /// Register cosmosDb database context.
    /// Create a Scoped <see cref="T"/> object based on <see cref="CosmosDbSettings"/> configuration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterCosmosDb<T>(this IHostApplicationBuilder builder,
                                                                    TokenCredential credentials) where T : DbContext
    {
        // Register DbContext :
        var cosmosDbSettings = new CosmosDbSettings();
        builder.Configuration.GetRequiredSection(CosmosDbSettings.SectionName).Bind(cosmosDbSettings, options => options.ErrorOnUnknownConfiguration = true);
        builder.Services.AddDbContext<T>(options =>
        {
            options.UseCosmos(cosmosDbSettings.Endpoint, credentials, cosmosDbSettings.DatabaseName);
        });

        return builder;
    }
}

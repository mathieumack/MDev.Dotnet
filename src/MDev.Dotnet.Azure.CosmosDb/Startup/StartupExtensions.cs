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
        builder.Services.RegisterCosmosDb<T>(builder.Configuration, credentials);
        return builder;
    }

    /// <summary>
    /// Register cosmosDb database context.
    /// Create a Scoped <see cref="T"/> object based on <see cref="CosmosDbSettings"/> configuration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCosmosDb<T>(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                TokenCredential credentials) where T : DbContext
    {
        // Register DbContext :
        var cosmosDbSettings = new CosmosDbSettings();
        configuration.GetRequiredSection(CosmosDbSettings.SectionName).Bind(cosmosDbSettings, options => options.ErrorOnUnknownConfiguration = true);
        services.AddDbContext<T>(options =>
        {
            options.UseCosmos(cosmosDbSettings.Endpoint, credentials, cosmosDbSettings.DatabaseName);
        });

        return services;
    }
}

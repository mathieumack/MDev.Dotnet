using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MDev.Dotnet.AspNetCore.Apis.Extensions;

public static class StartupRegistersExtensions
{
    /// <summary>
    /// Register Controllers with logging for 400 bad requests errors.
    /// Add also versionning for APIs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="allowSynchronousIO">Indicate if synchronous IO must be enabled for HTTP Steams</param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterControllers<T>(this IHostApplicationBuilder builder, bool allowSynchronousIO = false)
    {
        builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Add logging for error 400
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
                        // Get an instance of ILogger (see below) and log accordingly.
                        logger.LogWarning("Invalid model state: {Errors}", context.ModelState.GetErrors());

                        return new BadRequestObjectResult(context.ModelState);
                    };
                });

        if (allowSynchronousIO)
        {
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        builder.Services.AddRouting(route => route.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApiVersioning();

        return builder;
    }

    /// <summary>
    /// Register app settings files with configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterConfiguration(this IHostApplicationBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Configuration.AddConfiguration(config);

        return builder;
    }

    /// <summary>
    /// Bing configuration object on section
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="bindObject">Output binded object</param>
    /// <param name="sectionName">section name from configuration</param>
    /// <returns></returns>
    public static IHostApplicationBuilder BindConfiguration<T>(this IHostApplicationBuilder builder, out T bindObject, string sectionName) where T : class, new()
    {
        bindObject = new();

        builder.Services.Configure<T>(builder.Configuration.GetRequiredSection(sectionName));

        builder.Configuration.GetRequiredSection(sectionName)
            .Bind(bindObject, options => options.ErrorOnUnknownConfiguration = true);

        return builder;
    }

    /// <summary>
    /// Bing configuration object to be available with IOptions<T>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="sectionName">section name from configuration</param>
    /// <returns></returns>
    public static IHostApplicationBuilder BindConfiguration<T>(this IHostApplicationBuilder builder, string sectionName) where T : class, new()
    {
        builder.Services.Configure<T>(builder.Configuration.GetRequiredSection(sectionName));

        return builder;
    }
} 

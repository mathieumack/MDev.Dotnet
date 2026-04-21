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
    /// <param name="mvcOptions">Allow to override or execute operations on controllers options</param>
    /// <returns></returns>
    public static IHostApplicationBuilder RegisterControllers<T>(this IHostApplicationBuilder builder, 
                                                                    bool allowSynchronousIO = false, 
                                                                    Action<MvcOptions> mvcOptions = null)
    {
        builder.Services.AddControllers(options =>
                {
                    if (mvcOptions != null)
                        mvcOptions(options);
                })
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
    /// Bind configuration object on section and also make it available via <see cref="IOptions{T}"/>.
    /// Throws if the section is missing from configuration.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="builder"></param>
    /// <param name="bindObject">Output bound object.</param>
    /// <param name="sectionName">Section name from configuration.</param>
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
    /// Bind configuration object to be available with <see cref="IOptions{T}"/>.
    /// Throws if the section is missing from configuration.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="builder"></param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IHostApplicationBuilder BindConfiguration<T>(this IHostApplicationBuilder builder, string sectionName) where T : class, new()
    {
        builder.Services.Configure<T>(builder.Configuration.GetRequiredSection(sectionName));

        return builder;
    }

    /// <summary>
    /// Attempt to bind an optional configuration section.
    /// If the section does not exist the method returns without registering anything and
    /// <paramref name="bindObject"/> is set to a default-constructed instance.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="builder"></param>
    /// <param name="bindObject">Output bound object; default-constructed when the section is absent.</param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IHostApplicationBuilder TryBindConfiguration<T>(this IHostApplicationBuilder builder, out T bindObject, string sectionName) where T : class, new()
    {
        bindObject = new();

        var section = builder.Configuration.GetSection(sectionName);
        if (!section.Exists())
            return builder;

        builder.Services.Configure<T>(section);
        section.Bind(bindObject);

        return builder;
    }

    /// <summary>
    /// Attempt to bind an optional configuration section so that it is available via
    /// <see cref="IOptions{T}"/>.  If the section does not exist the method returns
    /// without registering anything.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="builder"></param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IHostApplicationBuilder TryBindConfiguration<T>(this IHostApplicationBuilder builder, string sectionName) where T : class, new()
    {
        var section = builder.Configuration.GetSection(sectionName);
        if (!section.Exists())
            return builder;

        builder.Services.Configure<T>(section);

        return builder;
    }

    /// <summary>
    /// Bind a configuration section to a settings object and also make it available via <see cref="IOptions{T}"/>.
    /// Throws if the section is missing from configuration.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="bindObject">Output bound object.</param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IServiceCollection BindConfiguration<T>(this IServiceCollection services, IConfiguration configuration, out T bindObject, string sectionName) where T : class, new()
    {
        bindObject = new();

        services.Configure<T>(configuration.GetRequiredSection(sectionName));

        configuration.GetRequiredSection(sectionName).Bind(bindObject, options => options.ErrorOnUnknownConfiguration = true);

        return services;
    }

    /// <summary>
    /// Bind a configuration section to a settings object so that it is available via <see cref="IOptions{T}"/>.
    /// Throws if the section is missing from configuration.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IServiceCollection BindConfiguration<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class, new()
    {
        services.Configure<T>(configuration.GetRequiredSection(sectionName));

        return services;
    }

    /// <summary>
    /// Attempt to bind an optional configuration section.
    /// If the section does not exist the method returns without registering anything and
    /// <paramref name="bindObject"/> is set to a default-constructed instance.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="bindObject">Output bound object; default-constructed when the section is absent.</param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IServiceCollection TryBindConfiguration<T>(this IServiceCollection services, IConfiguration configuration, out T bindObject, string sectionName) where T : class, new()
    {
        bindObject = new();

        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
            return services;

        services.Configure<T>(section);
        section.Bind(bindObject);

        return services;
    }

    /// <summary>
    /// Attempt to bind an optional configuration section so that it is available via
    /// <see cref="IOptions{T}"/>.  If the section does not exist the method returns
    /// without registering anything.
    /// </summary>
    /// <typeparam name="T">Type of the settings object.</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName">Section name from configuration.</param>
    /// <returns></returns>
    public static IServiceCollection TryBindConfiguration<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
            return services;

        services.Configure<T>(section);

        return services;
    }
} 

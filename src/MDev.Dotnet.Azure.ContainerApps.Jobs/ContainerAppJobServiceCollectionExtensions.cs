using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs;

/// <summary>
/// Registers Azure Container Apps Job services.
/// </summary>
public static class ContainerAppJobServiceCollectionExtensions
{
    /// <summary>
    /// Adds generic-host observability and graceful telemetry flushing for a Container Apps Job.
    /// </summary>
    public static IServiceCollection AddContainerAppJob(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ContainerAppJobOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new ContainerAppJobOptions();
        configure?.Invoke(options);
        options.ServiceName ??= configuration["CONTAINER_APP_JOB_NAME"]
            ?? configuration["CONTAINER_APP_NAME"];

        Validate(options, configuration);

        services.AddSingleton<IOptions<ContainerAppJobOptions>>(Options.Create(options));

        if (options.AddConsoleLogging)
        {
            services.AddLogging(logging => logging.AddConsole());
        }

        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(options.ServiceName!))
            .WithLogging(logging =>
            {
                if (options.EnableAzureMonitor)
                {
                    logging.AddAzureMonitorLogExporter(exporter =>
                        exporter.ConnectionString = connectionString);
                }
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(options.ServiceName!);
                if (options.EnableAzureMonitor)
                {
                    tracing.AddAzureMonitorTraceExporter(exporter =>
                        exporter.ConnectionString = connectionString);
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(options.ServiceName!);
                if (options.EnableAzureMonitor)
                {
                    metrics.AddAzureMonitorMetricExporter(exporter =>
                        exporter.ConnectionString = connectionString);
                }
            });

        services.AddHostedService<TelemetryFlushHostedService>();
        return services;
    }

    private static void Validate(ContainerAppJobOptions options, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new InvalidOperationException(
                "A telemetry service name is required. Set ContainerAppJobOptions.ServiceName or CONTAINER_APP_JOB_NAME.");
        }

        if (options.FlushTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options.FlushTimeout),
                "The telemetry flush timeout must be greater than zero.");
        }

        if (!options.EnableAzureMonitor)
        {
            return;
        }

        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "APPLICATIONINSIGHTS_CONNECTION_STRING is required when Azure Monitor is enabled.");
        }

        var instrumentationKey = connectionString
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => part.Split('=', 2, StringSplitOptions.TrimEntries))
            .FirstOrDefault(part =>
                part.Length == 2
                && part[0].Equals("InstrumentationKey", StringComparison.OrdinalIgnoreCase));

        if (instrumentationKey is null || !Guid.TryParse(instrumentationKey[1], out _))
        {
            throw new InvalidOperationException(
                "APPLICATIONINSIGHTS_CONNECTION_STRING is not a valid Application Insights connection string.");
        }
    }
}

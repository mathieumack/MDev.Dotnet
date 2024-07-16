using Azure.Monitor.OpenTelemetry.AspNetCore;
using MDev.Dotnet.AspNetCore.OpenTelemetry.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MDev.Dotnet.AspNetCore.OpenTelemetry.Apis.Extensions;

public static class StartupRegistersExtensions
{
    /// <summary>
    /// Register OpenTeleetry as ILogger.
    /// Clear all logging providers.
    /// Works only in Release mode. In Debug nothing os done, only console logger is registered
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="metricsConfigurationMeters">List of meter names that must be configured for mtrics for OpenTelemetry</param>
    public static IHostApplicationBuilder RegisterOpenTelemetry(this IHostApplicationBuilder builder,
                                                                OpenTelemetrySettings openTelemetrySettings,
                                                                List<string> metricsConfigurationMeters = null)
    {
        builder.Logging.ClearProviders();

        var meters = new List<string>()
        {
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http"
        };

        if (metricsConfigurationMeters != null)
            meters.AddRange(metricsConfigurationMeters);

        // OTEL_EXPORTER_OTLP_ENDPOINT is supported natively by Container Apps (https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents?tabs=arm#environment-variables)
        if (openTelemetrySettings.ServiceType == "AppInsights")
        {
            // OpenTelemety with Dynatrace :
            builder.Services.AddOpenTelemetry()
                .UseAzureMonitor()
                .WithMetrics(metricsConfiguration =>
                {
                    metricsConfiguration
                        .AddMeter(meters.ToArray());
                })
                .WithTracing(tracesConfiguration =>
                {
                });
        }
        else
        {
            var endpoint = builder.Configuration.GetValue<string>("OTEL_ENDPOINT");
            var endPointAuhorization = builder.Configuration.GetValue<string>("OTEL_ENDPOINT_AUTH");
            var containerAppName = builder.Configuration.GetValue<string>("CONTAINER_APP_NAME");
            var containerAppRevisionName = builder.Configuration.GetValue<string>("CONTAINER_APP_REPLICA_NAME");

            // OpenTelemety with AppInsights :
            List<KeyValuePair<string, object>> dt_metadata = new List<KeyValuePair<string, object>>();

            Action<ResourceBuilder> configureResource = r => r
                        .AddService(serviceName: containerAppName) //TODO Replace with the name of your application
                        .AddAttributes(dt_metadata);

            builder.Services.AddOpenTelemetry()
                    .ConfigureResource(configureResource)
                .WithLogging(builderDyn =>
                {
                    builderDyn
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(endpoint + "/v1/logs");
                            options.Protocol = OtlpExportProtocol.HttpProtobuf; // Only grpc supported by Container Apps
                            if(!string.IsNullOrWhiteSpace(endPointAuhorization))
                                options.Headers = $"Authorization={endPointAuhorization}";
                        });
                })
                .WithMetrics(builderDyn =>
                {
                    builderDyn
                        .AddMeter(meters.ToArray())
                        .AddOtlpExporter((OtlpExporterOptions exporterOptions, MetricReaderOptions readerOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri(endpoint + "/v1/metrics");
                            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf; // Only grpc supported by Container Apps
                            if (!string.IsNullOrWhiteSpace(endPointAuhorization))
                                exporterOptions.Headers = $"Authorization={endPointAuhorization}";
                            readerOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                        });
                });
        }

        return builder;
    }
}

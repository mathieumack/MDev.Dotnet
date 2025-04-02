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
using System.Diagnostics;

namespace MDev.Dotnet.AspNetCore.OpenTelemetry.Apis.Extensions;

public static class StartupRegistersExtensions
{
    /// <summary>
    /// Bing configuration object on section
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static IHostApplicationBuilder BindConfiguration<T>(this IHostApplicationBuilder builder, out T bindObject, string sectionName) where T : class, new()
    {
        bindObject = new();

        builder.Services.Configure<T>(builder.Configuration.GetRequiredSection(sectionName));

        builder.Configuration.GetRequiredSection(sectionName)
            .Bind(bindObject, options => options.ErrorOnUnknownConfiguration = true);

        return builder;
    }

    /// <summary>
    /// Register OpenTelemetry as ILogger.
    /// Clear all logging providers.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="metricsConfigurationMeters">List of meter names that must be configured for mtrics for OpenTelemetry</param>
    public static IHostApplicationBuilder RegisterOpenTelemetry(this IHostApplicationBuilder builder,
                                                                List<string> metricsConfigurationMeters = null)
    {
        builder.BindConfiguration(out OpenTelemetrySettings openTelemetrySettings, OpenTelemetrySettings.SectionName);

        builder.Logging.ClearProviders();

        var meters = new List<string>()
        {
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http"
        };

        if (metricsConfigurationMeters != null)
            meters.AddRange(metricsConfigurationMeters);

        builder.Services.RegisterOpenTelemetry(builder.Configuration, meters);

        return builder;
    }

    /// <summary>
    /// Register OpenTelemetry as ILogger.
    /// Clear all logging providers.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="metricsConfigurationMeters">List of meter names that must be configured for mtrics for OpenTelemetry</param>
    public static IServiceCollection RegisterOpenTelemetry(this IServiceCollection servicesCollection,
                                                                IConfiguration configuration,
                                                                List<string> meters)
    {
        var openTelemetrySettings = new OpenTelemetrySettings();
        configuration.GetRequiredSection(OpenTelemetrySettings.SectionName)
                .Bind(openTelemetrySettings, options => options.ErrorOnUnknownConfiguration = true);

        // OTEL_EXPORTER_OTLP_ENDPOINT is supported natively by Container Apps (https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents?tabs=arm#environment-variables)
        if (openTelemetrySettings.ServiceType == "AppInsights")
        {
            // OpenTelemety with Dynatrace :
            servicesCollection.AddOpenTelemetry()
                .UseAzureMonitor()
                .WithMetrics(metricsConfiguration =>
                {
                    metricsConfiguration
                        .AddMeter(meters.ToArray());
                })
                .WithTracing(tracesConfiguration =>
                {
                    tracesConfiguration.AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            if (response.StatusCode == 404)
                            {
                                activity.SetStatus(ActivityStatusCode.Ok);
                            }
                        };
                    });
                });
        }
        else
        {
            var endpoint = configuration.GetValue<string>("OTEL_ENDPOINT");
            var endPointAuhorization = configuration.GetValue<string>("OTEL_ENDPOINT_AUTH");
            var containerAppName = configuration.GetValue<string>("CONTAINER_APP_NAME");
            var containerAppRevisionName = configuration.GetValue<string>("CONTAINER_APP_REPLICA_NAME");

            // OpenTelemety with AppInsights :
            List<KeyValuePair<string, object>> dt_metadata = new List<KeyValuePair<string, object>>();

            Action<ResourceBuilder> configureResource = r => r
                        .AddService(serviceName: containerAppName) //TODO Replace with the name of your application
                        .AddAttributes(dt_metadata);

            servicesCollection.AddOpenTelemetry()
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
                .WithTracing(tracesConfiguration =>
                {
                    tracesConfiguration.AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            if (response.StatusCode == 404)
                            {
                                activity.SetStatus(ActivityStatusCode.Ok);
                            }
                        };
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

        return servicesCollection;
    }
}

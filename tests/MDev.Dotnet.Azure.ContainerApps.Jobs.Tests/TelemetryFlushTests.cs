using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs.Tests;

public class TelemetryFlushTests
{
    [Fact]
    public async Task StopAsync_ExportsFinalLogsAndActivities()
    {
        const string serviceName = "flush-test-job";
        var logExporter = new MatchingExporter<LogRecord>(
            record => record.Body?.ToString()?.Contains("final job log") == true);
        var activityExporter = new MatchingExporter<Activity>(
            activity => activity.DisplayName == "final job activity");
        var metricExporter = new MatchingExporter<Metric>(
            metric => metric.Name == "completed-items");
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddHostedService<FinalTelemetryHostedService>();
        builder.Services.AddContainerAppJob(
            builder.Configuration,
            options =>
            {
                options.ServiceName = serviceName;
                options.AddConsoleLogging = false;
                options.FlushTimeout = TimeSpan.FromSeconds(5);
            });
        builder.Services.AddOpenTelemetry()
            .WithLogging(logging => logging.AddProcessor(
                new BatchLogRecordExportProcessor(
                    logExporter, 2048, 60_000, 30_000, 512)))
            .WithTracing(tracing => tracing.AddProcessor(
                new BatchActivityExportProcessor(
                    activityExporter, 2048, 60_000, 30_000, 512)))
            .WithMetrics(metrics => metrics.AddReader(
                new PeriodicExportingMetricReader(metricExporter, 60_000)));

        using var host = builder.Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(0, logExporter.MatchCount);
        Assert.Equal(0, activityExporter.MatchCount);
        Assert.Equal(0, metricExporter.MatchCount);

        await host.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, logExporter.MatchCount);
        Assert.Equal(1, activityExporter.MatchCount);
        Assert.Equal(1, metricExporter.MatchCount);
    }

    private sealed class FinalTelemetryHostedService(
        ILogger<FinalTelemetryHostedService> logger) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("final job log");
            using var source = new ActivitySource("flush-test-job");
            using var activity = source.StartActivity("final job activity");
            using var meter = new Meter("flush-test-job");
            meter.CreateCounter<int>("completed-items").Add(1);
            return Task.CompletedTask;
        }
    }

    private sealed class MatchingExporter<T>(Func<T, bool> matches) : BaseExporter<T>
        where T : class
    {
        private int matchCount;

        public int MatchCount => Volatile.Read(ref matchCount);

        public override ExportResult Export(in Batch<T> batch)
        {
            foreach (var item in batch)
            {
                if (matches(item))
                {
                    Interlocked.Increment(ref matchCount);
                }
            }

            return ExportResult.Success;
        }
    }
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs;

internal sealed class TelemetryFlushHostedService(
    IOptions<ContainerAppJobOptions> options,
    IHostApplicationLifetime lifetime,
    LoggerProvider loggerProvider,
    TracerProvider tracerProvider,
    MeterProvider meterProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        lifetime.ApplicationStopped.Register(Flush);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void Flush()
    {
        var providerTimeout = (int)Math.Min(
            options.Value.FlushTimeout.TotalMilliseconds / 3,
            int.MaxValue);

        TryFlush(() => loggerProvider.ForceFlush(providerTimeout));
        TryFlush(() => tracerProvider.ForceFlush(providerTimeout));
        TryFlush(() => meterProvider.ForceFlush(providerTimeout));
    }

    private static void TryFlush(Func<bool> flush)
    {
        try
        {
            flush();
        }
        catch
        {
            // Telemetry export must not replace the job's original failure or exit code.
        }
    }
}

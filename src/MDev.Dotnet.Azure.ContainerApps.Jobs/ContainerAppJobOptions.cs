namespace MDev.Dotnet.Azure.ContainerApps.Jobs;

/// <summary>
/// Configures observability for an Azure Container Apps Job.
/// </summary>
public sealed class ContainerAppJobOptions
{
    /// <summary>
    /// Gets or sets the OpenTelemetry service, activity source, and meter name.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets or sets whether console logging is added. Defaults to <see langword="true"/>.
    /// </summary>
    public bool AddConsoleLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets whether Azure Monitor exporters are added.
    /// </summary>
    public bool EnableAzureMonitor { get; set; }

    /// <summary>
    /// Gets or sets the maximum time spent flushing telemetry during shutdown.
    /// </summary>
    public TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(10);
}

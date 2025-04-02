namespace MDev.Dotnet.AspNetCore.OpenTelemetry.Settings;

/// <summary>
/// Default configuration class for OpenTelemetry settings.
/// </summary>
public class OpenTelemetrySettings
{
    public const string SectionName = "OpenTelemetry";

    /// <summary>
    /// Type of service (ApplicationInsight, OenTelemetry, etc.).
    /// </summary>
    public string ServiceType { get; set; }
}

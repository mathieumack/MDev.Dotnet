# MDev.Dotnet.Azure.ContainerApps.Jobs

`MDev.Dotnet.Azure.ContainerApps.Jobs` adds generic-host observability and managed identity helpers for short-lived .NET console jobs. It has no ASP.NET Core or Dapr sidecar requirement.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.ContainerApps.Jobs
```

## Configure a job

```csharp
using System.Diagnostics;
using MDev.Dotnet.Azure.ContainerApps.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddContainerAppJob(
    builder.Configuration,
    options =>
    {
        options.ServiceName = "DocumentProcessingJob";
        options.AddConsoleLogging = true;
        options.EnableAzureMonitor = true;
        options.FlushTimeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IConfiguration>().GetManagedIdentity());
builder.Services.AddHostedService<DocumentProcessingWorker>();

using var host = builder.Build();
await host.RunAsync();

sealed class DocumentProcessingWorker(
    ILogger<DocumentProcessingWorker> logger,
    IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var source = new ActivitySource("DocumentProcessingJob");
            using var activity = source.StartActivity("Process documents");

            await ProcessDocumentsAsync(stoppingToken);
            logger.LogInformation("Document processing completed");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Document processing was cancelled");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Document processing failed");
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }

    private static Task ProcessDocumentsAsync(CancellationToken cancellationToken) =>
        Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
}
```

Registration adds providers rather than clearing existing logging configuration. `AddConsoleLogging` defaults to `true`. Consumers can add filters, providers, OpenTelemetry instrumentation, or exporters before or after this registration.

Use the same value for `ServiceName`, `ActivitySource`, and custom `Meter` names so traces and metrics are selected. When Azure Monitor is enabled, logs, traces, and metrics are exported with the configured Application Insights connection string.

## Configuration

| Setting | Required | Purpose |
| --- | --- | --- |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | When `EnableAzureMonitor` is `true` | Application Insights connection string; keep it in a Container Apps secret |
| `AZURE_CLIENT_ID` | No | Client ID of a user-assigned managed identity |
| `CONTAINER_APP_JOB_NAME` | When `ServiceName` is not set | OpenTelemetry service name |

If `AZURE_CLIENT_ID` is absent or blank, `GetManagedIdentity()` selects the job's system-assigned managed identity. The helper intentionally creates `ManagedIdentityCredential`; it does not fall back to developer credentials.

Missing or malformed required settings fail during registration with an error that does not include the configured secret.

## Shutdown and exit codes

Run work through `BackgroundService` and the generic host. On graceful completion, cancellation, or a handled failure, call `StopApplication` or allow host shutdown to proceed. The package force-flushes the OpenTelemetry log, trace, and metric providers with the configured bounded timeout before they are disposed.

Write final activities and logs before requesting shutdown. Set `Environment.ExitCode` for handled failures, as shown above; do not call `Environment.Exit`, because it bypasses the host lifecycle and telemetry flush. Exceptions are not intercepted by this package.

## Local development

Disable Azure Monitor when no development Application Insights resource is available:

```csharp
options.EnableAzureMonitor = !builder.Environment.IsDevelopment();
```

Managed identity endpoints are available only on supported Azure hosts. For local development, register a separate credential such as `DefaultAzureCredential` through an environment-specific application configuration; reserve `GetManagedIdentity()` for the deployed job.

## Deploy to Azure

1. Enable a system-assigned identity on the Container Apps Job, or attach a user-assigned identity and set `AZURE_CLIENT_ID` to its client ID.
2. Grant that identity only the roles required by the resources the job accesses.
3. Store the Application Insights connection string as a Container Apps secret and expose it through `APPLICATIONINSIGHTS_CONNECTION_STRING`.
4. Configure the job replica timeout to leave enough time for application cancellation and `FlushTimeout`.

See [jobs in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/jobs), [managed identities in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/managed-identity), and [manage secrets in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/manage-secrets).

## Resources

- [Getting started](../getting-started.md)
- [Source code](https://github.com/mathieumack/MDev.Dotnet/tree/main/src/MDev.Dotnet.Azure.ContainerApps.Jobs)
- [Azure Monitor OpenTelemetry for .NET](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore)

[Previous: Azure Container Apps](azure-containerapps.md) · [Documentation home](../index.md) · [Next: Azure Cosmos DB](azure-cosmosdb.md)

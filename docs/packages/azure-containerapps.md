# MDev.Dotnet.Azure.ContainerApps

`MDev.Dotnet.Azure.ContainerApps` adds Azure Container Apps integrations for observability, authentication, and asynchronous operations.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.ContainerApps
```

## Capabilities

- OpenTelemetry configuration for Azure Monitor or custom OTLP endpoints
- Metrics, tracing, and logging
- Access to the Container Apps authentication principal
- Dapr pub/sub request handlers
- Dapr API token validation

## Basic setup

Register OpenTelemetry with any application-specific meters:

```csharp
using MDev.Dotnet.AspNetCore.OpenTelemetry.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterOpenTelemetry(new List<string> { "MyApp.Orders" });

var app = builder.Build();
app.Run();
```

Select the telemetry destination in configuration:

```json
{
  "OpenTelemetry": {
    "ServiceType": "AppInsights",
    "IgnoreErrorStatusCode": [404, 401]
  }
}
```

See the [source code](../../src/MDev.Dotnet.Azure.ContainerApps/) for authentication and Dapr helpers, and [Azure Container Apps documentation](https://learn.microsoft.com/en-us/azure/container-apps/) for platform guidance.

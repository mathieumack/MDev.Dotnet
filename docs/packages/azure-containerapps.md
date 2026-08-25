# MDev.Dotnet.Azure.ContainerApps

`MDev.Dotnet.Azure.ContainerApps` adds Azure Container Apps integrations for observability, authentication, and asynchronous operations.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.ContainerApps
```

## Capabilities

- OpenTelemetry configuration for Azure Monitor or custom OTLP endpoints
- Metrics, tracing, and logging
- Access to Azure Container Apps authentication headers and claims
- Dispatch of Dapr request messages to application handlers
- Dapr API-token validation for controller actions

## OpenTelemetry

The `OpenTelemetry` section is required. `ServiceType` is case-sensitive: use `AppInsights` for Azure Monitor, or any other value for OTLP.

```csharp
using MDev.Dotnet.AspNetCore.OpenTelemetry.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterOpenTelemetry(new List<string> { "MyApp.Orders" });
```

```json
{
  "OpenTelemetry": {
    "ServiceType": "AppInsights",
    "IgnoreErrorStatusCode": [404, 401]
  }
}
```

Registration clears the existing logging providers. The package always includes ASP.NET Core, Kestrel, and HTTP client meters. Listed error status codes are marked as successful spans.

For OTLP, configure `OTEL_ENDPOINT` with the base HTTP endpoint. The package sends to `/v1/logs` and `/v1/metrics` using HTTP/protobuf. `OTEL_ENDPOINT_AUTH` is optional; `CONTAINER_APP_NAME` supplies the service name. See [OpenTelemetry agents in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents).

## Container Apps authentication

Enable the platform's [built-in authentication](https://learn.microsoft.com/en-us/azure/container-apps/authentication), register `IHttpContextAccessor`, and use the extension methods:

```csharp
using MDev.Dotnet.Azure.ContainerApps.Authentication.Extensions;

builder.Services.AddHttpContextAccessor();

// In a request-scoped service:
var userId = httpContextAccessor.GetUserId();
var fullName = httpContextAccessor.GetUserFullName(decode: true);
var claims = await httpContextAccessor.GetClaims();
```

These methods read `X-MS-CLIENT-PRINCIPAL-ID`, `X-MS-CLIENT-PRINCIPAL-NAME`, and the Base64-encoded `X-MS-CLIENT-PRINCIPAL` header. Only trust these headers when requests are protected by the Container Apps authentication proxy.

## Dapr async operations

Implement `IAsyncOperationRequestMessageHandler` for each operation and register implementations with dependency injection:

```csharp
using MDev.Dotnet.AspNetCore.AsyncOperations.Abstracts;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;

public sealed class RebuildIndexHandler
    : IAsyncOperationRequestMessageHandler
{
    public string HandlerOperationName => "rebuild-index";

    public Task HandleRequest(
        AsyncOperationRequestMessage message,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

builder.Services.AddScoped<
    IAsyncOperationRequestMessageHandler,
    RebuildIndexHandler>();
```

`AsyncOperationRequestsService` dispatches a message to every handler whose `HandlerOperationName` exactly matches its `OperationName`. Register that service and expose `DaprHandlerController.OperationRequestAsync` through the routing appropriate for your application.

The package controller has no route attribute. Add its assembly as an MVC application part and map it with conventional routing if you use it directly:

```csharp
using MDev.Dotnet.AspNetCore.AsyncOperations.Controllers.v1;
using MDev.Dotnet.AspNetCore.AsyncOperations.Services.Handlers;

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(DaprHandlerController).Assembly);
builder.Services.AddScoped<AsyncOperationRequestsService>();

app.MapControllerRoute(
    name: "dapr-operation",
    pattern: "operations/{action}",
    defaults: new { controller = "DaprHandler" });
```

With the default MVC action-name convention, the callback path is `/operations/OperationRequest`.

To protect a Dapr callback, set `APP_API_TOKEN` through secure configuration and apply `[RequireDaprApiToken]`. The caller must send the same value in the `dapr-api-token` header. See [Dapr API token authentication](https://docs.dapr.io/operations/security/api-token/).

## Resources

- [Getting started](../getting-started.md)
- [Source code](https://github.com/mathieumack/MDev.Dotnet/tree/main/src/MDev.Dotnet.Azure.ContainerApps)
- [Azure Container Apps documentation](https://learn.microsoft.com/en-us/azure/container-apps/)

[Previous: ASP.NET Core](aspnetcore.md) · [Documentation home](../index.md) · [Next: Azure Cosmos DB](azure-cosmosdb.md)

# MDev.Dotnet.AspNetCore

`MDev.Dotnet.AspNetCore` provides extension methods for common ASP.NET Core API configuration.

## Installation

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

## Capabilities

- `RegisterControllers<T>()` adds controllers, endpoint discovery, API versioning, lowercase URLs, and warning logs for invalid model state.
- `RegisterConfiguration()` loads `appsettings.json`, an optional environment-specific file, and environment variables.
- `BindConfiguration<T>()` registers a required configuration section with the options pattern.
- `RegisterOpenApi()` can rewrite generated server URLs to HTTPS or omit them.
- `AddRoutesPrefix()` applies a path base to the application.
- `RegisterControllers<T>(allowSynchronousIO: true)` enables synchronous Kestrel I/O when required by legacy code.

## Controllers and OpenAPI

```csharp
using MDev.Dotnet.AspNetCore.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterControllers<Program>(mvcOptions: options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.RegisterOpenApi(forceHttpsServers: true);

var app = builder.Build();

app.AddRoutesPrefix("/api");
app.MapControllers();
app.MapOpenApi();
app.Run();
```

`T` is the logging category used when model validation returns HTTP 400. Both `forceHttpsServers` and `includeServerUrls` are optional; for example, `RegisterOpenApi(includeServerUrls: false)` clears the server list.

## Configuration binding

`WebApplication.CreateBuilder` already loads standard application configuration. Call `RegisterConfiguration()` only when you explicitly want the package to rebuild and append its JSON and environment-variable sources.

```csharp
builder.RegisterConfiguration();
builder.BindConfiguration<MyAppSettings>("MyAppSettings");
```

Consume the bound section with `IOptions<MyAppSettings>`. To also obtain an eagerly bound instance, use the strict overload:

```csharp
builder.BindConfiguration<MyAppSettings>(
    out var settings,
    "MyAppSettings");
```

This overload rejects unknown configuration properties. All binding overloads require the named section.

## Route prefix

Call `AddRoutesPrefix` before mapping endpoints. The method accepts `api` or `/api`, sets `Request.PathBase`, and rejects an empty prefix.

## Resources

- [Getting started](../getting-started.md)
- [Source code](https://github.com/mathieumack/MDev.Dotnet/tree/main/src/MDev.Dotnet.AspNetCore)
- [ASP.NET Core fundamentals](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/)
- [OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)

[Documentation home](../index.md) · [Next: Azure Container Apps](azure-containerapps.md)

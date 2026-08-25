# MDev.Dotnet.AspNetCore

`MDev.Dotnet.AspNetCore` provides extension methods for common ASP.NET Core API configuration.

## Installation

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

## Capabilities

- Controller registration with model-validation logging
- API versioning
- Strongly typed configuration binding
- Lowercase URL routing
- OpenAPI configuration
- Global route prefixes
- Optional synchronous I/O support

## Basic setup

```csharp
using MDev.Dotnet.AspNetCore.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterConfiguration();
builder.RegisterControllers<Program>();
builder.RegisterOpenApi();

var app = builder.Build();

app.UseRoutePrefix("api");
app.MapControllers();
app.MapOpenApi();
app.Run();
```

Configuration can be bound to a settings type and registered for dependency injection:

```csharp
builder.BindConfiguration<MyAppSettings>("MyAppSettings");
```

See the [source code](../../src/MDev.Dotnet.AspNetCore/) for the available extensions and [ASP.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/) for framework guidance.

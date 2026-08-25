# Getting started

## Prerequisites

- The [.NET 9 or .NET 10 SDK](https://dotnet.microsoft.com/download)
- An ASP.NET Core project
- For Azure packages, an Azure identity with the required data-plane roles

## Install a package

From your project directory, install only the helper you need:

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

Replace the package name with `MDev.Dotnet.Azure.ContainerApps`, `MDev.Dotnet.Azure.CosmosDb`, or `MDev.Dotnet.Azure.StorageAccount` as appropriate. NuGet restores each package's dependencies; applications that construct credentials directly can also reference [`Azure.Identity`](https://www.nuget.org/packages/Azure.Identity).

### Install a preview package

Preview packages are published to GitHub Packages. Create a [personal access token (classic) with `read:packages` scope](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-with-a-personal-access-token-classic), then add the authenticated source:

```bash
dotnet nuget add source https://nuget.pkg.github.com/mathieumack/index.json \
  --name github \
  --username USERNAME \
  --password GITHUB_TOKEN \
  --store-password-in-clear-text
dotnet add package MDev.Dotnet.AspNetCore --prerelease --source github
```

Replace `USERNAME` with your GitHub username and `GITHUB_TOKEN` with the token. Keep the token out of source control.

## Create a minimal API application

The ASP.NET Core package works with `WebApplicationBuilder` through `IHostApplicationBuilder`:

```csharp
using MDev.Dotnet.AspNetCore.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterControllers<Program>();
builder.RegisterOpenApi();

var app = builder.Build();

app.AddRoutesPrefix("/api");
app.MapControllers();
app.MapOpenApi();
app.Run();
```

`RegisterControllers<T>()` adds controllers, lowercase routing, endpoint discovery, API versioning, and logging for invalid model state. `AddRoutesPrefix` is optional.

## Add an Azure integration

Azure service packages accept an [`Azure.Core.TokenCredential`](https://learn.microsoft.com/en-us/dotnet/api/azure.core.tokencredential). `DefaultAzureCredential` supports local developer credentials and managed identity without storing secrets:

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.CosmosDb.Startup;

builder.RegisterCosmosDb<MyDbContext>(new DefaultAzureCredential());
```

Configure the relevant package before registration, then assign the least-privileged Azure role required by the application. Continue with a package guide:

- [ASP.NET Core](packages/aspnetcore.md)
- [Azure Container Apps](packages/azure-containerapps.md)
- [Azure Cosmos DB](packages/azure-cosmosdb.md)
- [Azure Storage Account](packages/azure-storageaccount.md)

[Documentation home](index.md)

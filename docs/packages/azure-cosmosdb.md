# MDev.Dotnet.Azure.CosmosDb

`MDev.Dotnet.Azure.CosmosDb` registers Entity Framework Core contexts for Azure Cosmos DB using Azure token credentials.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.CosmosDb
```

## Capabilities

- Configuration-based Cosmos DB setup
- Generic `DbContext` registration
- `TokenCredential` and managed identity authentication
- Scoped context lifetime

## Basic setup

Define the required `CosmosDb` configuration section. Unknown properties cause registration to fail.

```json
{
  "CosmosDb": {
    "Endpoint": "https://your-account.documents.azure.com:443/",
    "DatabaseName": "MyAppDatabase"
  }
}
```

Register the context with an Azure credential:

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.CosmosDb.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterCosmosDb<MyAppDbContext>(new DefaultAzureCredential());
```

`MyAppDbContext` must derive from `Microsoft.EntityFrameworkCore.DbContext`. The helper calls `AddDbContext<T>()` and `UseCosmos(endpoint, credential, databaseName)`, so the context uses the standard scoped lifetime. An `IServiceCollection` overload is also available:

```csharp
services.RegisterCosmosDb<MyAppDbContext>(
    configuration,
    credential);
```

## Managed identity

`DefaultAzureCredential` uses a managed identity automatically in supported Azure hosts. For a user-assigned identity, configure its client ID using the options documented for [`DefaultAzureCredential`](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential).

Grant the identity an appropriate [Cosmos DB data-plane role](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-role-based-access); Azure resource-management roles alone do not grant access to items. Use local developer credentials rather than secrets during development.

## Resources

- [Getting started](../getting-started.md)
- [Source code](https://github.com/mathieumack/MDev.Dotnet/tree/main/src/MDev.Dotnet.Azure.CosmosDb)
- [EF Core Azure Cosmos DB provider](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/)
- [Azure-hosted application authentication](https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/system-assigned-managed-identity)

[Previous: Azure Container Apps Jobs](azure-containerapps-jobs.md) · [Documentation home](../index.md) · [Next: Azure Storage Account](azure-storageaccount.md)

# MDev.Dotnet.Azure.CosmosDb

`MDev.Dotnet.Azure.CosmosDb` registers Entity Framework Core contexts for Azure Cosmos DB using Azure token credentials.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.CosmosDb
dotnet add package Azure.Identity
```

## Capabilities

- Configuration-based Cosmos DB setup
- Generic `DbContext` registration
- `TokenCredential` and managed identity authentication
- Scoped context lifetime

## Basic setup

Define the `CosmosDb` configuration section:

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

See the [source code](../../src/MDev.Dotnet.Azure.CosmosDb/) for registration details and the [EF Core Azure Cosmos DB provider documentation](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/) for data-model guidance.

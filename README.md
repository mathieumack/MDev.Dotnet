# MDev.Dotnet

> Production-ready helpers for building .NET web APIs and integrating Azure services with less boilerplate.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MDev.Dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MDev.Dotnet)
[![.NET](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml)
[![NuGet](https://buildstats.info/nuget/MDev.Dotnet.AspNetCore?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.AspNetCore)
[![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.ContainerApps?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.ContainerApps)
[![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.CosmosDb?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.CosmosDb)
[![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.StorageAccount?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.StorageAccount)

## Highlight

MDev.Dotnet packages turn common ASP.NET Core and Azure integration tasks into consistent, dependency-injection-friendly registrations and services.

## Explain

Choose only the helpers your application needs:

| Package | Capabilities | Documentation |
| --- | --- | --- |
| [`MDev.Dotnet.AspNetCore`](https://nuget.org/packages/MDev.Dotnet.AspNetCore) | Controllers, configuration binding, API versioning, OpenAPI, and route prefixes | [Read the guide](docs/packages/aspnetcore.md) |
| [`MDev.Dotnet.Azure.ContainerApps`](https://nuget.org/packages/MDev.Dotnet.Azure.ContainerApps) | OpenTelemetry, Container Apps authentication, and Dapr async operations | [Read the guide](docs/packages/azure-containerapps.md) |
| [`MDev.Dotnet.Azure.CosmosDb`](https://nuget.org/packages/MDev.Dotnet.Azure.CosmosDb) | Entity Framework Core Cosmos DB registration with token credentials | [Read the guide](docs/packages/azure-cosmosdb.md) |
| [`MDev.Dotnet.Azure.StorageAccount`](https://nuget.org/packages/MDev.Dotnet.Azure.StorageAccount) | Blob persistence, SAS access, and keyed queue clients | [Read the guide](docs/packages/azure-storageaccount.md) |

## Resources

- [Documentation](docs/index.md)
- [Getting started](docs/getting-started.md)
- [Contributing](docs/contributing.md)
- [Source code](src/)
- [NuGet packages](https://www.nuget.org/profiles/mathieumack)
- [Microsoft Learn for .NET](https://learn.microsoft.com/en-us/dotnet/)
- [Microsoft Learn for Azure](https://learn.microsoft.com/en-us/azure/)
- [MDev.Dotnet Wiki](https://github.com/mathieumack/MDev.Dotnet/wiki)

## Operation

Install the package that matches your use case:

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

Replace the package name with any package listed above. The [getting-started guide](docs/getting-started.md) and each package guide contain current registration, configuration, and usage examples.

---

## Packages

### MDev.Dotnet.AspNetCore

#### Highlight

Build consistent ASP.NET Core APIs without repeating standard setup.

#### Explain

Register controllers and API versions, bind strongly typed configuration, configure OpenAPI, and apply route prefixes through focused extension methods.

#### Main APIs

| API | Purpose |
| --- | --- |
| `RegisterControllers<T>()` | Adds controllers, API versioning, lowercase URLs, and model-validation logging |
| `RegisterConfiguration()` | Loads JSON configuration and environment variables |
| `BindConfiguration<T>()` | Binds and registers a required configuration section |
| `RegisterOpenApi()` | Registers OpenAPI and optionally rewrites or removes server URLs |
| `AddRoutesPrefix()` | Applies a path base, such as `/api`, to the application |
| `GetErrors()` | Returns model-state errors as a single message |

#### Resources

- [Documentation](docs/packages/aspnetcore.md)
- [NuGet](https://nuget.org/packages/MDev.Dotnet.AspNetCore)
- [Source](src/MDev.Dotnet.AspNetCore/)
- [ASP.NET Core on Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/)

#### Operation

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

[Continue with setup and examples](docs/packages/aspnetcore.md).

### MDev.Dotnet.Azure.ContainerApps

#### Highlight

Add observability, authentication context, and asynchronous Dapr workflows to Azure Container Apps.

#### Explain

Configure OpenTelemetry for Azure Monitor or OTLP, access the authenticated Container Apps principal, and handle Dapr pub/sub operations.

#### Main APIs

| API | Purpose |
| --- | --- |
| `RegisterOpenTelemetry()` | Configures logs, metrics, and traces for Azure Monitor or OTLP |
| `GetUserId()` / `GetUserFullName()` | Reads identity values supplied by Container Apps authentication |
| `GetClaims()` | Decodes the authenticated principal's claims |
| `GetUserValue()` | Reads a named Container Apps authentication header |
| `AsyncOperationRequestsService` | Dispatches Dapr operation messages to matching handlers |
| `IAsyncOperationRequestMessageHandler` | Defines a handler for an asynchronous operation |
| `[RequireDaprApiToken]` | Validates the `dapr-api-token` request header |

#### Resources

- [Documentation](docs/packages/azure-containerapps.md)
- [NuGet](https://nuget.org/packages/MDev.Dotnet.Azure.ContainerApps)
- [Source](src/MDev.Dotnet.Azure.ContainerApps/)
- [Azure Container Apps on Microsoft Learn](https://learn.microsoft.com/en-us/azure/container-apps/)

#### Operation

```bash
dotnet add package MDev.Dotnet.Azure.ContainerApps
```

[Continue with setup and examples](docs/packages/azure-containerapps.md).

### MDev.Dotnet.Azure.CosmosDb

#### Highlight

Connect Entity Framework Core contexts to Azure Cosmos DB with secure Azure credentials.

#### Explain

Register a Cosmos DB `DbContext` from application configuration and authenticate with an Azure `TokenCredential`, including managed identity.

#### Main APIs

| API | Purpose |
| --- | --- |
| `RegisterCosmosDb<T>(TokenCredential)` | Registers an EF Core Cosmos DB context from the `CosmosDb` configuration section |
| `RegisterCosmosDb<T>(IConfiguration, TokenCredential)` | Provides the same registration from an `IServiceCollection` |

#### Resources

- [Documentation](docs/packages/azure-cosmosdb.md)
- [NuGet](https://nuget.org/packages/MDev.Dotnet.Azure.CosmosDb)
- [Source](src/MDev.Dotnet.Azure.CosmosDb/)
- [EF Core Azure Cosmos DB provider](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/)

#### Operation

```bash
dotnet add package MDev.Dotnet.Azure.CosmosDb
```

[Continue with setup and examples](docs/packages/azure-cosmosdb.md).

### MDev.Dotnet.Azure.StorageAccount

#### Highlight

Use Azure Blob Storage and Queue Storage through ready-to-inject services.

#### Explain

Register storage clients with Azure credentials, manage blobs and SAS access through `PersistentService`, and resolve configured queue clients by key.

#### Main APIs

| API | Purpose |
| --- | --- |
| `RegisterAzureStorage()` | Registers blob services and keyed queue clients from configuration |
| `PersistentService` | Creates and lists containers and uploads, downloads, copies, or deletes blobs |
| `RetreiveBlobUriAsync()` / `RetreiveBlobsUriAsync()` | Creates read-only SAS URIs for blobs |
| `QueuesService.SendMessageAsync<T>()` | Sends one message to a configured queue |
| `QueuesService.SendMessagesAsync<T>()` | Distributes multiple messages across configured queues |

#### Resources

- [Documentation](docs/packages/azure-storageaccount.md)
- [NuGet](https://nuget.org/packages/MDev.Dotnet.Azure.StorageAccount)
- [Source](src/MDev.Dotnet.Azure.StorageAccount/)
- [Azure Storage on Microsoft Learn](https://learn.microsoft.com/en-us/azure/storage/)

#### Operation

```bash
dotnet add package MDev.Dotnet.Azure.StorageAccount
```

[Continue with setup and examples](docs/packages/azure-storageaccount.md).

---

## Additional Resources

- [ASP.NET Core package guide](docs/packages/aspnetcore.md)
- [Azure Container Apps package guide](docs/packages/azure-containerapps.md)
- [Azure Cosmos DB package guide](docs/packages/azure-cosmosdb.md)
- [Azure Storage Account package guide](docs/packages/azure-storageaccount.md)
- [MDev.Dotnet Wiki](https://github.com/mathieumack/MDev.Dotnet/wiki)

## Support / Contribute

If you have a question, problem, or suggestion, [create an issue](https://github.com/mathieumack/MDev.Dotnet/issues) or fork the project and create a pull request.

See the [contributing guide](docs/contributing.md) for build and documentation guidance.

## Build Status

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MDev.Dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MDev.Dotnet)
[![.NET](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml)

## License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

---

**Made with ❤️ for the .NET Community**

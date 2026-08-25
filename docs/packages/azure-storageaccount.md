# MDev.Dotnet.Azure.StorageAccount

`MDev.Dotnet.Azure.StorageAccount` provides injectable helpers for Azure Blob Storage and Queue Storage.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.StorageAccount
```

## Capabilities

- Blob upload, download, copy, deletion, and metadata
- Container creation, deletion, and listing
- SAS URI generation
- Multiple keyed queue clients
- `TokenCredential` and managed identity authentication
- Optional Base64 queue-message encoding

## Basic setup

Define the storage endpoints and queues in configuration:

```json
{
  "StorageAccount": {
    "BlobsEndpoint": "https://your-account.blob.core.windows.net",
    "QueuesEndpoint": "https://your-account.queue.core.windows.net",
    "QueueMessagesEncodeBase64": false,
    "Queues": [
      {
        "Id": "orders",
        "Queues": ["order-processing", "order-processing-2"]
      }
    ],
    "QueueClients": ["order-results"]
  }
}
```

Register the storage services with an Azure credential:

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.StorageAccount.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterAzureStorage(new DefaultAzureCredential());
builder.Services.AddScoped<PersistentService>();
```

Registration adds a scoped `BlobServiceClient`, one keyed `QueuesService` for each `Queues` entry, and one keyed `QueueClient` for each name in `QueueClients`. Configure `BlobsEndpoint` for blob use and `QueuesEndpoint` when either queue collection is populated. `QueueClients` defaults to an empty collection, and `QueueMessagesEncodeBase64` applies to every queue client.

## Blob persistence and SAS URIs

Inject `PersistentService` to create and list containers; upload, download, copy, and delete blobs; update metadata; and list blobs by prefix:

```csharp
await persistentService.SaveOnBlobAsync(
    "documents",
    contentStream,
    "invoices/2026-001.pdf",
    cancellationToken: cancellationToken);

var readUri = await persistentService.RetreiveBlobUriAsync(
    "documents",
    "invoices/2026-001.pdf",
    cancellationToken);
```

The public API spells `RetreiveBlobUriAsync` as shown. It returns a read-only SAS URI valid for five hours. With token credentials, the service requests a [user delegation key](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-user-delegation-sas-create-dotnet); the identity therefore needs permission to generate that key in addition to the required blob data permissions.

## Queues

Resolve a grouped sender by its `Id`. `QueuesService` sends one message to a random configured queue, or distributes a list round-robin:

```csharp
public sealed class OrdersPublisher(
    [FromKeyedServices("orders")] QueuesService queues)
{
    public Task PublishAsync(
        OrderCreated message,
        CancellationToken cancellationToken)
    {
        return queues.SendMessageAsync(
            message,
            cancellationToken: cancellationToken);
    }
}
```

Resolve a direct queue client with `[FromKeyedServices("order-results")] QueueClient`.

## Managed identity permissions

`DefaultAzureCredential` uses managed identity in Azure. Assign only the needed [Azure Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/assign-azure-role-data-access) and [Azure Queue Storage](https://learn.microsoft.com/en-us/azure/storage/queues/assign-azure-role-data-access) data roles. Role changes can take time to propagate.

## Resources

- [Getting started](../getting-started.md)
- [Source code](../../src/MDev.Dotnet.Azure.StorageAccount/)
- [Azure Storage documentation](https://learn.microsoft.com/en-us/azure/storage/)

[Previous: Azure Cosmos DB](azure-cosmosdb.md) · [Documentation home](../index.md)

# MDev.Dotnet.Azure.StorageAccount

`MDev.Dotnet.Azure.StorageAccount` provides injectable helpers for Azure Blob Storage and Queue Storage.

## Installation

```bash
dotnet add package MDev.Dotnet.Azure.StorageAccount
dotnet add package Azure.Identity
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
        "Queues": ["order-processing"]
      }
    ]
  }
}
```

Register the storage services with an Azure credential:

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.StorageAccount.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterAzureStorage(new DefaultAzureCredential());
```

Inject `PersistentService` for blob operations or a configured keyed queue client for queue operations. See the [source code](../../src/MDev.Dotnet.Azure.StorageAccount/) and [Azure Storage documentation](https://learn.microsoft.com/en-us/azure/storage/) for more detail.

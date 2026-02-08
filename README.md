# MDev.Dotnet

Welcome to **MDev.Dotnet** - a collection of production-ready helpers and tools designed to accelerate .NET web application development and simplify cloud service integration. This repository provides battle-tested libraries that streamline common development tasks and Azure PaaS service configurations.

## Table of Contents

- [Overview](#overview)
- [Quick Start](#quick-start)
- [Packages](#packages)
  - [MDev.Dotnet.AspNetCore](#mdevdotnetaspnetcore)
  - [MDev.Dotnet.Azure.ContainerApps](#mdevdotnetazurecontainerapps)
  - [MDev.Dotnet.Azure.CosmosDb](#mdevdotnetazurecosmosdb)
  - [MDev.Dotnet.Azure.StorageAccount](#mdevdotnetazurestorageaccount)
- [Additional Resources](#additional-resources)
- [Support / Contribute](#support--contribute)

---

## Overview

This repository contains four main packages that help you build robust .NET applications with Azure integration:

- **AspNetCore helpers** for standard web API configuration
- **Container Apps helpers** for OpenTelemetry, authentication, and async operations
- **CosmosDb helpers** for Entity Framework Core integration
- **Storage Account helpers** for blob and queue operations

Each package follows consistent patterns and integrates seamlessly with the .NET dependency injection system.

---

## Quick Start

1. Install the NuGet package you need:
   ```bash
   dotnet add package MDev.Dotnet.AspNetCore
   dotnet add package MDev.Dotnet.Azure.ContainerApps
   dotnet add package MDev.Dotnet.Azure.CosmosDb
   dotnet add package MDev.Dotnet.Azure.StorageAccount
   ```

2. Configure your services in `Program.cs`
3. Add required configuration sections to `appsettings.json`
4. Start using the helpers in your application

---

# Packages

## MDev.Dotnet.AspNetCore

### 🎯 Highlight

**Streamline ASP.NET Core web API configuration** with pre-configured controllers, routing, API versioning, and configuration management. This package eliminates boilerplate code and enforces best practices for web API development.

**Key Features:**
- ✅ Simplified controller registration with built-in logging for 400 errors
- ✅ Automatic API versioning support
- ✅ Configuration binding with type safety
- ✅ Lowercase URL routing
- ✅ OpenAPI/Swagger configuration
- ✅ Route prefix middleware
- ✅ Synchronous IO support when needed

### 📚 Explain

This package provides extension methods that wrap common ASP.NET Core configurations into single-line registrations. It helps you:

1. **Controller Registration** - Registers controllers with automatic model validation logging
2. **Configuration Management** - Binds configuration sections to strongly-typed objects
3. **API Versioning** - Adds versioning support to your APIs
4. **OpenAPI Support** - Configures OpenAPI documentation with customizable options
5. **Route Prefixing** - Adds global route prefixes to your API endpoints

**Main Extension Methods:**
- `RegisterControllers<T>()` - Registers MVC controllers with logging and versioning
- `RegisterConfiguration()` - Loads appsettings.json with environment-specific overrides
- `BindConfiguration<T>()` - Binds configuration sections to POCOs
- `RegisterOpenApi()` - Configures OpenAPI/Swagger documentation
- `UseRoutePrefix()` - Adds a route prefix to all endpoints

### 🔗 Resources

- **NuGet Package:** [![NuGet](https://buildstats.info/nuget/MDev.Dotnet.AspNetCore?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.AspNetCore)
- **Wiki Documentation:** [MDev.Dotnet Wiki](https://github.com/mathieumack/MDev.Dotnet/wiki)
- **Source Code:** [src/MDev.Dotnet.AspNetCore](src/MDev.Dotnet.AspNetCore)

### ⚙️ Operation

#### Installation

```bash
dotnet add package MDev.Dotnet.AspNetCore
```

#### Basic Setup in Program.cs

```csharp
using MDev.Dotnet.AspNetCore.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register configuration
builder.RegisterConfiguration();

// Register controllers with API versioning
builder.RegisterControllers<Program>();

// Register OpenAPI
builder.RegisterOpenApi();

var app = builder.Build();

// Add route prefix (optional)
app.UseRoutePrefix("api");

app.MapControllers();
app.MapOpenApi();

app.Run();
```

#### Configuration Binding Example

```csharp
// Define your settings class
public class MyAppSettings
{
    public string ApiKey { get; set; }
    public int Timeout { get; set; }
}

// In Program.cs
builder.BindConfiguration<MyAppSettings>(out var settings, "MyAppSettings");

// Or inject via IOptions<T>
builder.BindConfiguration<MyAppSettings>("MyAppSettings");

// In your service
public class MyService
{
    private readonly MyAppSettings _settings;
    
    public MyService(IOptions<MyAppSettings> settings)
    {
        _settings = settings.Value;
    }
}
```

#### appsettings.json Example

```json
{
  "MyAppSettings": {
    "ApiKey": "your-api-key",
    "Timeout": 30
  }
}
```

#### Advanced Controller Registration

```csharp
// With synchronous IO support
builder.RegisterControllers<Program>(
    allowSynchronousIO: true,
    mvcOptions: options => {
        // Add custom MVC options
        options.MaxModelValidationErrors = 50;
    }
);
```

#### OpenAPI Configuration

```csharp
// Force HTTPS and hide server URLs
builder.RegisterOpenApi(
    forceHttpsServers: true,
    includeServerUrls: false
);
```

---

## MDev.Dotnet.Azure.ContainerApps

### 🎯 Highlight

**Supercharge your Azure Container Apps** with integrated OpenTelemetry, authentication helpers, and Dapr-based async operations. This package simplifies container app configuration and monitoring.

**Key Features:**
- ✅ OpenTelemetry integration with Azure Monitor or custom OTLP endpoints
- ✅ Container Apps authentication helpers
- ✅ Dapr pub/sub async operation handlers
- ✅ Built-in metrics, tracing, and logging
- ✅ Configurable status code handling
- ✅ Dapr API token validation

### 📚 Explain

This package builds on top of `MDev.Dotnet.AspNetCore` and adds Azure Container Apps-specific functionality:

1. **OpenTelemetry Configuration** - Automatic setup for Azure Monitor or Dynatrace with customizable metrics
2. **Authentication Helpers** - Easy access to Container Apps authentication context
3. **Async Operations** - Base controller for handling Dapr pub/sub messages
4. **Dapr Integration** - Attribute-based API token validation

**Main Components:**
- `RegisterOpenTelemetry()` - Configures OpenTelemetry with Azure Monitor or OTLP
- `DaprHandlerController` - Base controller for Dapr pub/sub handlers
- `IAsyncOperationRequestMessageHandler` - Interface for async operation handlers
- `RequireDaprApiTokenAttribute` - Validates Dapr API tokens
- `MsClientPrincipal` - Access Container Apps authentication context

### 🔗 Resources

- **NuGet Package:** [![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.ContainerApps?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.ContainerApps)
- **Azure Container Apps Docs:** [Microsoft Learn](https://learn.microsoft.com/en-us/azure/container-apps/)
- **OpenTelemetry Docs:** [Azure Monitor Integration](https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents)
- **Source Code:** [src/MDev.Dotnet.Azure.ContainerApps](src/MDev.Dotnet.Azure.ContainerApps)

### ⚙️ Operation

#### Installation

```bash
dotnet add package MDev.Dotnet.Azure.ContainerApps
```

#### OpenTelemetry Setup

```csharp
using MDev.Dotnet.AspNetCore.OpenTelemetry.Apis.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register OpenTelemetry with custom meters
var customMeters = new List<string>
{
    "MyApp.Orders",
    "MyApp.Payments"
};

builder.RegisterOpenTelemetry(customMeters);

var app = builder.Build();
app.Run();
```

#### appsettings.json for OpenTelemetry

```json
{
  "OpenTelemetry": {
    "ServiceType": "AppInsights",
    "IgnoreErrorStatusCode": [404, 401]
  }
}
```

For custom OTLP endpoints (e.g., Dynatrace):
```json
{
  "OpenTelemetry": {
    "ServiceType": "Custom"
  },
  "OTEL_ENDPOINT": "https://your-otlp-endpoint.com",
  "OTEL_ENDPOINT_AUTH": "Api-Token your-token",
  "CONTAINER_APP_NAME": "myapp",
  "CONTAINER_APP_REPLICA_NAME": "myapp-revision-1"
}
```

#### Implementing Async Operations with Dapr

**Step 1: Create a Message Handler**

```csharp
using MDev.Dotnet.AspNetCore.AsyncOperations.Abstracts;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;

public class OrderProcessingHandler : IAsyncOperationRequestMessageHandler
{
    public string OperationType => "ProcessOrder";
    
    public async Task<object> HandleAsync(
        AsyncOperationRequestMessage message, 
        CancellationToken cancellationToken)
    {
        // Process your order
        var orderId = message.Data["orderId"].ToString();
        
        // Your business logic here
        await ProcessOrderAsync(orderId);
        
        return new { Success = true, OrderId = orderId };
    }
}
```

**Step 2: Register the Handler**

```csharp
builder.Services.AddScoped<IAsyncOperationRequestMessageHandler, OrderProcessingHandler>();
builder.Services.AddScoped<AsyncOperationRequestsService>();
```

**Step 3: Create Your Dapr Controller**

```csharp
using MDev.Dotnet.AspNetCore.AsyncOperations.Controllers.v1;
using MDev.Dotnet.Dapr.Attributes;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/dapr")]
public class MyDaprController : DaprHandlerController
{
    public MyDaprController(AsyncOperationRequestsService service) 
        : base(service)
    {
    }
    
    [HttpPost("orders")]
    [RequireDaprApiToken]
    public override Task<IActionResult> OperationRequestAsync(
        [FromBody] AsyncOperationRequestMessage item,
        CancellationToken cancellationToken)
    {
        return base.OperationRequestAsync(item, cancellationToken);
    }
}
```

#### Accessing Container Apps Authentication

```csharp
using MDev.Dotnet.Azure.ContainerApps.Authentication.Extensions;

public class MyController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MyController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    [HttpGet]
    public IActionResult GetUserInfo()
    {
        var principal = _httpContextAccessor.GetMsClientPrincipal();
        
        if (principal != null)
        {
            return Ok(new 
            { 
                UserId = principal.UserId,
                UserName = principal.UserDetails,
                Roles = principal.UserRoles
            });
        }
        
        return Unauthorized();
    }
}
```

---

## MDev.Dotnet.Azure.CosmosDb

### 🎯 Highlight

**Simplify Azure CosmosDb integration** with Entity Framework Core. This package provides a one-line registration for CosmosDb contexts with managed identity support.

**Key Features:**
- ✅ Simple DbContext registration for CosmosDb
- ✅ Managed Identity (TokenCredential) support
- ✅ Configuration-based setup
- ✅ Scoped DbContext lifetime management

### 📚 Explain

This package simplifies the integration of Azure CosmosDb with Entity Framework Core by:

1. **DbContext Registration** - Automatically configures EF Core with CosmosDb provider
2. **Credential Management** - Uses Azure TokenCredential for secure authentication
3. **Configuration Binding** - Reads CosmosDb settings from appsettings.json

**Main Extension Method:**
- `RegisterCosmosDb<T>()` - Registers a DbContext with CosmosDb provider

### 🔗 Resources

- **NuGet Package:** [![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.CosmosDb?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.CosmosDb)
- **CosmosDb Docs:** [Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/)
- **EF Core CosmosDb:** [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/providers/cosmos/)
- **Source Code:** [src/MDev.Dotnet.Azure.CosmosDb](src/MDev.Dotnet.Azure.CosmosDb)

### ⚙️ Operation

#### Installation

```bash
dotnet add package MDev.Dotnet.Azure.CosmosDb
dotnet add package Azure.Identity
```

#### Define Your DbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .ToContainer("Products")
            .HasPartitionKey(p => p.Category);
            
        modelBuilder.Entity<Order>()
            .ToContainer("Orders")
            .HasPartitionKey(o => o.CustomerId);
    }
}

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public List<string> ProductIds { get; set; }
    public decimal Total { get; set; }
}
```

#### Register CosmosDb in Program.cs

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.CosmosDb.Startup;

var builder = WebApplication.CreateBuilder(args);

// Use DefaultAzureCredential for managed identity
var credential = new DefaultAzureCredential();

// Or use specific credential type
// var credential = new ManagedIdentityCredential();
// var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

builder.RegisterCosmosDb<MyAppDbContext>(credential);

var app = builder.Build();
app.Run();
```

#### appsettings.json Configuration

```json
{
  "CosmosDb": {
    "Endpoint": "https://your-account.documents.azure.com:443/",
    "DatabaseName": "MyAppDatabase"
  }
}
```

#### Using the DbContext

```csharp
public class ProductService
{
    private readonly MyAppDbContext _dbContext;
    
    public ProductService(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Product> GetProductAsync(string id, string category)
    {
        return await _dbContext.Products
            .WithPartitionKey(category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task CreateProductAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _dbContext.Products
            .WithPartitionKey(category)
            .ToListAsync();
    }
}
```

#### EnsureCreated (Development Only)

```csharp
// In Program.cs for development environments
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}
```

---

## MDev.Dotnet.Azure.StorageAccount

### 🎯 Highlight

**Streamline Azure Storage operations** with ready-to-use helpers for blob storage and queue management. Includes SAS token generation, blob operations, and queue client management.

**Key Features:**
- ✅ Blob upload/download with metadata support
- ✅ SAS token generation for secure blob access
- ✅ Container management (create, delete, list)
- ✅ Multiple queue client support with key-based registration
- ✅ Managed Identity (TokenCredential) support
- ✅ Base64 encoding support for queue messages

### 📚 Explain

This package provides two main helpers for Azure Storage operations:

1. **PersistentService** - Comprehensive blob storage operations
   - Upload/download blobs
   - Generate SAS URIs for secure access
   - Container management
   - Blob copying and deletion
   - Metadata support

2. **QueuesService** - Queue client management
   - Multiple queue support per application
   - Key-based queue client registration
   - Base64 message encoding option

**Main Components:**
- `RegisterAzureStorage()` - Registers BlobServiceClient and QueueClients
- `PersistentService` - Scoped service for blob operations
- `QueuesService` - Keyed service for queue operations

### 🔗 Resources

- **NuGet Package:** [![NuGet](https://buildstats.info/nuget/MDev.Dotnet.Azure.StorageAccount?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.StorageAccount)
- **Azure Storage Docs:** [Microsoft Learn](https://learn.microsoft.com/en-us/azure/storage/)
- **Blob Storage:** [Blob Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- **Queue Storage:** [Queue Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/queues/)
- **Source Code:** [src/MDev.Dotnet.Azure.StorageAccount](src/MDev.Dotnet.Azure.StorageAccount)

### ⚙️ Operation

#### Installation

```bash
dotnet add package MDev.Dotnet.Azure.StorageAccount
dotnet add package Azure.Identity
```

#### Register Storage Account in Program.cs

```csharp
using Azure.Identity;
using MDev.Dotnet.Azure.StorageAccount.Startup;

var builder = WebApplication.CreateBuilder(args);

// Use DefaultAzureCredential for managed identity
var credential = new DefaultAzureCredential();

builder.RegisterAzureStorage(credential);

var app = builder.Build();
app.Run();
```

#### appsettings.json Configuration

```json
{
  "StorageAccount": {
    "BlobsEndpoint": "https://yourstorageaccount.blob.core.windows.net",
    "QueuesEndpoint": "https://yourstorageaccount.queue.core.windows.net",
    "QueueMessagesEncodeBase64": false,
    "Queues": [
      {
        "Id": "orders",
        "Queues": ["order-processing", "order-notifications"]
      },
      {
        "Id": "reports",
        "Queues": ["report-generation"]
      }
    ]
  }
}
```

#### Using PersistentService for Blob Operations

```csharp
using MDev.Dotnet.Azure.StorageAccount.Helpers;

public class FileStorageService
{
    private readonly PersistentService _persistentService;
    private readonly ILogger<FileStorageService> _logger;
    
    public FileStorageService(
        PersistentService persistentService,
        ILogger<FileStorageService> logger)
    {
        _persistentService = persistentService;
        _logger = logger;
    }
    
    // Upload a file
    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName,
        string containerName = "documents")
    {
        var metadata = new Dictionary<string, string>
        {
            { "UploadedBy", "user@example.com" },
            { "UploadDate", DateTime.UtcNow.ToString("O") }
        };
        
        await _persistentService.SaveOnBlobAsync(
            containerName, 
            fileStream, 
            fileName,
            metadata);
            
        return await _persistentService.RetreiveBlobUriAsync(
            containerName, 
            fileName);
    }
    
    // Download a file
    public async Task<Stream> DownloadFileAsync(
        string containerName, 
        string fileName)
    {
        return await _persistentService.DownloadBlobContentAsync(
            containerName, 
            fileName);
    }
    
    // Generate secure access URL
    public async Task<string> GetSecureUrlAsync(
        string containerName, 
        string fileName)
    {
        return await _persistentService.RetreiveBlobUriAsync(
            containerName, 
            fileName);
    }
    
    // Delete a file
    public async Task DeleteFileAsync(
        string containerName, 
        string fileName)
    {
        await _persistentService.DeleteBlobIfExistsAsync(
            containerName, 
            fileName);
    }
    
    // List all files with prefix
    public async Task<List<string>> ListFilesAsync(
        string containerName, 
        string prefix)
    {
        return await _persistentService.RetreiveBlobsUriAsync(
            containerName, 
            prefix);
    }
    
    // Copy a file
    public async Task CopyFileAsync(
        string sourceContainer,
        string sourceFileName,
        string destContainer,
        string destFileName)
    {
        await _persistentService.CopyBlocFileAsync(
            sourceContainer,
            sourceFileName,
            destContainer,
            destFileName);
    }
}
```

#### Using QueuesService for Queue Operations

```csharp
using MDev.Dotnet.Azure.StorageAccount.Helpers;

public class OrderProcessingService
{
    private readonly QueuesService _ordersQueue;
    private readonly ILogger<OrderProcessingService> _logger;
    
    // Inject using keyed service
    public OrderProcessingService(
        [FromKeyedServices("orders")] QueuesService ordersQueue,
        ILogger<OrderProcessingService> logger)
    {
        _ordersQueue = ordersQueue;
        _logger = logger;
    }
    
    public async Task QueueOrderAsync(string orderId)
    {
        var message = new OrderMessage
        {
            OrderId = orderId,
            Timestamp = DateTime.UtcNow
        };
        
        var json = JsonSerializer.Serialize(message);
        
        // Send to first queue in the "orders" group
        await _ordersQueue.Clients[0].SendMessageAsync(json);
        
        _logger.LogInformation("Order {OrderId} queued", orderId);
    }
}

public class OrderMessage
{
    public string OrderId { get; set; }
    public DateTime Timestamp { get; set; }
}
```

#### Complete Example: File Upload API

```csharp
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly PersistentService _persistentService;
    
    public FilesController(PersistentService persistentService)
    {
        _persistentService = persistentService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");
            
        using var stream = file.OpenReadStream();
        
        var fileName = $"{Guid.NewGuid()}-{file.FileName}";
        var metadata = new Dictionary<string, string>
        {
            { "OriginalName", file.FileName },
            { "ContentType", file.ContentType },
            { "Size", file.Length.ToString() }
        };
        
        await _persistentService.SaveOnBlobAsync(
            "uploads",
            stream,
            fileName,
            metadata);
            
        var url = await _persistentService.RetreiveBlobUriAsync(
            "uploads",
            fileName);
            
        return Ok(new { Url = url, FileName = fileName });
    }
    
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var stream = await _persistentService.DownloadBlobContentAsync(
            "uploads",
            fileName);
            
        if (stream == Stream.Null)
            return NotFound();
            
        return File(stream, "application/octet-stream", fileName);
    }
    
    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        await _persistentService.DeleteBlobIfExistsAsync(
            "uploads",
            fileName);
            
        return NoContent();
    }
}
```

---

## Additional Resources

- **Wiki Documentation:** [Complete documentation on the Wiki](https://github.com/mathieumack/MDev.Dotnet/wiki)
- **Code Documentation:** All public APIs include XML documentation comments
- **Azure Documentation:** [Microsoft Learn](https://learn.microsoft.com/en-us/azure/)
- **Sample Projects:** Check the source code for inline examples and patterns

---

# Support / Contribute

> If you have any question, problem or suggestion, create an issue or fork the project and create a Pull Request.

## Build Status

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MDev.Dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MDev.Dotnet)
[![.NET](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml)

## License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

---

**Made with ❤️ for the .NET Community**

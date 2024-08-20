# MDev.Dotnet
This repository contains some helpers and tools that help you to configure quickly a new Web Application based on .net.

It contains some helpers and accelerators for your project. And dependending on infrastructure, some helpers to configure quickly access to PaaS services on different Cloud providers (Microsoft Azure yet).

This page contains some suick resume of each package. You can check on the [Wiki](https://github.com/mathieumack/MDev.Dotnet/wiki) for more documentation. Also, documentation on code can be helpfull.

# Packages
## MDev.Dotnet.AspNetCore
This package let you to configure standard services on a .net web application :
- Application configuration
- Controllers
- Support for synchronous IO operations
- Routing
- API Versioning 

## PaaS Services - Microsoft Azure
### MDev.Dotnet.Azure.ContainerApps
This package let you to confgure some services when your application is deployed with docker on an Azure Container Environment as Container Apps.

It helps you to configure:
- Authentication when using Container App Authentication middleware
- Configure OpenTelemetry by using Azure Monitor or Dynatrace
- An async controller that let you to use Dapr as pubsub service from an Azure storage queue or an Azure servicebus

### MDev.Dotnet.Azure.CosmosDb
This package contains a helper for Entity Framework Core that configure CosmosDb as provider.

### MDev.Dotnet.Azure.StorageAccount
This package contains a helper for interactions with a storage account:
- Save blobs
- Generate blob uri with Sas tokens

And more packages will come in few weeks.

# IC
## MDev.Dotnet
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MDev.Dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MDev.Dotnet)
[![.NET](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MDev.Dotnet/actions/workflows/ci.yml)

## Nuget packages
MDev.Dotnet.AspNetCore - [![NuGet package](https://buildstats.info/nuget/MDev.Dotnet.AspNetCore?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.AspNetCore)

MDev.Dotnet.Azure.ContainerApps - [![NuGet package](https://buildstats.info/nuget/MDev.Dotnet.Azure.ContainerApps?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.AspNetCore)

MDev.Dotnet.Azure.CosmosDb - [![NuGet package](https://buildstats.info/nuget/MDev.Dotnet.AspNetCore?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.CosmosDb)

MDev.Dotnet.Azure.StorageAccount - [![NuGet package](https://buildstats.info/nuget/MDev.Dotnet.Azure.StorageAccount?includePreReleases=true)](https://nuget.org/packages/MDev.Dotnet.Azure.StorageAccount)


# Support / Contribute
> If you have any question, problem or suggestion, create an issue or fork the project and create a Pull Request.

using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs;

/// <summary>
/// Resolves credentials for the managed identity assigned to a Container Apps Job.
/// </summary>
public static class ManagedIdentityExtensions
{
    /// <summary>
    /// Gets the system-assigned identity, or the user-assigned identity selected by AZURE_CLIENT_ID.
    /// </summary>
    public static TokenCredential GetManagedIdentity(this IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var clientId = configuration["AZURE_CLIENT_ID"];
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
        }

        if (!Guid.TryParse(clientId, out _))
        {
            throw new InvalidOperationException(
                "AZURE_CLIENT_ID must be the client ID of a user-assigned managed identity.");
        }

        return new ManagedIdentityCredential(
            ManagedIdentityId.FromUserAssignedClientId(clientId));
    }

    /// <summary>
    /// Gets the managed identity configured for the current workload.
    /// </summary>
    public static TokenCredential GetManagedIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        return configuration.GetManagedIdentity();
    }
}

﻿using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceCollectionHelpers.AssemblyFinder.Attributes;
using System.Globalization;
using System.Text;

namespace MDev.Dotnet.Azure.StorageAccount.Helpers;

[ServiceRegister(Scope = ServiceLifetime.Scoped)]
public class PersistentService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<PersistentService> _logger;

    public PersistentService(BlobServiceClient blobServiceClient, ILogger<PersistentService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Delete a container blob if it exists.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _blobServiceClient.DeleteBlobContainerAsync(containerName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during blob container deletion for container '{containerName}'.", containerName);
            throw;
        }
    }

    /// <summary>
    /// Crée un container blob s'il n'existe pas déjà.
    /// </summary>
    /// <param name="containerName">Nom du nouveau container</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si le container a été créé, False s'il existait déjà</returns>
    public async Task<bool> CreateContainerIfNotExistsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during container creation for container '{containerName}'.", containerName);
            throw;
        }
    }

    /// <summary>
    /// Retreive a blob client from the storage account for a specified container and file name
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public BlobClient GetBlobAsync(string containerName, string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(fileName);

        return blobClient;
    }

    /// <summary>
    /// Delete a blob from the storage
    /// </summary>
    /// <param name="container"></param>
    /// <param name="blobName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteBlobAsync(string container, string blobName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Delete {container}", container);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        _ = await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Try to delete a blob from the storage
    /// </summary>
    /// <param name="container"></param>
    /// <param name="blobName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteBlobIfExistsAsync(string container, string blobName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Delete {container}", container);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retreive a public uri with a sas token for a file in the storage account
    /// </summary>
    /// <param name="container"></param>
    /// <param name="blobName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> RetreiveBlobUriAsync(string container, string blobName, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
        _ = await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        var uri = await GetBlobClientUriWithSasAsync(blobClient, cancellationToken: cancellationToken);
        return uri;
    }

    public async Task<List<string>> RetreiveBlobsUriAsync(string container, string prefix, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
        _ = await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var result = new List<string>();

        await foreach (var blobItem in blobContainerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
            var uri = await GetBlobClientUriWithSasAsync(blobClient, cancellationToken: cancellationToken);
            result.Add(uri.ToString());
        }

        return result;
    }

    /// <summary>
    /// Allow to retreive a blob stream from container and blobName
    /// </summary>
    /// <param name="container"></param>
    /// <param name="blobName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Document stream, null instead</returns>
    public async Task<Stream> DownloadBlobContentAsync(string container, string blobName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Download {container}", container);
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        if (await blobClient.ExistsAsync())
        {
            var response = await blobClient.DownloadAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }
        return Stream.Null;
    }

    /// <summary>
    /// Delete multiple blob at same time
    /// </summary>
    /// <param name="container"></param>
    /// <param name="prefix"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteBlobsAsync(string container, string prefix, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Delete {container}", container);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);

        if (string.IsNullOrWhiteSpace(prefix)) // We want to delete the container.
            _ = await blobContainerClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        else
        {
            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
            {
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            }
        }
    }

    public async Task SaveOnBlobAsync(string container,
                                                Stream blobContent,
                                                string fileName,
                                                Dictionary<string, string> metadatas = null,
                                                CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Save {FileName} to blob", fileName);

        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
        _ = await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Add metadatas :
        blobContent.Position = 0;
        await blobClient.UploadAsync(blobContent, true, cancellationToken);

        if (metadatas != null)
        {
            // Remove diacritics by default on metadatas
            foreach (var metadata in metadatas)
                metadatas[metadata.Key] = RemoveDiacritics(metadata.Value);
            blobClient.SetMetadata(metadatas, cancellationToken: cancellationToken);
        }

        _logger.LogDebug("File {FileName} saved", fileName);
    }

    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public async Task SaveOnBlobAsync(string container, string blobContent, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(blobContent));
        await SaveOnBlobAsync(container, stream, fileName, null, cancellationToken);
    }

    private async Task<string> GetBlobClientUriWithSasAsync(BlobClient blobClient, CancellationToken cancellationToken = default)
    {
        var expiration = DateTimeOffset.UtcNow.AddHours(5);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddHours(-10),
            ExpiresOn = expiration
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri uri;
        if (_blobServiceClient.CanGenerateAccountSasUri)
        {
            uri = blobClient.GenerateSasUri(sasBuilder);
        }
        else
        {
            var delegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(sasBuilder.StartsOn, sasBuilder.ExpiresOn, cancellationToken);

            _logger.LogDebug("Generate SASToken Uri valid until {Expiration}", expiration);

            uri = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(delegationKey, _blobServiceClient.AccountName)
            }.ToUri();
        }

        return uri.ToString();
    }

    /// <summary>
    /// Copy bloc file async from a source container to a destination container
    /// </summary>
    /// <param name="sourceContainer"></param>
    /// <param name="sourceStorageFileName"></param>
    /// <param name="destContainer"></param>
    /// <param name="destFilename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CopyBlocFileAsync(string sourceContainer, string sourceStorageFileName, string destContainer, string destFilename, CancellationToken cancellationToken = default)
    {
        var sourceBlobContainerClient = _blobServiceClient.GetBlobContainerClient(sourceContainer);
        var sourceBlobClient = sourceBlobContainerClient.GetBlobClient(sourceStorageFileName);

        var destBlobContainerClient = _blobServiceClient.GetBlobContainerClient(destContainer);
        var destBlobClient = destBlobContainerClient.GetBlobClient(destFilename);

        var response = await sourceBlobClient.DownloadAsync(cancellationToken: cancellationToken);
        await destBlobClient.UploadAsync(response.Value.Content, true, cancellationToken);
    }
}

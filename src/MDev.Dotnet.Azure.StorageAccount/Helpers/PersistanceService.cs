﻿using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceCollectionHelpers.AssemblyFinder.Attributes;
using System.Globalization;
using System.Text;

namespace MDev.Dotnet.Azure.StorageAccount.Helpers;

[ServiceRegister(Scope = ServiceLifetime.Scoped)]
public class PersistanceService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<PersistanceService> _logger;

    public PersistanceService(BlobServiceClient blobServiceClient,
                                ILogger<PersistanceService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Retreive a public uri with a sas token for a file in the storage account
    /// </summary>
    /// <param name="container"></param>
    /// <param name="documentName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> RetreiveBlobUri(string container, string documentName, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient
            .GetBlobContainerClient(container);
        _ = await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(documentName);
        var uri = await GetBlobClientUriWithSas(blobClient, cancellationToken: cancellationToken);
        return uri;
    }

    public async Task<List<string>> RetreiveBlobsUri(string container, string prefix, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient
            .GetBlobContainerClient(container);
        _ = await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var result = new List<string>();

        await foreach(var blobItem in blobContainerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
            var uri = await GetBlobClientUriWithSas(blobClient, cancellationToken: cancellationToken);
            result.Add(uri.ToString());
        }

        return result;
    }

    /// <summary>
    /// Delete a blob from the storage :
    /// </summary>
    /// <param name="container"></param>
    /// <param name="blobName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteBlob(string container, string blobName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Delete {container}", container);

        var blobContainerClient = _blobServiceClient
            .GetBlobContainerClient(container);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Delete multiple blob at same time
    /// </summary>
    /// <param name="container"></param>
    /// <param name="prefix"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteBlobs(string container, string prefix, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Delete {container}", container);

        var blobContainerClient = _blobServiceClient
            .GetBlobContainerClient(container);

        if(string.IsNullOrWhiteSpace(prefix)) // We want to delete the container.
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

    public async Task<string> SaveOnBlobAsync(string container, 
                                                Stream blobContent, 
                                                string fileName,
                                                Dictionary<string, string> metadatas = null, 
                                                CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Save {FileName} to blob", fileName);

        var blobContainerClient = _blobServiceClient
            .GetBlobContainerClient(container);
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

        var uri = await GetBlobClientUriWithSas(blobClient, cancellationToken: cancellationToken);

        return uri.ToString();
    }

    public string RemoveDiacritics(string text)
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

    public async Task<string> SaveOnBlobAsync(string container, string blobContent, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(blobContent));
        return await SaveOnBlobAsync(container, stream, fileName, null, cancellationToken);
    }

    private async Task<string> GetBlobClientUriWithSas(BlobClient blobClient, CancellationToken cancellationToken = default)
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
}

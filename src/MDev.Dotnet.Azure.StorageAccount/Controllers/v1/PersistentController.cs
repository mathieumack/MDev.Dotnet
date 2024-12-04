using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MDev.Dotnet.Azure.StorageAccount.DTOs;
using MDev.Dotnet.Azure.StorageAccount.Helpers;
using Asp.Versioning;
using System.Net;

namespace MDev.Dotnet.Azure.StorageAccount.Controllers.v1;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/persistent")]
public class PersistentController : ControllerBase
{
    private readonly PersistentService _persistanceService;

    public PersistentController(PersistentService persistanceService)
    {
        _persistanceService = persistanceService;
    }

    /// <summary>
    /// Find all file with a name that starts with a specific prefix.
    /// Returns a list of file uris that all read
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpGet("{container}/{prefix}/uri")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<IActionResult> GetFileReferencesAsync(string container, string prefix)
    {
        var uris = await _persistanceService.RetreiveBlobsUriAsync(container, prefix);
        return Ok(uris);
    }

    /// <summary>
    /// Get the document as markdown format
    /// </summary>
    /// <param name="id">Document id</param>
    /// <param name="format">Format of export. Supported values : markdown, raw, text</param>
    [HttpGet("{container}/{blobName}/download")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ExportBlobAsync(string container, string blobName, [FromQuery]string documentName = null, CancellationToken cancellationToken)
    {
        // Read file :
        var blob = await _persistanceService.DownloadBlobContent(container, blobName, cancellationToken);
        if(blob == Stream.Null)
            return NotFound();

        return File(blob, "application/octet-stream", (string.IsNullOrWhiteSpace(documentName) ? blobName : documentName));
    }

    /// <summary>
    /// Allow to store a file
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpPost("{container}")]
    [ProducesResponseType(typeof(StoreFileDto), 200)]
    public async Task<IActionResult> SaveStreamOnBlobContainerAsync(string container, [FromForm] IFormFile form)
    {
        var uri = await _persistanceService.SaveOnBlobAsync(container, form.OpenReadStream(), form.FileName, null, HttpContext.RequestAborted);
        return Ok(new StoreFileDto(new Uri(uri)));
    }

    /// <summary>
    /// Delete a blob on th storage account
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpDelete("{container}/{prefix}")]
    [ProducesResponseType(typeof(StoreFileDto), 200)]
    public async Task<IActionResult> DeleteStreamOnBlobContainerAsync(string container, string prefix)
    {
        await _persistanceService.DeleteBlobsAsync(container, prefix);
        return Ok();
    }
}

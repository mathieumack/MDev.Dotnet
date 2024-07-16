using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MDev.Dotnet.Azure.StorageAccount.DTOs;
using MDev.Dotnet.Azure.StorageAccount.Helpers;

namespace MDev.Dotnet.Azure.StorageAccount.Controllers.v1;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/persistance")]
public class PersistanceController : ControllerBase
{
    private readonly PersistanceService _persistanceService;

    public PersistanceController(PersistanceService persistanceService)
    {
        _persistanceService = persistanceService;
    }

    /// <summary>
    /// Find all file with a name that starts with a specific prefix.
    /// Returns a list of file uris that all read
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpGet("{container}/{prefix}")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<IActionResult> GetFileReferences(string container, string prefix)
    {
        var uris = await _persistanceService.RetreiveBlobsUri(container, prefix);
        return Ok(uris);
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
    /// Allow to store a file
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [HttpDelete("{container}/{prefix}")]
    [ProducesResponseType(typeof(StoreFileDto), 200)]
    public async Task<IActionResult> DeleteStreamOnBlobContainerAsync(string container, string prefix)
    {
        await _persistanceService.DeleteBlobs(container, prefix);
        return Ok();
    }
}

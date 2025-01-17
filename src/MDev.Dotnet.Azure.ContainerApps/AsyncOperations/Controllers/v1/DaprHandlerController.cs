using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;
using MDev.Dotnet.AspNetCore.AsyncOperations.Services.Handlers;
using Asp.Versioning;

namespace MDev.Dotnet.AspNetCore.AsyncOperations.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/v{version:apiVersion}/daprhandler")]
public class DaprHandlerController : ControllerBase
{
    private readonly AsyncOperationRequestsService asyncOperationRequestsService;
    private readonly ILogger<DaprHandlerController> logger;

    public DaprHandlerController(AsyncOperationRequestsService asyncOperationRequestsService,
                                    ILogger<DaprHandlerController> logger)
    {
        this.asyncOperationRequestsService = asyncOperationRequestsService;
        this.logger = logger;
    }

    /// <summary>
    /// Called by the Dapr components when a new message request is sent
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost("operationrequest")]
    public async Task<IActionResult> OperationRequest([FromBody] AsyncOperationRequestMessage item, CancellationToken cancellationToken)
    {
        await asyncOperationRequestsService.HandleAsync(item, cancellationToken);
        return Ok();
    }
}
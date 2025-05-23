﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MDev.Dotnet.AspNetCore.AsyncOperations.Messages;
using MDev.Dotnet.AspNetCore.AsyncOperations.Services.Handlers;
using Asp.Versioning;

namespace MDev.Dotnet.AspNetCore.AsyncOperations.Controllers.v1;

[ApiExplorerSettings(IgnoreApi = true)]
public class DaprHandlerController : ControllerBase
{
    private readonly AsyncOperationRequestsService asyncOperationRequestsService;

    public DaprHandlerController(AsyncOperationRequestsService asyncOperationRequestsService)
    {
        this.asyncOperationRequestsService = asyncOperationRequestsService;
    }

    /// <summary>
    /// Called by the Dapr components when a new message request is sent
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual async Task<IActionResult> OperationRequestAsync([FromBody] AsyncOperationRequestMessage item, CancellationToken cancellationToken)
    {
        await asyncOperationRequestsService.HandleAsync(item, cancellationToken);
        return Ok();
    }
}
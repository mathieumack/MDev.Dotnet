using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace MDev.Dotnet.Azure.ContainerApps.Dapr.Attributes;

/// <summary>
/// Attribute to require the Dapr API token header (DAPR_API_TOKEN) for controller actions.
/// Returns 401 Unauthorized if the header is missing, empty, or does not match the configured value.
/// The expected token is read from the <c>APP_API_TOKEN</c> configuration key on every request,
/// which avoids stale-state issues during testing and when configuration is reloaded at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireDaprApiTokenAttribute : Attribute, IAuthorizationFilter
{
    private const string DaprApiTokenHeader = "dapr-api-token";
    private const string AppApiTokenConfigKey = "APP_API_TOKEN";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var expectedToken = config?[AppApiTokenConfigKey];

        if (string.IsNullOrWhiteSpace(expectedToken) ||
            !context.HttpContext.Request.Headers.TryGetValue(DaprApiTokenHeader, out var token) ||
            !string.Equals(token, expectedToken, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace MDev.Dotnet.Azure.ContainerApps.Dapr.Attributes;

/// <summary>
/// Attribute to require the Dapr API token header (DAPR_API_TOKEN) for controller actions.
/// Returns 401 Unauthorized if the header is missing, empty, or does not match the configured value.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireDaprApiTokenAttribute : Attribute, IAuthorizationFilter
{
    private const string DaprApiTokenHeader = "dapr-api-token";
    private const string AppApiTokenConfigKey = "APP_API_TOKEN";
    private static string? _expectedToken;
    private static bool _initialized = false;

    private static void EnsureTokenLoaded(IServiceProvider services)
    {
        if (_initialized) return;
        var config = services.GetService(typeof(IConfiguration)) as IConfiguration;
        _expectedToken = config?[AppApiTokenConfigKey];
        _initialized = true;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        EnsureTokenLoaded(context.HttpContext.RequestServices);
        if (string.IsNullOrWhiteSpace(_expectedToken) ||
            !context.HttpContext.Request.Headers.TryGetValue(DaprApiTokenHeader, out var token) ||
            !string.Equals(token, _expectedToken, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}

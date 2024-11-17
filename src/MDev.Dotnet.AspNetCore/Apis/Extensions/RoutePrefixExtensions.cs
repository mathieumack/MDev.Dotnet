using MDev.Dotnet.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MDev.Dotnet.AspNetCore.Apis.Extensions;

public static class RoutePrefixExtensions
{
    /// <summary>
    /// Add a route prefix to the application.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="routePrefix">Route.</param>
    /// <example>/api</example>
    /// <example>/core</example>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static WebApplication AddRoutesPrefix(this WebApplication application, string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(routePrefix))
            throw new ArgumentNullException(nameof(routePrefix));

        var pathRoute = routePrefix.StartsWith("/") ? routePrefix : $"/{routePrefix}";
        application.UseMiddleware<RoutePrefixMiddleware>(pathRoute);
        application.UsePathBase(new PathString(pathRoute));
        
        return application;
    }
}

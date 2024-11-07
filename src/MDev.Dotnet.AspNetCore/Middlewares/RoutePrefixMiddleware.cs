using Microsoft.AspNetCore.Http;

namespace MDev.Dotnet.AspNetCore.Middlewares;

internal class RoutePrefixMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _routePrefix;

    public RoutePrefixMiddleware(RequestDelegate next, string routePrefix)
    {
        _next = next;
        _routePrefix = routePrefix;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.PathBase = new PathString(_routePrefix);
        await _next(context);
    }
}

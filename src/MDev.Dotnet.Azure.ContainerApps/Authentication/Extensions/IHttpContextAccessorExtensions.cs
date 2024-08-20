using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MDev.Dotnet.Azure.ContainerApps.Authentication.Extensions;

public static class IHttpContextAccessorExtensions
{
    public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL-ID");
    }

    public static string GetUserFullName(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL-NAME");
    }

    public static async Task<IEnumerable<UserClaim>> GetClaims(this IHttpContextAccessor httpContextAccessor)
    {
        var token = httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL");

        var decodedBytes = Convert.FromBase64String(token);
        using var memoryStream = new MemoryStream(decodedBytes);
        var clientPrincipal = await JsonSerializer.DeserializeAsync<MsClientPrincipal>(memoryStream);

        return clientPrincipal.Claims;
    }

    public static string GetUserValue(this IHttpContextAccessor httpContextAccessor, string headerName)
    {
        var userPrincipalId = httpContextAccessor.HttpContext.Request.Headers[headerName].ToString();
        return userPrincipalId;
    }

    public static void LogHeaders(this IHttpContextAccessor contextAccessor, ILogger logger)
    {
        foreach (var header in contextAccessor.HttpContext.Request.Headers)
        {
            logger.LogInformation("{HeaderName}: {HeaderValue}", header.Key, header.Value);
        }
    }
}

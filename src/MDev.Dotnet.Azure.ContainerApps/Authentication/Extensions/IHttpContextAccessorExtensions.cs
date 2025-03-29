using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;

namespace MDev.Dotnet.Azure.ContainerApps.Authentication.Extensions;

public static class IHttpContextAccessorExtensions
{
    public static string GetUserId(this IHttpContextAccessor httpContextAccessor, bool decode = false)
    {
        return httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL-ID", decode);
    }

    /// <summary>
    /// Get the user full name based on header X-MS-CLIENT-PRINCIPAL-NAME
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="decode">Indicate if the value must be uri decoded asvalue is encoded by the the authentication middleware</param>
    /// <returns></returns>
    public static string GetUserFullName(this IHttpContextAccessor httpContextAccessor, bool decode = false)
    {
        return httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL-NAME", decode);
    }

    public static async Task<IEnumerable<UserClaim>> GetClaims(this IHttpContextAccessor httpContextAccessor)
    {
        var token = httpContextAccessor.GetUserValue("X-MS-CLIENT-PRINCIPAL");

        var decodedBytes = Convert.FromBase64String(token);
        using var memoryStream = new MemoryStream(decodedBytes);
        var clientPrincipal = await JsonSerializer.DeserializeAsync<MsClientPrincipal>(memoryStream);

        return clientPrincipal.Claims;
    }

    public static string GetUserValue(this IHttpContextAccessor httpContextAccessor, string headerName, bool decode = false)
    {
        var result = httpContextAccessor.HttpContext.Request.Headers[headerName].ToString();
        if (!string.IsNullOrWhiteSpace(result) && decode)
        {
            result = HttpUtility.UrlDecode(result);
        }
        return result;
    }

    public static void LogHeaders(this IHttpContextAccessor contextAccessor, ILogger logger)
    {
        foreach (var header in contextAccessor.HttpContext.Request.Headers)
        {
            logger.LogInformation("{HeaderName}: {HeaderValue}", header.Key, header.Value);
        }
    }
}

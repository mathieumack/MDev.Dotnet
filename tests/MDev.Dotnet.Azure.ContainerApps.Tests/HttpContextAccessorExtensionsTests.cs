using System.Text;
using MDev.Dotnet.Azure.ContainerApps.Authentication.Extensions;
using Microsoft.AspNetCore.Http;

namespace MDev.Dotnet.Azure.ContainerApps.Tests;

public class HttpContextAccessorExtensionsTests
{
    [Fact]
    public void UserHeaders_AreReadAndDecoded()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] = "user-123";
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"] = "Ada%20Lovelace";
        var accessor = new HttpContextAccessor { HttpContext = context };

        Assert.Equal("user-123", accessor.GetUserId());
        Assert.Equal("Ada Lovelace", accessor.GetUserFullName(decode: true));
    }

    [Fact]
    public async Task GetClaims_DecodesClientPrincipal()
    {
        const string principal = """
            {"claims":[{"typ":"roles","val":"admin"}]}
            """;
        var context = new DefaultHttpContext();
        context.Request.Headers["X-MS-CLIENT-PRINCIPAL"] =
            Convert.ToBase64String(Encoding.UTF8.GetBytes(principal));
        var accessor = new HttpContextAccessor { HttpContext = context };

        var claim = Assert.Single(await accessor.GetClaims());

        Assert.Equal("roles", claim.Type);
        Assert.Equal("admin", claim.Value);
    }
}

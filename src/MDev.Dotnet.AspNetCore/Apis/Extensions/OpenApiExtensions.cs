
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MDev.Dotnet.AspNetCore.Apis.Extensions;

public static class OpenApiExtensions
{
    public static IHostApplicationBuilder RegisterOpenApi<T>(this IHostApplicationBuilder builder,
                                                                bool forceHttpsServers = false)
    {
        builder.Services.AddOpenApi(options => {
            if (forceHttpsServers)
            {
                options.AddDocumentTransformer((document, context, cancellationToken) => {
                    foreach (var server in document.Servers)
                    {
                        server.Url = server.Url.Replace("http://", "https://");
                    }
                    return Task.CompletedTask;
                });
            }
        });

        return builder;
    }
}
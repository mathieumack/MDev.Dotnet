
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MDev.Dotnet.AspNetCore.Apis.Extensions;

public static class OpenApiExtensions
{
    public static IHostApplicationBuilder RegisterOpenApi(this IHostApplicationBuilder builder,
                                                                bool includeServerUrls = true,
                                                                bool forceHttpsServers = false)
    {
        builder.Services.AddOpenApi(options => {
            if (!includeServerUrls)
            {
                options.AddDocumentTransformer((document, context, cancellationToken) => {
                    if (document.Servers is null || !document.Servers.Any())
                    {
                        return Task.CompletedTask;
                    }
                    
                    document.Servers.Clear();

                    return Task.CompletedTask;
                });
            }
            
            if (forceHttpsServers)
            {
                options.AddDocumentTransformer((document, context, cancellationToken) => {
                    if(document.Servers is null || !document.Servers.Any())
                    {
                        return Task.CompletedTask;
                    }

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
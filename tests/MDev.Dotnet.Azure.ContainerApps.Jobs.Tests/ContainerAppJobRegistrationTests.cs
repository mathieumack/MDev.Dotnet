using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MDev.Dotnet.Azure.ContainerApps.Jobs.Tests;

public class ContainerAppJobRegistrationTests
{
    [Fact]
    public void AddContainerAppJob_UsesConfiguredServiceNameAndPreservesLoggerProviders()
    {
        var provider = new TestLoggerProvider();
        var configuration = Configuration(("CONTAINER_APP_JOB_NAME", "configured-job"));
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<ILoggerProvider>(provider);

        builder.Services.AddContainerAppJob(
            configuration,
            options => options.AddConsoleLogging = false);

        using var host = builder.Build();
        var options = host.Services
            .GetRequiredService<IOptions<ContainerAppJobOptions>>()
            .Value;

        Assert.Equal("configured-job", options.ServiceName);
        Assert.Contains(provider, host.Services.GetServices<ILoggerProvider>());
        Assert.NotEmpty(host.Services.GetServices<IHostedService>());
    }

    [Fact]
    public void AddContainerAppJob_RequiresServiceName()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddContainerAppJob(Configuration()));

        Assert.Contains("ServiceName", exception.Message);
    }

    [Fact]
    public void AddContainerAppJob_RequiresPositiveFlushTimeout()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ServiceCollection().AddContainerAppJob(
                Configuration(),
                options =>
                {
                    options.ServiceName = "job";
                    options.FlushTimeout = TimeSpan.Zero;
                }));

        Assert.Equal("FlushTimeout", exception.ParamName);
    }

    [Fact]
    public void AddContainerAppJob_RejectsMalformedAzureMonitorConnectionStringWithoutExposingIt()
    {
        const string secret = "not-a-connection-string";
        var configuration = Configuration(
            ("APPLICATIONINSIGHTS_CONNECTION_STRING", secret));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddContainerAppJob(
                configuration,
                options =>
                {
                    options.ServiceName = "job";
                    options.EnableAzureMonitor = true;
                }));

        Assert.Contains("APPLICATIONINSIGHTS_CONNECTION_STRING", exception.Message);
        Assert.DoesNotContain(secret, exception.Message);
    }

    [Fact]
    public void AddContainerAppJob_AcceptsAzureMonitorConnectionString()
    {
        var configuration = Configuration(
            ("APPLICATIONINSIGHTS_CONNECTION_STRING",
                "InstrumentationKey=00000000-0000-0000-0000-000000000001"));

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddContainerAppJob(
            configuration,
            options =>
            {
                options.ServiceName = "job";
                options.EnableAzureMonitor = true;
            });

        using var host = builder.Build();
        Assert.NotEmpty(host.Services.GetServices<IHostedService>());
    }

    private static IConfiguration Configuration(
        params (string Key, string? Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(value =>
                new KeyValuePair<string, string?>(value.Key, value.Value)))
            .Build();

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new TestLogger();

        public void Dispose()
        {
        }

        private sealed class TestLogger : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => false;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
            }
        }
    }
}

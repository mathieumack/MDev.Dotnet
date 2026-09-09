using System.Text.Json;
using MDev.Dotnet.AspNetCore.Apis.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MDev.Dotnet.AspNetCore.Tests;

public class AspNetCoreExtensionsTests
{
    [Fact]
    public void GetErrors_SerializesModelStateErrors()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Name is required");

        using var errors = JsonDocument.Parse(modelState.GetErrors());

        var error = Assert.Single(errors.RootElement.EnumerateArray());
        Assert.Equal("Name", error.GetProperty("Key").GetString());
        Assert.Equal(
            "Name is required",
            Assert.Single(error.GetProperty("Errors").EnumerateArray())
                .GetProperty("ErrorMessage")
                .GetString());
    }

    [Fact]
    public void RegistrationExtensions_AddControllersVersioningAndOpenApiServices()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.RegisterControllers<AspNetCoreExtensionsTests>();
        builder.RegisterOpenApi();

        Assert.Contains(builder.Services, descriptor =>
            descriptor.ServiceType.Assembly.GetName().Name?.StartsWith("Asp.Versioning") == true);
        Assert.Contains(builder.Services, descriptor =>
            descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericArguments().Contains(typeof(OpenApiOptions)));
    }
}

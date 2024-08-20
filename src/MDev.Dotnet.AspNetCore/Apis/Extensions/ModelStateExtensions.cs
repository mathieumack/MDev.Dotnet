using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MDev.Dotnet.AspNetCore.Apis.Extensions;

public static class ModelStateExtensions
{
    /// <summary>
    /// Serialize as a json object the model state errors
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    public static string GetErrors(this ModelStateDictionary modelState)
    {
        return System.Text.Json.JsonSerializer.Serialize(modelState.Select(x =>
                new
                {
                    Key = x.Key,
                    Errors = x.Value?.Errors.Select(x => new
                    {
                        x.ErrorMessage,
                        x.Exception
                    })
                }));
    }
}

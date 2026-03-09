using System.Text.Json;
using GeoComponent.Core.ErrorHanders;
using WebView.Models.API;

namespace WebView.Middleware;

public class ApiExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidGeoJsonException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "Invalid GeoJson", ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (Exception ex)
        {
            await WriteError(context, StatusCodes.Status500InternalServerError, "Internal server error", ex.Message);
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string error, string? details)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse
        {
            Error = error,
            Details = details
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
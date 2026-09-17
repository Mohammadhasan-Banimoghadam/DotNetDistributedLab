using System.Net;
using System.Text.Json;
using OrderService.Application.Exceptions;

namespace OrderService.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomerNotFoundException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.NotFound,
                "Customer Not Found",
                ex.Message);
        }
        catch (ServiceUnavailableException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.ServiceUnavailable,
                "Service Unavailable",
                ex.Message);
        }
        catch (Exception ex)
        {
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                ex.Message);
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var response = new
        {
            title,
            status = (int)statusCode,
            detail
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
using System.Text.Json;
using CQRS.Core.Exceptions;
using InvalidOperationException = System.InvalidOperationException;

namespace Post.Cmd.Api.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.Headers.Add("content-type", "application/json");
            
            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", String.Empty));
            
            var json = JsonSerializer.Serialize(new 
                {
                    ErrorCode = errorCode,
                    Message = "Client made a bad request"
                });

            await context.Response.WriteAsync(json);
        }
        catch (AggregateNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", String.Empty));
            
            var json = JsonSerializer.Serialize(new 
            {
                ErrorCode = errorCode,
                Message = "Could not find an aggregate, client passed an invalid id"
            });

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", String.Empty));
            
            var json = JsonSerializer.Serialize(new 
            {
                ErrorCode = errorCode,
                ex.Message
            });

            await context.Response.WriteAsync(json);
        }
    }
    
    private static string ToUnderscoreCase(string value)
        => string.Concat((value ?? string.Empty).Select((x, i) => i > 0 && char.IsUpper(x) && !char.IsUpper(value[i-1]) ? $"_{x}" : x.ToString())).ToLower();
}
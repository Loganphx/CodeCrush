using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate              _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment             _env;

    public ExceptionMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next     = next;
        _logger   = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (BadRequestException badRequestException)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)HttpStatusCode.BadRequest;

            var response = new ApiException(context.Response.StatusCode, badRequestException.Message, "");
            var options  = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json     = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
        catch (UnauthorizedException unauthorizedException)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)HttpStatusCode.Unauthorized;

            var response = new ApiException(context.Response.StatusCode, unauthorizedException.Message, "");
            var options  = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json     = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
        // catch (NotFoundException notFoundException)
        // {
        //     context.Response.ContentType = "application/json";
        //     context.Response.StatusCode  = (int)HttpStatusCode.NotFound;
        //
        //     var response = new ApiException(context.Response.StatusCode, notFoundException.Message, "");
        //     var options  = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        //     var json     = JsonSerializer.Serialize(response, options);
        //
        //     await context.Response.WriteAsync(json);
        // }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
            var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");
            
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json    = JsonSerializer.Serialize(response, options);
            
            await context.Response.WriteAsync(json);
        }
    }
}
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Template.Infra.Authentication;

public class ApiKeyMiddleware(RequestDelegate next, IHostingEnvironment env)
{
    private readonly RequestDelegate _next = next;
    private string _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? throw new ArgumentNullException("API_KEY");
    private string _apiSecret = Environment.GetEnvironmentVariable("API_SECRET") ?? throw new ArgumentNullException("API_SECRET");
    private readonly IHostingEnvironment _env = env;
    
    private const string ApiKeyHeaderName = "X-API-KEY";
    private const string ApiSecretHeaderName = "X-API-SECRET";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        var isLocalRequest = context.Connection.RemoteIpAddress is { } ip &&
                             (IPAddress.IsLoopback(ip) || ip.ToString() == "::1");
        
        if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        if (path.StartsWithSegments("/api/stripe/webhook", StringComparison.OrdinalIgnoreCase) ||
            (path.StartsWithSegments("/swagger") && _env.IsDevelopment()) ||
            isLocalRequest)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey) ||
            !context.Request.Headers.TryGetValue(ApiSecretHeaderName, out var apiSecret) ||
            apiKey != _apiKey || apiSecret != _apiSecret)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid API Key or Secret");
            return;
        }

        await _next(context);
    }
}
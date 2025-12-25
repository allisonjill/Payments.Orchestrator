using System.Collections.Concurrent;
using System.Text.Json;
using Payments.Orchestrator.Api.Models;

namespace Payments.Orchestrator.Api.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;
    // Key: IdempotencyKey, Value: (StatusCode, BodyJson)
    // Note: In a real system, this would be Redis with TTL
    private static readonly ConcurrentDictionary<string, (int StatusCode, string Body)> _cache = new();

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Post)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var keyVal))
        {
            // If header is missing, we proceed (or we could enforce it)
            // For this scope, let's assume it's optional but highly recommended
            await _next(context);
            return;
        }

        var key = keyVal.ToString();

        if (_cache.TryGetValue(key, out var cachedResponse))
        {
            _logger.LogInformation("Idempotency hit for key: {Key}", key);
            context.Response.StatusCode = cachedResponse.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse.Body);
            return;
        }

        // Capture the response
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        // We only cache 2xx created/success responses for idempotency in this simple scope
        // or logic that says "if we processed it, cache it"
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            
            _cache.TryAdd(key, (context.Response.StatusCode, responseBody));
            
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        else
        {
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        context.Response.Body = originalBodyStream;
    }
}

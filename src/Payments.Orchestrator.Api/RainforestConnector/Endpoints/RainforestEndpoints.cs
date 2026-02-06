using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payments.Orchestrator.Api.RainforestConnector.DTOs;

namespace Payments.Orchestrator.Api.RainforestConnector.Endpoints;

public static class RainforestEndpoints
{
    public static void MapRainforestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        // Payin Session Endpoint
        group.MapPost("/payments/rainforest/payin-session", async (
            [FromBody] CreateRainforestPayinSessionRequest request,
            [FromServices] RainforestPayinOrchestrationService service,
            CancellationToken ct) =>
        {
            var result = await service.CreatePayinSessionAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("CreateRainforestPayinSession")
        .WithOpenApi();

        // Webhook Endpoint
        group.MapPost("/webhooks/rainforest", async (
            HttpContext context,
            [FromServices] ILogger<RainforestPayinOrchestrationService> logger, // Using service logger for continuity
            [FromServices] IOptions<RainforestOptions> options) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            // Verify Signature
            // TODO: Implement actual HMAC verification if header/strategy known.
            // For now, checking if key exists to simulate requirement.
            var signingKey = options.Value.WebhookSigningKey;
            if (!string.IsNullOrEmpty(signingKey))
            {
                // TODO: Verify signature using signingKey and body
                // if (!VerifySignature(signingKey, body, context.Request.Headers)) 
                // {
                //     logger.LogWarning("Invalid Rainforest webhook signature");
                //     return Results.Unauthorized();
                // }
            }
            
            // Allowlist Logic (Optional)
            // TODO: Add IP allowlist validation here if needed.

            try
            {
                var envelope = JsonSerializer.Deserialize<RainforestWebhookEnvelope>(body);
                if (envelope == null)
                {
                    logger.LogWarning("Received empty or invalid Rainforest webhook payload");
                    return Results.BadRequest();
                }

                logger.LogInformation("Received Rainforest webhook: {EventType}", envelope.EventType);

                // Handle Event
                switch (envelope.EventType)
                {
                    case "payin.processing":
                    case "payin.succeeded":
                    case "payin.failed":
                    case "payin.canceled":
                        // TODO: Update internal payment status based on these events.
                        // For POC, we just log.
                        logger.LogInformation("Processed Rainforest event {EventType} for payload: {Data}", envelope.EventType, envelope.Data);
                        break;
                    default:
                        logger.LogInformation("Ignored Rainforest event {EventType}", envelope.EventType);
                        break;
                }

                return Results.Ok();
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse Rainforest webhook body");
                return Results.BadRequest("Invalid JSON");
            }
        })
        .WithName("RainforestWebhook")
        .WithOpenApi();
    }
}

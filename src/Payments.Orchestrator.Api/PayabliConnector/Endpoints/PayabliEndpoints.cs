using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payments.Orchestrator.Api.PayabliConnector.DTOs;

namespace Payments.Orchestrator.Api.PayabliConnector.Endpoints;

public static class PayabliEndpoints
{
    public static void MapPayabliEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapPost("/payments/payabli/transaction", async (
            [FromBody] PayabliAuthCaptureRequest request,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.ProcessPaymentAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliProcessTransaction")
        .WithOpenApi();

        group.MapPost("/payments/payabli/void", async (
            [FromBody] PayabliVoidRequest request,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.ProcessVoidAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliVoidTransaction")
        .WithOpenApi();

        group.MapPost("/payments/payabli/refund", async (
            [FromBody] PayabliRefundRequest request,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.ProcessRefundAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliRefundTransaction")
        .WithOpenApi();

        group.MapPut("/payments/payabli/boarding/applink/{appId}/{email}", async (
            int appId,
            string email,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.GenerateAppLinkAsync(appId, email, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliGenerateAppLink")
        .WithOpenApi();

        group.MapGet("/payments/payabli/boarding/paypoints/{orgId}", async (
            int orgId,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.QueryPaypointsAsync(orgId, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliQueryPaypoints")
        .WithOpenApi();

        group.MapPost("/payments/payabli/boarding/app", async (
            [FromBody] PayabliBoardingAppRequest request,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.CreateBoardingAppAsync(request, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliCreateBoardingApp")
        .WithOpenApi();

        group.MapGet("/payments/payabli/boarding/app/{appId}", async (
            int appId,
            [FromServices] IPayabliClient client,
            CancellationToken ct) =>
        {
            var result = await client.ReadBoardingAppAsync(appId, ct);
            return Results.Ok(result);
        })
        .WithName("PayabliReadBoardingApp")
        .WithOpenApi();
    }
}

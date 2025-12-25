using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Payments.Orchestrator.Api.Models;
using Payments.Orchestrator.Api.Services;

namespace Payments.Orchestrator.Api.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/payments")
                       .WithTags("Payments")
                       .WithOpenApi();

        group.MapPost("/", CreatePayment);
        group.MapPost("/{id}/confirm", ConfirmPayment);
        group.MapGet("/{id}", GetPayment);
    }

    static async Task<IResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        [FromServices] IValidator<CreatePaymentRequest> validator,
        [FromServices] PaymentService service)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var intent = await service.CreatePaymentAsync(request.Amount, request.Currency);
        var response = PaymentResponse.FromDomain(intent);

        return Results.Created($"/api/v1/payments/{intent.Id}", response);
    }

    static async Task<IResult> ConfirmPayment(
        [FromRoute] Guid id,
        [FromServices] PaymentService service)
    {
        try
        {
            var intent = await service.ConfirmPaymentAsync(id);
            if (intent == null) return Results.NotFound();

            // 402 if failed, 200 if succeeded/processing
            if (intent.Status == Domain.PaymentStatus.Failed)
            {
                return Results.Json(PaymentResponse.FromDomain(intent), statusCode: 402);
            }

            return Results.Ok(PaymentResponse.FromDomain(intent));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(ex.Message); // State transition error
        }
    }

    static async Task<IResult> GetPayment(
        [FromRoute] Guid id,
        [FromServices] PaymentService service)
    {
        var intent = await service.GetPaymentAsync(id);
        return intent is null ? Results.NotFound() : Results.Ok(PaymentResponse.FromDomain(intent));
    }
}

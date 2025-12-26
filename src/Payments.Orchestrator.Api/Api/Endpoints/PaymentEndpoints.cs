using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Payments.Orchestrator.Api.Application.Models;
using Payments.Orchestrator.Api.Application.Services;
using Payments.Orchestrator.Api.Domain.Enums;

namespace Payments.Orchestrator.Api.Api.Endpoints;

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

        var payment = await service.ProcessPaymentRequestAsync(request.Amount, request.Currency);
        var response = PaymentResponse.FromDomain(payment);

        if (payment.Status == PaymentStatus.Failed)
        {
            return Results.Json(response, statusCode: 402); // Payment Required (Declined)
        }

        return Results.Ok(response); // 200 OK (Captured)
    }

    static async Task<IResult> ConfirmPayment(
        [FromRoute] Guid id,
        [FromServices] PaymentService service)
    {
        try
        {
            var payment = await service.ConfirmPaymentAsync(id);
            if (payment == null) return Results.NotFound();

            // 402 if failed, 200 if succeeded/processing
            if (payment.Status == PaymentStatus.Failed)
            {
                return Results.Json(PaymentResponse.FromDomain(payment), statusCode: 402);
            }

            return Results.Ok(PaymentResponse.FromDomain(payment));
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
        var payment = await service.GetPaymentAsync(id);
        return payment is null ? Results.NotFound() : Results.Ok(PaymentResponse.FromDomain(payment));
    }
}

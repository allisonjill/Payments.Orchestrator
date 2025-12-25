using FluentValidation;
using Payments.Orchestrator.Api.Endpoints;
using Payments.Orchestrator.Api.Infrastructure;
using Payments.Orchestrator.Api.Interfaces;
using Payments.Orchestrator.Api.Models;
using Payments.Orchestrator.Api.Services;
using Payments.Orchestrator.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

// Domain Services
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<IValidator<CreatePaymentRequest>, CreatePaymentRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<Payments.Orchestrator.Api.Middleware.IdempotencyMiddleware>();

app.MapHealthChecks("/health");

// Map Endpoints
app.MapPaymentEndpoints();

app.Run();

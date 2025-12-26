using FluentValidation;
using Payments.Orchestrator.Api.Application.Interfaces;
using Payments.Orchestrator.Api.Application.Models;
using Payments.Orchestrator.Api.Application.Services;
using Payments.Orchestrator.Api.Application.Validators;
using Payments.Orchestrator.Api.Api.Endpoints;
using Payments.Orchestrator.Api.Api.Middleware;
using Payments.Orchestrator.Api.Infrastructure.Gateways;
using Payments.Orchestrator.Api.Infrastructure.Persistence;
using Payments.Orchestrator.Api.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

// Application Services
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();
builder.Services.AddScoped<PaymentService>();

// Infrastructure
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
// builder.Services.AddScoped<IPaymentRepository, DapperPaymentRepository>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<IdempotencyMiddleware>();

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapPaymentEndpoints();

app.Run();

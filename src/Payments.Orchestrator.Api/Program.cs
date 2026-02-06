using FluentValidation;
using MediatR;
using Payments.Orchestrator.Api.Application.Interfaces;
using Payments.Orchestrator.Api.Application.Models;
using Payments.Orchestrator.Api.Application.Validators;
using Payments.Orchestrator.Api.Api.Endpoints;
using Payments.Orchestrator.Api.Api.Middleware;
using Payments.Orchestrator.Api.Infrastructure.Gateways;
using Payments.Orchestrator.Api.Infrastructure.Persistence;
using Payments.Orchestrator.Api.Infrastructure.Services;
using Payments.Orchestrator.Api.RainforestConnector.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

// Application Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("DemoCorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Allow any origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Infrastructure
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
// builder.Services.AddScoped<IPaymentRepository, DapperPaymentRepository>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();


builder.Services.AddRainforestConnector(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("DemoCorsPolicy");

app.UseMiddleware<IdempotencyMiddleware>();

// app.UseHttpsRedirection(); // Disabled for local demo connectivity

app.MapHealthChecks("/health");

app.MapPaymentEndpoints();
app.MapRainforestEndpoints();

app.Run();

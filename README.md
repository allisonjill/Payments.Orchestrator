# Payments.Orchestrator

A .NET 9 Payments Orchestrator API.

## Project Structure
- `src/Payments.Orchestrator.Api`: The Web API project.
- `tests/Payments.Orchestrator.Tests`: Unit and integration tests.

## Prerequisites
- .NET 9 SDK

## How to Run

### Via Command Line (PowerShell)
Navigate to the root directory and run the following command to start the API:

```powershell
dotnet run --project src/Payments.Orchestrator.Api/Payments.Orchestrator.Api.csproj
```

The API will be available at `http://localhost:5079` (or the port assigned by launch settings).

### Health Checks
A health check endpoint is available at:
`GET /health`

### OpenAPI (Swagger)
OpenAPI document is generated at runtime (in Development environment).
You can access the OpenAPI JSON at: `/openapi/v1.json` (Default path for .NET 9 `MapOpenApi`)

## How to Test

Run the tests from the root directory:

```powershell
dotnet test
```

# Payments.Orchestrator

A .NET 9 Payments Orchestrator API designed to demonstrate production-ready architectural patterns, adhering to **Clean Architecture** principles.

## Features

*   **Clean Architecture**: Strict separation of concerns (Domain, Application, Infrastructure, Api).
*   **CQRS with MediatR**: Decoupled Command and Query handlers for better testability and SRP.
*   **Rich Domain Model**: Strong types and state machine (`Initiated` -> `Validated` -> `Authorized` -> `Captured`).
*   **Idempotency**: Robust handling of duplicate requests via `Idempotency-Key` headers.
*   **Persistence Options**:
    *   **In-Memory**: Thread-safe `ConcurrentDictionary` for rapid prototyping (default).
    *   **Dapper**: Low-level, high-performance SQL implementation (code-ready, just enable in `Program.cs`).
*   **Validation**: FluentValidation for request integrity.
*   **Test-Driven**: Comprehensive Unit and Integration tests.

## Architecture

The solution follows a strict dependency rule and uses **CQRS** with **MediatR**:

```text
src/
  Payments.Orchestrator.Api/
    ├── Domain/                     # Enterprise Logic (Entities, Enums)
    ├── Application/
    │   ├── Features/               # Vertical Slices (Commands/Queries)
    │   ├── Interfaces/
    │   └── Validators/
    ├── Infrastructure/             # Implementations (Dapper, SystemClock)
    └── Api/                        # Endpoints (Maps to MediatR)
```

## Prerequisites
- .NET 9 SDK
- PowerShell (for running typical workflows)
- (Optional) SQL Server instance if switching to `DapperPaymentRepository`

## How to Run

### Start the API
Navigate to the root directory and run:

```powershell
dotnet run --project src/Payments.Orchestrator.Api/Payments.Orchestrator.Api.csproj --urls=http://localhost:5200
```

The API will be available at `http://localhost:5200`.
Health Check: `http://localhost:5200/health`

### Run Tests
```powershell
dotnet test
```

## Usage Examples (PowerShell)

#### 1. Create a Payment
```powershell
$response = Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body '{"amount": 100.00, "currency": "USD"}'
Write-Output "Payment Created. ID: $($response.id) Status: $($response.status)"
```

#### 2. Confirm a Payment
Triggers the full lifecycle: `Validate` -> `Authorize` -> `Capture`.
```powershell
Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments/$($response.id)/confirm" -Method Post
```

#### 3. Idempotency Check
Sending the same `Idempotency-Key` header with identical operations returns the cached 2xx response.
```powershell
$key = "test-key-$(Get-Random)"
$body = '{"amount": 50.00, "currency": "USD"}'
# First Request
Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body $body -Headers @{"Idempotency-Key"=$key}
# Second Request (Returns cached result)
Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body $body -Headers @{"Idempotency-Key"=$key}
```

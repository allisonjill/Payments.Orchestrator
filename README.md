# Payments.Orchestrator

A .NET 9 Payments Orchestrator API designed to demonstrate:
*   **Idempotency** (Middleware-based)
*   **Clean Architecture** (Domain, Services, Infrastructure layers)
*   **Validation** (FluentValidation)
*   **Minimal API**

## Project Structure
- `src/Payments.Orchestrator.Api`: The Web API project.
- `tests/Payments.Orchestrator.Tests`: Unit and integration tests using xUnit and Moq.

## Prerequisites
- .NET 9 SDK
- PowerShell (for running the demo commands)

## How to Run

### Start the API
Navigate to the root directory and run:

```powershell
dotnet run --project src/Payments.Orchestrator.Api/Payments.Orchestrator.Api.csproj --urls=http://localhost:5200
```

The API will be available at `http://localhost:5200`.

### Health Checks
- `GET /health` -> 200 OK

## How to Test

### Automated Tests
Run the unit tests from the root directory:

```powershell
dotnet test
```

### Manual API Testing (PowerShell)

You can verify the core flows using the following commands while the API is running.

#### 1. Create a Payment
```powershell
$response = Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body '{"amount": 100.00, "currency": "USD"}'
Write-Output "Payment Created. ID: $($response.id)"
```

#### 2. Confirm a Payment
```powershell
Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments/$($response.id)/confirm" -Method Post
```

#### 3. Test Validation (Negative Amount)
```powershell
try {
    Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body '{"amount": -50, "currency": "USD"}'
} catch {
    Write-Output "Validation Error: $($_.Exception.Message)"
}
```

#### 4. Test Idempotency
Send the same request twice with the same `Idempotency-Key` header. The returned ID should be identical.

```powershell
$key = "test-key-$(Get-Random)"
$body = '{"amount": 10.00, "currency": "USD"}'

$res1 = Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body $body -Headers @{"Idempotency-Key"=$key}
$res2 = Invoke-RestMethod -Uri "http://localhost:5200/api/v1/payments" -Method Post -ContentType "application/json" -Body $body -Headers @{"Idempotency-Key"=$key}

if ($res1.id -eq $res2.id) { Write-Output "✅ Idempotency Works!" } else { Write-Output "❌ Failed" }
```

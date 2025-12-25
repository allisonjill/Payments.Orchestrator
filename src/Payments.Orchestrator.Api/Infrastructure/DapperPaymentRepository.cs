using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Payments.Orchestrator.Api.Domain;
using Payments.Orchestrator.Api.Interfaces;

namespace Payments.Orchestrator.Api.Infrastructure;

public class DapperPaymentRepository : IPaymentRepository
{
    private readonly string _connectionString;

    public DapperPaymentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<Payment?> GetAsync(Guid id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Payments WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Payment>(sql, new { Id = id });
    }

    public async Task SaveAsync(Payment payment)
    {
        using var connection = CreateConnection();
        var sql = @"
            MERGE Payments AS target
            USING (SELECT @Id AS Id) AS source
            ON (target.Id = source.Id)
            WHEN MATCHED THEN
                UPDATE SET 
                    Status = @Status, 
                    GatewayTransactionId = @GatewayTransactionId, 
                    FailureReason = @FailureReason, 
                    ProcessedAt = @ProcessedAt
            WHEN NOT MATCHED THEN
                INSERT (Id, Amount, Currency, Status, GatewayTransactionId, FailureReason, CreatedAt, ProcessedAt)
                VALUES (@Id, @Amount, @Currency, @Status, @GatewayTransactionId, @FailureReason, @CreatedAt, @ProcessedAt);";

        // Dapper handles Enum to String/Int conversion automatically if configured, 
        // but often it's safer to pass status as string if DB expects nvarchar.
        // For simplicity, we assume Dapper default mapping or standard types.
        
        await connection.ExecuteAsync(sql, payment);
    }
}

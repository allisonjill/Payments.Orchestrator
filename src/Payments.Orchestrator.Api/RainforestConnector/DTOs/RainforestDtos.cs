using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payments.Orchestrator.Api.RainforestConnector.DTOs;

// --- Orchestrator API (Internal) DTOs ---

public record CreateRainforestPayinSessionRequest(
    string MerchantId,
    int Amount,
    string Currency,
    string? IdempotencyKey,
    RainforestBillingContact? BillingContact,
    Dictionary<string, string>? Metadata
);

public record CreateRainforestPayinSessionResult(
    string PayinConfigId,
    string SessionKey,
    string? IdempotencyKey
);

// --- Rainforest API (External) DTOs ---

public record RainforestCreatePayinConfigRequest(
    [property: JsonPropertyName("merchant_id")] string MerchantId,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("currency_code")] string Currency,
    [property: JsonPropertyName("idempotency_key")] string IdempotencyKey,
    [property: JsonPropertyName("billing_contact")] RainforestBillingContact? BillingContact,
    [property: JsonPropertyName("metadata")] Dictionary<string, string>? Metadata
);

public record RainforestBillingContact(
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("name")] string? Name
);

public record RainforestCreatePayinConfigResponse(
    [property: JsonPropertyName("payin_config_id")] string PayinConfigId
);

public record RainforestCreateSessionRequest(
    [property: JsonPropertyName("ttl")] int Ttl,
    [property: JsonPropertyName("statements")] List<RainforestSessionStatement> Statements
);

public record RainforestSessionStatement(
    [property: JsonPropertyName("permissions")] List<string> Permissions,
    [property: JsonPropertyName("constraints")] RainforestSessionConstraints Constraints
);

public record RainforestSessionConstraints(
    [property: JsonPropertyName("merchant")] RainforestMerchantConstraint Merchant
);

public record RainforestMerchantConstraint(
    [property: JsonPropertyName("merchant_id")] string MerchantId
);

public record RainforestCreateSessionResponse(
    [property: JsonPropertyName("session_key")] string SessionKey,
    [property: JsonPropertyName("session_id")] string SessionId
);

// Webhook DTOs
public record RainforestWebhookEnvelope(
    [property: JsonPropertyName("event_type")] string EventType,
    [property: JsonPropertyName("data")] JsonElement Data
);

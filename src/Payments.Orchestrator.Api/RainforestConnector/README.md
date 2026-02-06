# Rainforest Connector

This module integrates the Payments Orchestrator with [Rainforest Pay](https://rainforestpay.com/).

## Configuration

Add the following to your `appsettings.json` or User Secrets:

```json
"Rainforest": {
  "BaseUrl": "https://api.sandbox.rainforestpay.com/v1/",
  "ApiKey": "YOUR_SBX_API_KEY",
  "ApiVersion": "2024-10-16",
  "WebhookSigningKey": "YOUR_WEBHOOK_SIGNING_KEY",
  "DefaultSessionTtlSeconds": 3600
}
```

## Endpoints

### Create Payin Session

**POST** `/api/payments/rainforest/payin-session`

Exchanges payment details for a Rainforest `session_key` that the frontend can use to initialize the Rainforest Payment Component.

**Request:**
```json
{
  "merchantId": "sbx_mid_2vYF6MAxOrjH2m...",
  "amount": 100,
  "currency": "USD",
  "idempotencyKey": "unique-uuid",
  "billingContact": {
    "email": "customer@example.com"
  },
  "metadata": {
    "orderRef": "ABC-123"
  }
}
```

**Response:**
```json
{
  "payinConfigId": "payin_config_...",
  "sessionKey": "sess_...",
  "idempotencyKey": "unique-uuid"
}
```

### Webhook

**POST** `/api/webhooks/rainforest`

Receives asynchronous updates from Rainforest.

**Payload:**
```json
{
  "event_type": "payin.succeeded",
  "data": { ... }
}
```

## Helper Notes

1.  **Orchestration Flow**:
    - The `CreatePayinSession` endpoint calls `payin_configs` then `sessions` on Rainforest API.
    - It returns the `session_key` to the UI.
2.  **Webhooks**:
    - Currently logs events.
    - Signature verification is stubbed (check `RainforestEndpoints.cs`). Configure `WebhookSigningKey` and implement the verification logic when details are known.

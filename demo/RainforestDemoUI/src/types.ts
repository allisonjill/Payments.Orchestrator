export interface PayinSessionRequest {
  merchantId: string;
  amount: number;
  currency: string;
}

export interface PayinSessionResponse {
  payinConfigId: string;
  sessionKey: string;
  expiresAt?: string;
}

export interface WebhookEvent {
  event_type: string;
  data: Record<string, any>;
}

export interface WebhookRequest {
  event_type: string;
  data: Record<string, any>;
}

export interface WebhookHistoryItem {
    id: string;
    timestamp: string;
    event_type: string;
    payload: WebhookRequest;
    responseStatus: number;
    responseBody: string;
}

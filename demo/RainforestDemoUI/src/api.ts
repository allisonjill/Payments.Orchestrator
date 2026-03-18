import type { PayinSessionRequest, PayinSessionResponse, WebhookRequest } from './types';

const BASE_URL = import.meta.env.VITE_ORCHESTRATOR_BASE_URL || 'http://localhost:5000';

export async function createPayinSession(data: PayinSessionRequest): Promise<PayinSessionResponse> {
    const response = await fetch(`${BASE_URL}/api/payments/rainforest/payin-session`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Error ${response.status}: ${errorText}`);
    }

    return response.json();
}

export async function sendWebhook(data: WebhookRequest): Promise<{ status: number; body: string; timestamp: string }> {
    const response = await fetch(`${BASE_URL}/api/webhooks/rainforest`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
    });

    const text = await response.text();
    return {
        status: response.status,
        body: text,
        timestamp: new Date().toISOString(),
    };
}

export async function getRecentWebhooks(): Promise<any[]> {
    try {
        const response = await fetch(`${BASE_URL}/api/webhooks/rainforest/events`);
        console.log('[DEBUG] Polling status:', response.status);
        if (!response.ok) return [];
        const json = await response.json();
        console.log('[DEBUG] Polling result:', json);
        return json;
    } catch (e) {
        console.error('Failed to fetch webhooks', e);
        return [];
    }
}

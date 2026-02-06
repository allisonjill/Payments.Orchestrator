import React, { useState, useEffect } from 'react';
import { sendWebhook } from '../../api';
import type { WebhookRequest, WebhookHistoryItem } from '../../types';

interface WebhookSimulatorProps {
    onEventSent: (item: WebhookHistoryItem) => void;
    template?: { eventType: string; data: string } | null;
}

export const WebhookSimulator: React.FC<WebhookSimulatorProps> = ({ onEventSent, template }) => {
    const [eventType, setEventType] = useState('payin.succeeded');
    const [payloadData, setPayloadData] = useState('{\n  "payinId": "pi_123",\n  "amount": 100\n}');
    const [loading, setLoading] = useState(false);
    const [lastResponse, setLastResponse] = useState<{ status: number; body: string } | null>(null);

    useEffect(() => {
        if (template) {
            setEventType(template.eventType);
            setPayloadData(template.data);
        }
    }, [template]);

    const handleSend = async () => {
        setLoading(true);
        setLastResponse(null);
        try {
            let parsedData;
            try {
                parsedData = JSON.parse(payloadData);
            } catch (e) {
                alert('Invalid JSON in data field');
                setLoading(false);
                return;
            }

            const request: WebhookRequest = {
                event_type: eventType,
                data: parsedData
            };

            const result = await sendWebhook(request);
            setLastResponse({ status: result.status, body: result.body });

            const historyItem: WebhookHistoryItem = {
                id: Date.now().toString(),
                timestamp: result.timestamp,
                event_type: eventType,
                payload: request,
                responseStatus: result.status,
                responseBody: result.body
            };
            onEventSent(historyItem);

        } catch (err: any) {
            setLastResponse({ status: 0, body: err.message });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="card">
            <h2>Webhook Simulator</h2>
            <div className="form-group">
                <label>Event Type</label>
                <select value={eventType} onChange={(e) => setEventType(e.target.value)}>
                    <option value="payin.processing">payin.processing</option>
                    <option value="payin.succeeded">payin.succeeded</option>
                    <option value="payin.failed">payin.failed</option>
                    <option value="payin.canceled">payin.canceled</option>
                </select>
            </div>

            <div className="form-group">
                <label>Data (JSON)</label>
                <textarea
                    rows={10}
                    value={payloadData}
                    onChange={(e) => setPayloadData(e.target.value)}
                    style={{ fontFamily: 'monospace' }}
                />
            </div>

            <div className="flex-row">
                <button onClick={handleSend} disabled={loading}>
                    {loading ? 'Sending...' : 'Send Webhook'}
                </button>
                <button className="secondary" onClick={() => setLastResponse(null)}>Clear Log</button>
            </div>

            {lastResponse && (
                <div style={{ marginTop: '1rem', padding: '1rem', background: lastResponse.status >= 200 && lastResponse.status < 300 ? 'rgba(76, 175, 80, 0.1)' : 'rgba(244, 67, 54, 0.1)', border: '1px solid currentColor', borderRadius: '4px' }}>
                    <strong>Response: {lastResponse.status}</strong>
                    <pre>{lastResponse.body}</pre>
                </div>
            )}
        </div>
    );
};

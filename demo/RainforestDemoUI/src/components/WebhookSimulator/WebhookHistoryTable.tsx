import React from 'react';
import type { WebhookHistoryItem } from '../../types';

interface WebhookHistoryTableProps {
    history: WebhookHistoryItem[];
    onResend: (item: WebhookHistoryItem) => void;
    onDuplicate: (item: WebhookHistoryItem) => void;
}

export const WebhookHistoryTable: React.FC<WebhookHistoryTableProps> = ({ history, onResend, onDuplicate }) => {
    if (history.length === 0) {
        return <div className="card"><p>No webhooks sent yet.</p></div>;
    }

    return (
        <div className="card">
            <h2>Webhook History</h2>
            <table>
                <thead>
                    <tr>
                        <th>Time</th>
                        <th>Event</th>
                        <th>Response</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {history.map((item) => (
                        <tr key={item.id}>
                            <td>{new Date(item.timestamp).toLocaleTimeString()}</td>
                            <td>{item.event_type}</td>
                            <td style={{ color: item.responseStatus === 200 ? 'var(--success-color)' : 'var(--error-color)' }}>{item.responseStatus}</td>
                            <td>
                                <div className="flex-row">
                                    <button className="secondary" style={{ padding: '0.2em 0.5em', fontSize: '0.9em' }} onClick={() => onResend(item)}>
                                        Resend
                                    </button>
                                    <button className="secondary" style={{ padding: '0.2em 0.5em', fontSize: '0.9em' }} onClick={() => onDuplicate(item)}>
                                        Duplicate
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

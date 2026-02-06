import React from 'react';

interface StatusPanelProps {
    status: 'pending' | 'processing' | 'succeeded' | 'failed' | 'canceled';
}

export const StatusPanel: React.FC<StatusPanelProps> = ({ status }) => {
    const steps = [
        { key: 'pending', label: 'Pending' },
        { key: 'processing', label: 'Processing' },
        { key: 'succeeded', label: 'Succeeded' },
    ];

    return (
        <div className="status-panel" style={{ padding: '1rem', background: '#333', borderRadius: '8px', marginBottom: '1rem' }}>
            <div className="flex-row" style={{ justifyContent: 'space-between', alignItems: 'center' }}>
                <strong>Payment Status:</strong>
                <div className="flex-row">
                    {steps.map((step) => {
                        const isActive = status === step.key;
                        return (
                            <span
                                key={step.key}
                                className={`status-badge ${isActive ? `status-${step.key}` : ''}`}
                                style={{ opacity: isActive ? 1 : 0.5 }}
                            >
                                {step.label}
                            </span>
                        );
                    })}
                    {/* Handle failure/canceled which are not in the main success flow */}
                    {(status === 'failed' || status === 'canceled') && (
                        <span className={`status-badge status-${status}`}>{status.toUpperCase()}</span>
                    )}
                </div>
            </div>
        </div>
    );
};

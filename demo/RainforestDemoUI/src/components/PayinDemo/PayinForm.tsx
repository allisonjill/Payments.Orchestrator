import React, { useState } from 'react';
import { createPayinSession } from '../../api';
import type { PayinSessionResponse } from '../../types';

interface PayinFormProps {
    onSessionCreated: (session: PayinSessionResponse) => void;
}

export const PayinForm: React.FC<PayinFormProps> = ({ onSessionCreated }) => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [formData, setFormData] = useState({
        merchantId: '', // User must enter a valid Merchant ID
        amount: 100,
        currency: 'USD',
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            const session = await createPayinSession({
                merchantId: formData.merchantId,
                amount: Number(formData.amount),
                currency: formData.currency,
            });
            onSessionCreated(session);
        } catch (err: any) {
            setError(err.message || 'Failed to create session');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="card">
            <h2>Payin Configuration</h2>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>Merchant ID</label>
                    <input
                        type="text"
                        value={formData.merchantId}
                        onChange={(e) => setFormData({ ...formData, merchantId: e.target.value })}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Amount (cents)</label>
                    <input
                        type="number"
                        value={formData.amount}
                        onChange={(e) => setFormData({ ...formData, amount: Number(e.target.value) })}
                        required
                    />
                </div>
                <div className="form-group">
                    <label>Currency</label>
                    <select
                        value={formData.currency}
                        onChange={(e) => setFormData({ ...formData, currency: e.target.value })}
                    >
                        <option value="USD">USD</option>
                        <option value="CAD">CAD</option>
                    </select>
                </div>

                {error && <div style={{ color: 'var(--error-color)', marginBottom: '1rem' }}>{error}</div>}

                <button type="submit" disabled={loading}>
                    {loading ? 'Creating Session...' : 'Create Session'}
                </button>
            </form>
        </div>
    );
};

import React from 'react';
import type { PayinSessionResponse } from '../../types';

interface SessionResultPanelProps {
    session: PayinSessionResponse;
}

export const SessionResultPanel: React.FC<SessionResultPanelProps> = ({ session }) => {
    return (
        <div className="card">
            <h2>Session Created</h2>
            <div className="form-group" style={{ position: 'relative' }}>
                <label>Payin Config ID</label>
                <input readOnly value={session.payinConfigId} onClick={(e) => e.currentTarget.select()} />
            </div>
            <div className="form-group">
                <label>Session Key</label>
                <input readOnly value={session.sessionKey} onClick={(e) => e.currentTarget.select()} />
            </div>
            {session.expiresAt && (
                <div className="form-group">
                    <label>Expires At</label>
                    <input readOnly value={session.expiresAt} />
                </div>
            )}
        </div>
    );
};

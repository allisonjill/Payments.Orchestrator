import { useState } from 'react';
import type { PayabliVoidRequest, PayabliBaseResponse } from '../types';
import { processVoid } from '../api/payabli';
import { TransactionResult } from './TransactionResult';
import { XCircle, SpinnerGap } from '@phosphor-icons/react';

export function VoidForm() {
  const [isLoading, setIsLoading] = useState(false);
  const [response, setResponse] = useState<PayabliBaseResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [transactionId, setTransactionId] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setResponse(null);
    setError(null);

    const request: PayabliVoidRequest = { transactionId };

    try {
      const res = await processVoid(request);
      setResponse(res);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An unknown error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-card">
      <div className="card-title">
        <XCircle size={24} weight="duotone" color="var(--primary)" />
        Void Transaction
      </div>
      
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Transaction ID</label>
          <input 
            name="transactionId" 
            value={transactionId} 
            onChange={(e) => setTransactionId(e.target.value)} 
            placeholder="e.g. 123456789"
            required
          />
          <small style={{ color: 'var(--text-muted)', fontSize: '0.75rem', marginTop: '0.25rem', display: 'block' }}>
            Enter a successful Transaction ID from the Auth/Capture step.
          </small>
        </div>

        <button type="submit" disabled={isLoading || !transactionId} className="danger">
          {isLoading ? <SpinnerGap className="spinner" size={20} /> : 'Void Transaction'}
        </button>
      </form>

      <TransactionResult response={response} error={error} isLoading={isLoading} />
    </div>
  );
}

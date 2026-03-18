import { useState } from 'react';
import type { PayabliRefundRequest, PayabliBaseResponse } from '../types';
import { processRefund } from '../api/payabli';
import { TransactionResult } from './TransactionResult';
import { ArrowUUpLeft, SpinnerGap } from '@phosphor-icons/react';

export function RefundForm() {
  const [isLoading, setIsLoading] = useState(false);
  const [response, setResponse] = useState<PayabliBaseResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [transactionId, setTransactionId] = useState('');
  const [amount, setAmount] = useState('10.00');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setResponse(null);
    setError(null);

    const request: PayabliRefundRequest = { 
      transactionId, 
      amount: parseFloat(amount) 
    };

    try {
      const res = await processRefund(request);
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
        <ArrowUUpLeft size={24} weight="duotone" color="var(--primary)" />
        Refund Transaction
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
        </div>

        <div className="form-group">
          <label>Amount ($)</label>
          <input 
            type="number" 
            step="0.01" 
            name="amount" 
            value={amount} 
            onChange={(e) => setAmount(e.target.value)} 
            required
          />
        </div>

        <button type="submit" disabled={isLoading || !transactionId} className="danger">
          {isLoading ? <SpinnerGap className="spinner" size={20} /> : 'Process Refund'}
        </button>
      </form>

      <TransactionResult response={response} error={error} isLoading={isLoading} />
    </div>
  );
}

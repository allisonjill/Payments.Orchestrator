import { useState } from 'react';
import type { PayabliAuthCaptureRequest, PayabliBaseResponse } from '../types';
import { processTransaction } from '../api/payabli';
import { TransactionResult } from './TransactionResult';
import { CreditCard, SpinnerGap } from '@phosphor-icons/react';

export function TransactionForm() {
  const [isLoading, setIsLoading] = useState(false);
  const [response, setResponse] = useState<PayabliBaseResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    cardNumber: '4111111111111111',
    expiry: '1225',
    cvv: '123',
    amount: '10.00',
    surcharge: '',
    convenienceFee: '',
    firstName: 'John',
    lastName: 'Doe',
    address: '123 Main St',
    city: 'Somewhere',
    state: 'CA',
    zip: '90210'
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setResponse(null);
    setError(null);

    const request: PayabliAuthCaptureRequest = {
      entryPoint: "1", // Keyed
      paymentMethod: {
        cardNumber: formData.cardNumber,
        expirationDate: formData.expiry,
        cvv2: formData.cvv
      },
      paymentDetails: {
        amount: parseFloat(formData.amount),
        surchargeAmount: formData.surcharge ? parseFloat(formData.surcharge) : undefined,
        convenienceFeeAmount: formData.convenienceFee ? parseFloat(formData.convenienceFee) : undefined
      },
      customerData: {
        firstName: formData.firstName,
        lastName: formData.lastName,
        address1: formData.address,
        city: formData.city,
        state: formData.state,
        zip: formData.zip,
        country: 'USA'
      }
    };

    try {
      const res = await processTransaction(request);
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
        <CreditCard size={24} weight="duotone" color="var(--primary)" />
        Process Transaction
      </div>
      
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Card Number</label>
          <input 
            name="cardNumber" 
            value={formData.cardNumber} 
            onChange={handleChange} 
            placeholder="0000 0000 0000 0000"
            required
          />
        </div>
        
        <div className="form-row">
          <div className="form-group">
            <label>Expiry (MMYY)</label>
            <input 
              name="expiry" 
              value={formData.expiry} 
              onChange={handleChange} 
              placeholder="MMYY"
              required
            />
          </div>
          <div className="form-group">
            <label>CVV</label>
            <input 
              name="cvv" 
              value={formData.cvv} 
              onChange={handleChange} 
              placeholder="123"
              required
            />
          </div>
        </div>

        <div className="form-group">
          <label>Amount ($)</label>
          <input 
            type="number" 
            step="0.01" 
            name="amount" 
            value={formData.amount} 
            onChange={handleChange} 
            required
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Surcharge ($)</label>
            <input 
              type="number" 
              step="0.01" 
              name="surcharge" 
              value={formData.surcharge} 
              onChange={handleChange} 
              placeholder="Optional"
            />
          </div>
          <div className="form-group">
            <label>Conv. Fee ($)</label>
            <input 
              type="number" 
              step="0.01" 
              name="convenienceFee" 
              value={formData.convenienceFee} 
              onChange={handleChange} 
              placeholder="Optional"
            />
          </div>
        </div>

        <button type="submit" disabled={isLoading}>
          {isLoading ? <SpinnerGap className="spinner" size={20} /> : 'Process Payment'}
        </button>
      </form>

      <TransactionResult response={response} error={error} isLoading={isLoading} />
    </div>
  );
}

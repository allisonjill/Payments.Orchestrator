import type { 
  PayabliAuthCaptureRequest, 
  PayabliVoidRequest, 
  PayabliRefundRequest,
  PayabliBaseResponse
} from '../types';

const API_BASE_URL = 'http://localhost:5114/api/payments/payabli';

export async function processTransaction(request: PayabliAuthCaptureRequest): Promise<PayabliBaseResponse> {
  const response = await fetch(`${API_BASE_URL}/transaction`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error(`Transaction failed: ${response.statusText}`);
  }

  return response.json();
}

export async function processVoid(request: PayabliVoidRequest): Promise<PayabliBaseResponse> {
  const response = await fetch(`${API_BASE_URL}/void`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error(`Void failed: ${response.statusText}`);
  }

  return response.json();
}

export async function processRefund(request: PayabliRefundRequest): Promise<PayabliBaseResponse> {
  const response = await fetch(`${API_BASE_URL}/refund`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error(`Refund failed: ${response.statusText}`);
  }

  return response.json();
}

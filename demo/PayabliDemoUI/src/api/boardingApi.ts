import type {
  PayabliBoardingAppRequest,
  PayabliBoardingAppResponse,
  PayabliQueryPaypointsResponse,
  PayabliAppLinkResponse
} from '../types';

const API_BASE_URL = 'http://localhost:5114';

export const boardingApi = {
  checkPaypoints: async (orgId: number): Promise<PayabliQueryPaypointsResponse> => {
    const response = await fetch(`${API_BASE_URL}/api/payments/payabli/boarding/paypoints/${orgId}`);
    if (!response.ok) {
      throw new Error(`Failed to check paypoints: ${response.statusText}`);
    }
    return response.json();
  },

  createApp: async (payload: PayabliBoardingAppRequest): Promise<PayabliBoardingAppResponse> => {
    const response = await fetch(`${API_BASE_URL}/api/payments/payabli/boarding/app`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });
    
    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Failed to create boarding app: ${text}`);
    }
    return response.json();
  },

  getAppLink: async (appId: number, email: string): Promise<PayabliAppLinkResponse> => {
    const encodedEmail = encodeURIComponent(email);
    const response = await fetch(`${API_BASE_URL}/api/payments/payabli/boarding/applink/${appId}/${encodedEmail}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      }
    });
    
    if (!response.ok) {
      const text = await response.text();
      throw new Error(`Failed to get app link: ${text}`);
    }
    return response.json();
  },

  readApp: async (appId: number) => {
    const response = await fetch(`${API_BASE_URL}/api/payments/payabli/boarding/app/${appId}`);
    if (!response.ok) {
      throw new Error(`Failed to read app: ${response.statusText}`);
    }
    return response.json();
  }
};

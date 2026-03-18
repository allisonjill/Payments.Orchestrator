import type { PayabliBaseResponse } from '../types';

interface TransactionResultProps {
  response: PayabliBaseResponse | null;
  error: string | null;
  isLoading: boolean;
}

export function TransactionResult({ response, error, isLoading }: TransactionResultProps) {
  if (isLoading) return null;
  if (!response && !error) return null;

  if (error) {
    return (
      <div className="result-box result-error">
        <strong>Error:</strong> {error}
      </div>
    );
  }

  if (response) {
    const isSuccess = response.responseCode === "00" || response.responseCode === "0";
    
    return (
      <div className={`result-box ${isSuccess ? 'result-success' : 'result-error'}`}>
        <div>
          <strong>Status:</strong> {isSuccess ? 'Success' : 'Failed'} ({response.responseCode})
        </div>
        <div>
          <strong>Message:</strong> {response.responseMessage}
        </div>
        
        {response.responseData?.transId && (
          <div className="result-data">
            Transaction ID: {response.responseData.transId}
            {response.responseData.authCode && (
              <><br/>Auth Code: {response.responseData.authCode}</>
            )}
          </div>
        )}
      </div>
    );
  }

  return null;
}

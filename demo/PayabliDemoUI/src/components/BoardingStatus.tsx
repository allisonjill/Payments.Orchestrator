import React, { useState } from 'react';
import { boardingApi } from '../api/boardingApi';

interface BoardingStatusProps {
  appId: number;
  email: string | null;
}

export const BoardingStatus: React.FC<BoardingStatusProps> = ({ appId, email }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [appLinkData, setAppLinkData] = useState<{ url: string; referenceId: string } | null>(null);
  const [rawStatus, setRawStatus] = useState<any>(null);

  const handleGenerateLink = async () => {
    if (!email) {
      setError("Email is missing. Cannot generate link.");
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const response = await boardingApi.getAppLink(appId, email);
      if (response.isSuccess && response.responseData) {
        const baseLink = response.responseData.appLink;
        const refId = response.responseData.referenceId;
        const finalUrl = `${baseLink}?email=${encodeURIComponent(email)}&referenceId=${encodeURIComponent(refId)}`;
        
        setAppLinkData({
          url: finalUrl,
          referenceId: refId
        });
      } else {
        setError(response.responseText || "Failed to generate link");
      }
    } catch (err: any) {
      setError(err.message || "An error occurred generating the link");
    } finally {
      setLoading(false);
    }
  };

  const handleRefreshStatus = async () => {
    try {
      setLoading(true);
      const res = await boardingApi.readApp(appId);
      setRawStatus(res.responseData);
      setError(null);
    } catch (err: any) {
      setError("Failed to fetch current status");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="glass-card">
      <h2 className="card-title">Merchant Boarding Status</h2>
      <div className="result-box result-success" style={{ marginBottom: '1.5rem' }}>
        <strong>Success!</strong> Payabli Application Initialized.
        <br />
        <span style={{ fontFamily: 'monospace', opacity: 0.9 }}>App ID: {appId}</span>
      </div>

      {error && (
        <div className="result-box result-error" style={{ marginBottom: '1.5rem' }}>
          <strong>Error:</strong> {error}
        </div>
      )}

      {!appLinkData ? (
        <div style={{ marginBottom: '2rem' }}>
          <p style={{ color: 'var(--text-muted)', marginBottom: '1rem', fontSize: '0.9rem' }}>
            The application is in-flight. Click below to retrieve the secure Payabli-hosted boarding link using your merchant's email.
          </p>
          <button 
            type="button" 
            onClick={handleGenerateLink} 
            disabled={loading || !email}
          >
            {loading ? <span className="spinner">⏳</span> : '✨'} Generate Hosted Boarding Link
          </button>
        </div>
      ) : (
        <div className="result-box" style={{ background: 'var(--bg-input)', border: '1px solid var(--border)', marginBottom: '2rem' }}>
          <p style={{ fontWeight: '600', color: 'var(--text-main)', marginBottom: '0.5rem' }}>Boarding Link Generated</p>
          <div className="result-data" style={{ marginBottom: '1rem', color: 'var(--text-muted)' }}>
            Reference ID: <strong>{appLinkData.referenceId}</strong>
          </div>
          <a 
            href={appLinkData.url} 
            target="_blank" 
            rel="noreferrer"
            style={{ display: 'inline-block', width: '100%', textAlign: 'center', padding: '0.875rem', backgroundColor: 'var(--success)', color: 'white', textDecoration: 'none', borderRadius: '0.5rem', fontWeight: '600', transition: 'all 0.2s', marginTop: '0.5rem' }}
          >
            Launch Hosted Boarding 🚀
          </a>
        </div>
      )}

      <div style={{ borderTop: '1px solid var(--border)', paddingTop: '1.5rem' }}>
        <button 
          onClick={handleRefreshStatus} 
          disabled={loading}
          style={{ backgroundColor: 'var(--bg-input)', color: 'var(--text-main)', border: '1px solid var(--border)' }}
        >
          {loading ? 'Refreshing...' : 'Refresh Application Status JSON'}
        </button>
        {rawStatus && (
          <div className="result-data" style={{ marginTop: '1rem', maxHeight: '300px', overflowY: 'auto' }}>
            <pre style={{ margin: 0, fontSize: '0.8rem' }}>{JSON.stringify(rawStatus, null, 2)}</pre>
          </div>
        )}
      </div>
    </div>
  );
};

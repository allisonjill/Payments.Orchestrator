import { useState, useEffect } from 'react';
import { ShieldCheck } from '@phosphor-icons/react';
import { TransactionForm } from './components/TransactionForm';
import { VoidForm } from './components/VoidForm';
import { RefundForm } from './components/RefundForm';
import { BoardingForm } from './components/BoardingForm';
import { BoardingStatus } from './components/BoardingStatus';

type Tab = 'PAYMENTS' | 'BOARDING';

function App() {
  const [activeTab, setActiveTab] = useState<Tab>('PAYMENTS');
  
  const [appId, setAppId] = useState<number | null>(null);
  const [boardingEmail, setBoardingEmail] = useState<string | null>(null);
  const ORG_ID = 7158;

  useEffect(() => {
    // Load from local storage for demo continuity
    const savedAppId = localStorage.getItem('demo_appId');
    const savedEmail = localStorage.getItem('demo_email');
    if (savedAppId) setAppId(parseInt(savedAppId, 10));
    if (savedEmail) setBoardingEmail(savedEmail);
  }, []);

  const handleBoardingSuccess = (newAppId: number, email: string) => {
    setAppId(newAppId);
    setBoardingEmail(email);
    localStorage.setItem('demo_appId', newAppId.toString());
    localStorage.setItem('demo_email', email);
  };

  const handleResetBoarding = () => {
    setAppId(null);
    setBoardingEmail(null);
    localStorage.removeItem('demo_appId');
    localStorage.removeItem('demo_email');
  };

  return (
    <div className="app-container">
      <header className="header" style={{ marginBottom: '20px' }}>
        <h1>Payabli Orchestrator Demo</h1>
        <p>Interactive Demo for Payments and Merchant Boarding via Orchestrator API</p>
        <div style={{ display: 'flex', justifyContent: 'center', marginTop: '1rem', color: 'var(--success)' }}>
          <ShieldCheck size={32} weight="duotone" />
        </div>
      </header>

      <div className="tabs" style={{ display: 'flex', justifyContent: 'center', gap: '15px', marginBottom: '30px' }}>
        <button 
          className={`tab-button ${activeTab === 'PAYMENTS' ? 'active' : ''}`}
          onClick={() => setActiveTab('PAYMENTS')}
          style={{ padding: '10px 20px', fontSize: '16px', fontWeight: 'bold', border: 'none', borderBottom: activeTab === 'PAYMENTS' ? '3px solid #0052cc' : '3px solid transparent', background: 'none', cursor: 'pointer', color: activeTab === 'PAYMENTS' ? '#0052cc' : '#666' }}
        >
          Payments Demo
        </button>
        <button 
          className={`tab-button ${activeTab === 'BOARDING' ? 'active' : ''}`}
          onClick={() => setActiveTab('BOARDING')}
          style={{ padding: '10px 20px', fontSize: '16px', fontWeight: 'bold', border: 'none', borderBottom: activeTab === 'BOARDING' ? '3px solid #0052cc' : '3px solid transparent', background: 'none', cursor: 'pointer', color: activeTab === 'BOARDING' ? '#0052cc' : '#666' }}
        >
          Merchant Boarding
        </button>
      </div>

      {activeTab === 'PAYMENTS' && (
        <div className="grid-container">
          <TransactionForm />
          <div style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
            <VoidForm />
            <RefundForm />
          </div>
        </div>
      )}

      {activeTab === 'BOARDING' && (
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
          {!appId ? (
            <BoardingForm onSuccess={handleBoardingSuccess} orgId={ORG_ID} />
          ) : (
            <>
              <BoardingStatus appId={appId} email={boardingEmail} />
              <div style={{ marginTop: '20px', textAlign: 'center' }}>
                <button 
                  onClick={handleResetBoarding}
                  style={{ background: 'none', border: 'none', color: '#c62828', textDecoration: 'underline', cursor: 'pointer' }}
                >
                  Clear Local State & Start Over
                </button>
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default App;

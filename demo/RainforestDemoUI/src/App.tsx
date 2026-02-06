import { useState } from 'react';
import { StatusPanel } from './components/StatusPanel';
import { PayinForm } from './components/PayinDemo/PayinForm';
import { SessionResultPanel } from './components/PayinDemo/SessionResultPanel';
import { RainforestPaymentComponent } from './components/PayinDemo/RainforestPaymentComponent';
import { WebhookSimulator } from './components/WebhookSimulator/WebhookSimulator';
import { WebhookHistoryTable } from './components/WebhookSimulator/WebhookHistoryTable';
import type { PayinSessionResponse, WebhookHistoryItem } from './types';
import './index.css';

function App() {
  const [activeTab, setActiveTab] = useState<'payin' | 'webhook'>('payin');
  const [status, setStatus] = useState<'pending' | 'processing' | 'succeeded' | 'failed' | 'canceled'>('pending');
  const [session, setSession] = useState<PayinSessionResponse | null>(null);
  const [webhookHistory, setWebhookHistory] = useState<WebhookHistoryItem[]>([]);

  const handleSessionCreated = (newSession: PayinSessionResponse) => {
    setSession(newSession);
    setStatus('pending'); // Reset status on new session
  };

  const handleEventSent = (item: WebhookHistoryItem) => {
    setWebhookHistory((prev) => [item, ...prev]);

    // Update status based on event type
    const eventType = item.event_type;
    if (eventType === 'payin.processing') {
      setStatus('processing');
    } else if (eventType === 'payin.succeeded') {
      setStatus('succeeded');
    } else if (eventType === 'payin.failed') {
      setStatus('failed');
    } else if (eventType === 'payin.canceled') {
      setStatus('canceled');
    }
  };

  // Re-thinking Resend/DuplicateRequirement:
  // "Re-send" -> Button in table -> Sends immediately.
  // "Duplicate" -> Button in table -> Fills editor.

  // I will refactor WebhookSimulator to accept `template` prop.
  const [simulatorTemplate, setSimulatorTemplate] = useState<{ eventType: string, data: string } | null>(null);

  const triggerResend = async (item: WebhookHistoryItem) => {
    // Direct resend logic
    const { sendWebhook } = await import('./api');
    try {
      const result = await sendWebhook(item.payload);
      const newItem: WebhookHistoryItem = {
        ...item,
        id: Date.now().toString(),
        timestamp: result.timestamp,
        responseStatus: result.status,
        responseBody: result.body
      };
      handleEventSent(newItem);
    } catch (e) {
      console.error(e);
      alert('Failed to resend');
    }
  };

  return (
    <div className="app-container">
      <h1>Rainforest Connector Demo</h1>

      <StatusPanel status={status} />

      <div className="tabs">
        <div
          className={`tab ${activeTab === 'payin' ? 'active' : ''}`}
          onClick={() => setActiveTab('payin')}
        >
          Payin Demo
        </div>
        <div
          className={`tab ${activeTab === 'webhook' ? 'active' : ''}`}
          onClick={() => setActiveTab('webhook')}
        >
          Webhook Simulator
        </div>
      </div>

      <div className="content">
        {activeTab === 'payin' && (
          <div className="two-col-grid">
            <div>
              <PayinForm onSessionCreated={handleSessionCreated} />
              {session && <SessionResultPanel session={session} />}
            </div>
            <div>
              {session ? (
                <RainforestPaymentComponent
                  key={session.sessionKey}
                  sessionKey={session.sessionKey}
                  payinConfigId={session.payinConfigId}
                />
              ) : (
                <div className="card" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '300px', opacity: 0.5 }}>
                  Create a session to see the component
                </div>
              )}
            </div>
          </div>
        )}

        {activeTab === 'webhook' && (
          <div className="two-col-grid">
            <WebhookSimulator
              onEventSent={handleEventSent}
              template={simulatorTemplate}
            />
            <WebhookHistoryTable
              history={webhookHistory}
              onResend={triggerResend}
              onDuplicate={(item) => setSimulatorTemplate({
                eventType: item.event_type,
                data: JSON.stringify(item.payload.data, null, 2)
              })}
            />
          </div>
        )}
      </div>
    </div>
  );
}

export default App;

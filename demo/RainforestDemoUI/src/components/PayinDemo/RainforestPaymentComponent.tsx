import React, { useEffect, useState, useRef } from 'react';

interface RainforestPaymentComponentProps {
    sessionKey: string;
    payinConfigId: string;
}

export const RainforestPaymentComponent: React.FC<RainforestPaymentComponentProps> = ({ sessionKey, payinConfigId }) => {
    const [scriptLoaded, setScriptLoaded] = useState(false);
    const componentRef = useRef<HTMLElement>(null);

    useEffect(() => {
        // ID for the script to prevent duplicate loading
        const scriptId = 'rainforest-payment-js';

        if (document.getElementById(scriptId)) {
            setScriptLoaded(true);
            return;
        }

        const script = document.createElement('script');
        script.id = scriptId;
        script.src = 'https://static.rainforestpay.com/sandbox.payment.js';
        script.type = 'module'; // Web components often need module type
        script.async = true;

        script.onload = () => {
            console.log('Rainforest Payment SDK loaded');
            setScriptLoaded(true);
        };

        script.onerror = (e) => {
            console.error('Failed to load Rainforest Payment SDK', e);
        };

        document.body.appendChild(script);
    }, []);

    useEffect(() => {
        const component = componentRef.current;
        if (!component) return;

        const handleApproved = (e: any) => {
            console.log('Payment Approved:', e.detail);
            const id = e.detail?.payin_id || e.detail?.data?.payin_id || '';
            alert(`Payment Approved! ${id ? `(ID: ${id})` : ''}`);
        };

        const handleError = (e: any) => {
            console.error('Payment Error:', e.detail);
            alert('Payment Error. See console for details.');
        };

        const handleDeclined = (e: any) => {
            console.warn('Payment Declined:', e.detail);
            alert('Payment Declined. Please try another card.');
        };

        component.addEventListener('approved', handleApproved);
        component.addEventListener('error', handleError);
        component.addEventListener('declined', handleDeclined);
        console.log('Event listeners attached to Rainforest component');

        return () => {
            component.removeEventListener('approved', handleApproved);
            component.removeEventListener('error', handleError);
            component.removeEventListener('declined', handleDeclined);
        };
    }, [scriptLoaded]);

    if (!scriptLoaded) {
        return <div className="card loading-state">Loading Secure Payment Form...</div>;
    }

    return (
        <div className="card" style={{ minHeight: '300px' }}>
            <h3>Rainforest Payment Component</h3>
            {/* The Web Component */}
            <rainforest-payment
                ref={componentRef}
                session-key={sessionKey}
                payin-config-id={payinConfigId}
            ></rainforest-payment>
        </div>
    );
};

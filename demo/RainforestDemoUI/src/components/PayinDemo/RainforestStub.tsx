import React, { useEffect, useRef } from 'react';

interface RainforestStubProps {
    sessionKey: string;
}

export const RainforestStub: React.FC<RainforestStubProps> = ({ sessionKey }) => {
    const mountRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (mountRef.current) {
            // In a real integration, we would mount the Rainforest component here.
            // e.g. Rainforest.mount(mountRef.current, { sessionKey });
            console.log('Mounting Rainforest component with sessionKey:', sessionKey);
        }
    }, [sessionKey]);

    return (
        <div className="card" style={{ border: '2px dashed var(--primary-color)', minHeight: '200px', display: 'flex', alignItems: 'center', justifyContent: 'center', flexDirection: 'column' }}>
            <h3>Rainforest Component Mount Area</h3>
            <p style={{ opacity: 0.7 }}>The payment form would appear here.</p>
            <div style={{ marginTop: '1rem', padding: '1rem', background: '#000', borderRadius: '4px', maxWidth: '80%' }}>
                <code>sessionKey: {sessionKey.substring(0, 10)}...</code>
            </div>
            <div ref={mountRef} />
        </div>
    );
};

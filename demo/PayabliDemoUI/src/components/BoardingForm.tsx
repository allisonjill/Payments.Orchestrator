import React, { useState } from 'react';
import { boardingApi } from '../api/boardingApi';
import type { PayabliBoardingAppRequest } from '../types';

interface BoardingFormProps {
  onSuccess: (appId: number, email: string) => void;
  orgId: number;
}

export const BoardingForm: React.FC<BoardingFormProps> = ({ onSuccess, orgId }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const today = new Date().toISOString().split('T')[0];
  const dobFallback = new Date(new Date().setFullYear(new Date().getFullYear() - 30)).toISOString().split('T')[0];

  const [formData, setFormData] = useState({
    dbaname: 'Demo Merchant Store',
    legalname: 'Demo Merchant Store LLC',
    contactName: 'Jane Doe',
    email: 'jane.doe@example.com',
    phone: '9705551212',
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    const payload: PayabliBoardingAppRequest = {
      orgId: orgId,
      templateId: 0,
      dbaname: formData.dbaname,
      legalname: formData.legalname,
      website: "https://example.com",
      ein: "123456789",
      taxfillname: formData.legalname,
      license: "",
      licstate: "",
      startdate: today,
      phonenumber: formData.phone,
      faxnumber: "",
      baddress: "123 Main St",
      baddress1: "",
      bcity: "Fruita",
      btype: "",
      bstate: "CO",
      bzip: "81521",
      bcountry: "US",
      maddress: "123 Main St",
      maddress1: "",
      mcity: "Fruita",
      mstate: "CO",
      mzip: "81521",
      mcountry: "US",
      mcc: "5812",
      bsummary: "Test merchant used for API integration testing",
      whenCharged: "At time of service",
      whenProvided: "Immediately",
      whenDelivered: "Immediately",
      whenRefunded: "Within 7 days",
      binperson: 0,
      binphone: 0,
      binweb: 100,
      avgmonthly: 5000,
      ticketamt: 50,
      highticketamt: 500,
      recipientEmail: formData.email,
      recipientEmailNotification: false,
      resumable: true,
      contacts: [
        {
          contactName: formData.contactName,
          contactEmail: formData.email,
          contactTitle: "Owner",
          contactPhone: formData.phone
        }
      ],
      ownership: [
        {
          ownername: formData.contactName,
          ownertitle: "Owner",
          ownerpercent: 100,
          ownerssn: "111223333",
          ownerdob: dobFallback,
          ownerphone1: formData.phone,
          ownerphone2: "",
          owneremail: formData.email,
          ownerdriver: "",
          odriverstate: "",
          oaddress: "123 Main St",
          ostate: "CO",
          ocountry: "US",
          ocity: "Fruita",
          ozip: "81521"
        }
      ],
      bankData: [
        {
          nickname: "Primary",
          bankName: "Test Bank",
          routingAccount: "021000021",
          accountNumber: "123456789",
          typeAccount: "Checking",
          bankAccountHolderName: formData.legalname,
          bankAccountHolderType: "Business",
          bankAccountFunction: 1
        }
      ],
      services: {
        card: {
          acceptVisa: true,
          acceptMastercard: true,
          acceptDiscover: true,
          acceptAmex: true
        },
        ach: {
          acceptWeb: true,
          acceptPPD: true,
          acceptCCD: true
        }
      },
      signer: {
        name: formData.contactName,
        ssn: "111223333",
        dob: dobFallback,
        phone: formData.phone,
        email: formData.email,
        address: "123 Main St",
        state: "CO",
        country: "US",
        city: "Fruita",
        zip: "81521"
      }
    };

    try {
      const response = await boardingApi.createApp(payload);
      if (response.isSuccess) {
        onSuccess(response.responseData, formData.email);
      } else {
        setError(response.responseText || 'Failed to create application');
      }
    } catch (err: any) {
      setError(err.message || 'An error occurred during boarding');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="glass-card">
      <h2 className="card-title">Create Merchant Application</h2>
      <p style={{ color: 'var(--text-muted)', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
        Enter the initial details required to map the Payabli API board payload.
      </p>
      
      {error && (
        <div className="result-box result-error">
          <strong>Error:</strong> {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="form-row">
          <div className="form-group">
            <label>DBA Name</label>
            <input
              type="text"
              name="dbaname"
              value={formData.dbaname}
              onChange={handleChange}
              placeholder="Store Frontiers"
              required
            />
          </div>
          <div className="form-group">
            <label>Legal Name</label>
            <input
              type="text"
              name="legalname"
              value={formData.legalname}
              onChange={handleChange}
              placeholder="Store Frontiers LLC"
              required
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Owner Full Name</label>
            <input
              type="text"
              name="contactName"
              value={formData.contactName}
              onChange={handleChange}
              placeholder="Jane Doe"
              required
            />
          </div>
          <div className="form-group">
            <label>Phone Number</label>
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              placeholder="9705551212"
              required
            />
          </div>
        </div>

        <div className="form-group">
          <label>Email Address</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="merchant@example.com"
            required
          />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? (
            <span style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <span className="spinner">⏳</span> Submitting Application...
            </span>
          ) : (
            'Create Payabli Merchant'
          )}
        </button>
      </form>
    </div>
  );
};

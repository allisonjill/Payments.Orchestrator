# Rainforest Connector Demo UI

This is a standalone React application for demonstrating the end-to-end integration between the **Payment Orchestrator Backend** and the **Rainforest Pay Sandbox**.

It allows you to:
1.  **Configure Payment**: Enter merchant details and amounts.
2.  **Create Session**: Call the backend to generate a real Rainforest Session Key.
3.  **Process Payment**: Render the **Actual Rainforest Sandbox Widget** to securely collect card details.
4.  **Simulate Webhooks**: Complete the payment lifecycle by simulating the backend receiving a success event from Rainforest.

## Prerequisites

- **Node.js 18+**
- **Payments Orchestrator Backend** running primarily at `http://localhost:5000` (or `https://localhost:7111`).
  - *Must be configured with a valid Rainforest Sandbox API Key.*
  - *Must allow CORS from `http://localhost:5173` (See `Program.cs`).*

## Setup

1.  **Install Dependencies**
    ```bash
    npm install
    ```

2.  **Configuration**
    Copy `.env.example` to `.env`. Ensure the URL points to your running backend:
    ```
    VITE_ORCHESTRATOR_BASE_URL=http://localhost:5000
    ```

## Running the Demo

1.  Start the development server:
    ```bash
    npm run dev
    ```
2.  Open **[http://localhost:5173](http://localhost:5173)** in your browser.

## Step-by-Step Usage Guide

### 1. The Payin Flow
1.  Go to the **"Payin Demo"** tab.
2.  The form prefills with a valid Sandbox Merchant ID (e.g., `sbx_mid_...`).
3.  Click **"Create Session"**.
4.  Wait for the **"Rainforest Payment Component"** card to load.
    - *It will dynamically fetch the Rainforest SDK from `static.rainforestpay.com`.*
    - *It will render a secure credit card iframe.*

### 2. Processing a Payment
Use one of the following Test Cards in the form:

| Card Brand | Test Number | Exp | CVC | Result |
| :--- | :--- | :--- | :--- | :--- |
| **Visa** | `4242 4242 4242 4242` | Any Future | Any 3-digit | **Approved** |
| **Mastercard** | `5555 5555 5555 4444` | Any Future | Any 3-digit | **Approved** |
| **Generates Error** | `4000 0000 0000 0099` | Any Future | Any 3-digit | **Declined** |

Click the **"Pay"** button inside the widget. You should see a browser alert validating the result (e.g., "Payment Approved!").

### 3. The Webhook Loop (The "Status Update")
Since your localhost is not accessible by the public internet, Rainforest cannot send the *real* `payin.succeeded` webhook to your backend. We simulate this final step:

1.  Look at the **Payment Status** badge at the top of the page (likely "Pending").
2.  Switch to the **"Webhook Simulator"** tab.
3.  Select Event Type: **`payin.succeeded`**.
4.  Click **"Send Webhook"**.
5.  Watch the top badge flip to **"Succeeded"**.

This confirms your backend correctly processed the event and pushed the update to the UI.

## Troubleshooting

### "Rainforest Payment Component - Missing required configuration"
ensure your backend is returning both a `sessionKey` AND a `payinConfigId`. The component requires both attributes to initialize.

### "Network Error" or "Failed to create session"
- Verify your backend API is running.
- Check the backend console for CORS errors.
- Ensure the `merchantId` in the form exists in your backend's `merchants.json` or database.

### "Payment Error" Alert
If the widget shows an error alert when clicking Pay, check the browser console (`F12`). It often means the session has expired (sessions last ~1 hour) or the test card data triggered a specific decline rule.

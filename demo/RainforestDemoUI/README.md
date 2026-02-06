# Rainforest Connector Demo UI

This is a standalone React application for demonstrating the end-to-end integration between the **Payment Orchestrator Backend** and the **Rainforest Pay Sandbox**.

## 🚀 Getting Started (New Machine Setup)

Because this repository does not check in secrets, you must perform the following setup steps when pulling this code to a new machine.

### 1. Backend Setup (Restore Secrets)
The backend requires your Rainforest Sandbox API Key to function.

1.  Navigate to `src/Payments.Orchestrator.Api/`.
2.  Create a new file named `appsettings.Development.json`.
3.  Paste the following configuration (replace `YOUR_*` with your actual keys from the Rainforest Portal):
    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "Rainforest": {
        "BaseUrl": "https://api.sandbox.rainforestpay.com/v1/",
        "ApiKey": "YOUR_SBX_API_KEY_HERE",
        "ApiVersion": "2024-10-16",
        "DefaultSessionTtlSeconds": 3600
      }
    }
    ```
4.  Run the backend:
    ```bash
    dotnet run --project src/Payments.Orchestrator.Api/Payments.Orchestrator.Api.csproj
    ```

### 2. Frontend Setup
1.  Navigate to the demo folder: `cd demo/RainforestDemoUI`.
2.  Install dependencies:
    ```bash
    npm install
    ```
3.  Create a `.env` file in this folder:
    ```
    VITE_ORCHESTRATOR_BASE_URL=http://localhost:5000
    ```
4.  Start the UI:
    ```bash
    npm run dev
    ```

---

## 📖 Usage Guide

### 1. The Payin Flow
1.  Open [http://localhost:5173](http://localhost:5173).
2.  Go to the **"Payin Demo"** tab.
3.  **Merchant ID**: You must provide a valid Sandbox Merchant ID.
    -   *Example*: `sbx_mid_2vYF6MAxOrjH2m8snaatVa3x2xZ`
4.  Click **"Create Session"**.
5.  The **Rainforest Payment Component** will load securely.

### 2. Test Cards (Sandbox)
Use these cards to test different outcomes:

| Brand | Test Number | Exp | CVC | Result |
| :--- | :--- | :--- | :--- | :--- |
| **Visa** | `4242 4242 4242 4242` | Any Future | Any 3-digit | **Approved** |
| **Mastercard** | `5555 5555 5555 4444` | Any Future | Any 3-digit | **Approved** |
| **Decline** | `4000 0000 0000 0099` | Any Future | Any 3-digit | **Declined** |

### 3. Webhook Simulation
Since `localhost` is not public, Rainforest cannot send real webhooks to your dev machine by default.
1.  Complete a payment (Status remains "Pending" in UI).
2.  Go to **"Webhook Simulator"**.
3.  Select **`payin.succeeded`**.
4.  Click **"Send Webhook"**.
5.  Observe the status badge update to **"Succeeded"**.

---

## 🪝 Real Webhooks with Hookdeck
To receive **live** webhooks from Rainforest during development:

1.  **Install Hookdeck CLI**:
    ```bash
    npm install hookdeck-cli -g
    ```
2.  **Start Forwarding**:
    ```bash
    hookdeck listen 5000 POST --path /api/webhooks/rainforest
    ```
    *This creates a public URL that forwards to your localhost.*
3.  **Configure Rainforest**:
    -   Copy the **Webhook URL** from the Hookdeck CLI output.
    -   Log in to **Rainforest Sandbox Portal > Settings > Webhooks**.
    -   Add a new endpoint with that URL.
4.  **Test**: Now, completing a real payment will automatically update the status in your UI!

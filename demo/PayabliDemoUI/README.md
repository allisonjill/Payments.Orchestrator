# Payabli Orchestrator Demo UI

This application is an interactive demo of the Payments Orchestrator API for Payabli (Sandbox).

## What Was Added (Merchant Boarding Flow)
We extended the existing payment flow (Auth/Capture, Void, Refund) to additionally support the *end-to-end Merchant Boarding Flow*. 

* **Backend Integration**: Configured `PayabliClient` and API routes to process requests for `POST Boarding/app`, `PUT Boarding/applink`, and `GET Boarding/read`.
* **Frontend Integration**: Implemented a React-based generic user entry form capturing Payabli business/owner metrics and translating them to the required API JSON schema.
* **Architecture**: The UI contains a structured tab navigation dividing normal Payment interactions from the new Merchant Onboarding pipeline.

## Required Environment Variables & Credentials
To run this demo properly, no frontend `.env` changes are required since it relies on the backend API.
However, you MUST plug your sandbox credentials into the backend's `appsettings.Development.json` file inside `src/Payments.Orchestrator.Api`:

```json
{
  "Payabli": {
    "BaseUrl": "https://api-sandbox.payabli.com/api/",
    "ApiKey": "YOUR_SANDBOX_TOKEN",
    "MerchantId": "Your_Testing_Merchant_Or_App_Id"
  }
}
```

## How to Run the Demo Flow
1. **Start the API Server**:
   Navigate to `src/Payments.Orchestrator.Api` and run `dotnet run`. It should listen on `http://localhost:5252`.
2. **Start the Frontend UI**:
   Navigate to `demo/PayabliDemoUI` and run `npm run dev`.
3. **Using the UI**:
   * Click on the **Merchant Boarding** tab.
   * Fill out the testing context fields (DBA Name, Legal Name, Contact, Email, Phone).
   * Click **Create Merchant Application**.
   * Upon success, the UI will retain the generated `App ID` and display the **Merchant Boarding Status** panel.
   * Click **Generate Hosted Boarding Link** to trigger the applink creation route.
   * Click **Launch Hosted Boarding 🚀** to open Payabli's interactive application UI in a new tab.

## Assumptions Made from Docs
* We assume the default Sandbox `orgId` of `7158` (as specified in the prompt's provided curl scripts) is correct for boarding generation.
* The structure mandates a deeply nested onboarding JSON. The React Form only exposes ~5 fields and dynamically maps them across the complex JSON requirements to maintain speed/demo-friendliness without breaking the actual Payabli JSON validation on their end.
* Local state management (`localStorage`) is used to store `App ID` and `Email` mid-boarding to mimic a seamless database return without standing up a persistent storage DB solely for the demo UI.

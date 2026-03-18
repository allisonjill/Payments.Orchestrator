#!/bin/bash

# ----------------------------------------------------------------------------------
# Payabli Payment Flow Example (Authorize & Capture)
# ----------------------------------------------------------------------------------
# NOTE: In Rainforest, the Payin session / charge creates a distinct intent or object.
# In Payabli, standard Authorization & Capture flow handles the full transaction against
# your specific allocated Merchant ID (Paypoint).
# ----------------------------------------------------------------------------------

API_TOKEN="YOUR_PAYABLI_API_TOKEN"
MERCHANT_ID="8675309"

echo "Running Auth & Capture against Merchant: $MERCHANT_ID..."

# Make the payment request
# This uses the specific "auth_capture" transaction type from the payload
response=$(curl -s -X POST "https://api-sandbox.payabli.com/api/Transaction" \
  -H "requestToken: $API_TOKEN" \
  -H "Content-Type: application/json" \
  -d @payabli_payment_payload.json)

echo "Response raw:"
echo "$response" | jq .

# Extract Payabli responseCode
responseCode=$(echo "$response" | jq -r '.responseCode')
responseMessage=$(echo "$response" | jq -r '.responseMessage')

# Check if responseCode is '00' or '0' indicating success.
if [ "$responseCode" == "00" ] || [ "$responseCode" == "0" ]; then
    transactionId=$(echo "$response" | jq -r '.responseData.transactionId')
    echo "✅ Success! Transaction ID: $transactionId"
else
    echo "❌ Failed to authorize transaction. Reason: $responseMessage"
    exit 1
fi

echo ""
echo "---------------------------------------------------------"
echo "2. Testing Void Transaction ($transactionId)"
echo "---------------------------------------------------------"

voidPayload=$(cat <<EOF
{
  "merchantId": "$MERCHANT_ID",
  "transactionType": "void",
  "transactionId": "$transactionId"
}
EOF
)

voidResponse=$(curl -s -X POST "https://api-sandbox.payabli.com/api/Transaction" \
  -H "requestToken: $API_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$voidPayload")

echo "Void Response raw:"
echo "$voidResponse" | jq .

voidCode=$(echo "$voidResponse" | jq -r '.responseCode')
if [ "$voidCode" == "00" ] || [ "$voidCode" == "0" ]; then
    echo "✅ Success! Transaction successfully voided."
else
    echo "❌ Failed to void transaction."
fi

echo ""
echo "---------------------------------------------------------"
echo "3. Testing Refund Transaction ($transactionId)"
echo "---------------------------------------------------------"

refundPayload=$(cat <<EOF
{
  "merchantId": "$MERCHANT_ID",
  "transactionType": "refund",
  "amount": 10.50,
  "transactionId": "$transactionId"
}
EOF
)

refundResponse=$(curl -s -X POST "https://api-sandbox.payabli.com/api/Transaction" \
  -H "requestToken: $API_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$refundPayload")

echo "Refund Response raw:"
echo "$refundResponse" | jq .

refundCode=$(echo "$refundResponse" | jq -r '.responseCode')
if [ "$refundCode" == "00" ] || [ "$refundCode" == "0" ]; then
    echo "✅ Success! Transaction successfully refunded."
else
    echo "❌ Failed to refund transaction. (Note: standard testing might fail here if the Void already succeeded, or if the batch hasn't settled. This is expected behavior for settled vs unsettled transactions.)"
fi

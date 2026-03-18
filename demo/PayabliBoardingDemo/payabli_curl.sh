#!/bin/bash

# ----------------------------------------------------------------------------------
# Payabli Merchant Boarding Flow Example
# ----------------------------------------------------------------------------------
# NOTE: In our old Rainforest setup, calling "create payfac" would instantly
# provision the merchant paypoint. In Payabli, this endpoint Submits a Boarding
# Application first. The actual paypoint is created AFTER underwriting and approval.
# ----------------------------------------------------------------------------------

API_TOKEN="YOUR_PAYABLI_API_TOKEN"
appId=""

# 1. Create the Boarding Application
echo "1. Creating Boarding Application..."
response=$(curl -s -X POST "https://api-sandbox.payabli.com/api/Boarding/app" \
  -H "requestToken: $API_TOKEN" \
  -H "Content-Type: application/json" \
  -d @payabli_boarding_payload.json)

echo "Response: $response"

# Extract appId from the response (assuming it's in the responseData)
# Requires jq installed for parsing
appId=$(echo "$response" | jq -r '.responseData')

if [ -z "$appId" ] || [ "$appId" == "null" ]; then
  echo "Failed to get appId from the response. Ensure your token and payload are correct."
  exit 1
fi

echo "Successfully captured appId: $appId"

# 2. Create the Boarding Link using the captured appId
# Here we URL-encode the email address
EMAIL="test@example.com"
URL_ENCODED_EMAIL="test%40example.com"

echo "2. Creating Boarding Link for $EMAIL..."
linkResponse=$(curl -s -X PUT "https://api-sandbox.payabli.com/api/Boarding/applink/${appId}/${URL_ENCODED_EMAIL}" \
  -H "requestToken: $API_TOKEN" \
  -H "Content-Type: application/json")

echo "Boarding Link Response: $linkResponse"

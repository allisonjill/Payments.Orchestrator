export interface PayabliPaymentMethod {
  cardNumber: string;
  expirationDate: string;
  cvv2: string;
}

export interface PayabliCustomerData {
  firstName: string;
  lastName: string;
  address1: string;
  city: string;
  state: string;
  zip: string;
  country: string;
}

export interface PayabliPaymentDetails {
  amount: number;
  surchargeAmount?: number;
  convenienceFeeAmount?: number;
}

export interface PayabliAuthCaptureRequest {
  entryPoint: string;
  paymentMethod: PayabliPaymentMethod;
  paymentDetails: PayabliPaymentDetails;
  customerData: PayabliCustomerData;
}

export interface PayabliVoidRequest {
  transactionId: string;
}

export interface PayabliRefundRequest {
  amount: number;
  transactionId: string;
}

export interface PayabliTransactionResponseData {
  transId: string;
  authCode: string;
}

export interface PayabliBaseResponse {
  responseCode: string;
  responseMessage: string;
  responseData: PayabliTransactionResponseData;
  isSuccess: boolean;
}

export interface PayabliBoardingContact {
  contactName: string;
  contactEmail: string;
  contactTitle: string;
  contactPhone: string;
}

export interface PayabliBoardingOwnership {
  ownername: string;
  ownertitle: string;
  ownerpercent: number;
  ownerssn: string;
  ownerdob: string;
  ownerphone1: string;
  ownerphone2: string;
  owneremail: string;
  ownerdriver: string;
  odriverstate: string;
  oaddress: string;
  ostate: string;
  ocountry: string;
  ocity: string;
  ozip: string;
}

export interface PayabliBoardingBankData {
  nickname: string;
  bankName: string;
  routingAccount: string;
  accountNumber: string;
  typeAccount: string;
  bankAccountHolderName: string;
  bankAccountHolderType: string;
  bankAccountFunction: number;
}

export interface PayabliBoardingCardServices {
  acceptVisa: boolean;
  acceptMastercard: boolean;
  acceptDiscover: boolean;
  acceptAmex: boolean;
}

export interface PayabliBoardingAchServices {
  acceptWeb: boolean;
  acceptPPD: boolean;
  acceptCCD: boolean;
}

export interface PayabliBoardingServices {
  card: PayabliBoardingCardServices;
  ach: PayabliBoardingAchServices;
}

export interface PayabliBoardingSigner {
  name: string;
  ssn: string;
  dob: string;
  phone: string;
  email: string;
  address: string;
  state: string;
  country: string;
  city: string;
  zip: string;
}

export interface PayabliBoardingAppRequest {
  orgId: number;
  templateId: number;
  dbaname: string;
  legalname: string;
  website: string;
  ein: string;
  taxfillname: string;
  license: string;
  licstate: string;
  startdate: string;
  phonenumber: string;
  faxnumber: string;
  baddress: string;
  baddress1: string;
  bcity: string;
  btype: string;
  bstate: string;
  bzip: string;
  bcountry: string;
  maddress: string;
  maddress1: string;
  mcity: string;
  mstate: string;
  mzip: string;
  mcountry: string;
  mcc: string;
  bsummary: string;
  whenCharged: string;
  whenProvided: string;
  whenDelivered: string;
  whenRefunded: string;
  binperson: number;
  binphone: number;
  binweb: number;
  avgmonthly: number;
  ticketamt: number;
  highticketamt: number;
  recipientEmail: string;
  recipientEmailNotification: boolean;
  resumable: boolean;
  contacts: PayabliBoardingContact[];
  ownership: PayabliBoardingOwnership[];
  bankData: PayabliBoardingBankData[];
  services: PayabliBoardingServices;
  signer: PayabliBoardingSigner;
}

export interface PayabliQueryPaypointsResponse {
  isSuccess: boolean;
  responseText: string;
  responseCode: number;
  totalRecords: number;
}

export interface PayabliBoardingAppResponse {
  isSuccess: boolean;
  responseText: string;
  responseCode: number;
  responseData: number; // appId
}

export interface PayabliAppLinkData {
  appLink: string;
  referenceId: string;
}

export interface PayabliAppLinkResponse {
  responseCode: number;
  pageIdentifier?: string;
  roomId: number;
  isSuccess: boolean;
  responseText: string;
  responseData: PayabliAppLinkData;
}

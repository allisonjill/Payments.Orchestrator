using System.Text.Json.Serialization;

namespace Payments.Orchestrator.Api.PayabliConnector.DTOs;

public class PayabliPaymentMethod
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string Cvv2 { get; set; } = string.Empty;
}

public class PayabliCustomerData
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class PayabliPaymentDetails
{
    public decimal Amount { get; set; }
    public decimal? SurchargeAmount { get; set; }
    public decimal? ConvenienceFeeAmount { get; set; }
}

public class PayabliAuthCaptureRequest
{
    public string EntryPoint { get; set; } = string.Empty;
    public PayabliPaymentMethod PaymentMethod { get; set; } = new();
    public PayabliPaymentDetails PaymentDetails { get; set; } = new();
    public PayabliCustomerData CustomerData { get; set; } = new();
}

public class PayabliVoidRequest
{
    public string TransactionId { get; set; } = string.Empty;
}

public class PayabliRefundRequest
{
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}

public class PayabliTransactionResponseData
{
    public string TransId { get; set; } = string.Empty;
    public string AuthCode { get; set; } = string.Empty;
}

public class PayabliBaseResponse
{
    public string ResponseCode { get; set; } = string.Empty;
    public string ResponseMessage { get; set; } = string.Empty;
    public PayabliTransactionResponseData ResponseData { get; set; } = new();
    
    [JsonIgnore]
    public bool IsSuccess => ResponseCode == "00" || ResponseCode == "0";
}

public class PayabliAppLinkData
{
    public string AppLink { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
}

public class PayabliAppLinkResponse
{
    public int ResponseCode { get; set; }
    public string? PageIdentifier { get; set; }
    public int RoomId { get; set; }
    public bool IsSuccess { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public PayabliAppLinkData ResponseData { get; set; } = new();
}

public class PayabliBoardingContact
{
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}

public class PayabliBoardingOwnership
{
    public string Ownername { get; set; } = string.Empty;
    public string Ownertitle { get; set; } = string.Empty;
    public int Ownerpercent { get; set; }
    public string Ownerssn { get; set; } = string.Empty;
    public string Ownerdob { get; set; } = string.Empty;
    public string Ownerphone1 { get; set; } = string.Empty;
    public string Ownerphone2 { get; set; } = string.Empty;
    public string Owneremail { get; set; } = string.Empty;
    public string Ownerdriver { get; set; } = string.Empty;
    public string Odriverstate { get; set; } = string.Empty;
    public string Oaddress { get; set; } = string.Empty;
    public string Ostate { get; set; } = string.Empty;
    public string Ocountry { get; set; } = string.Empty;
    public string Ocity { get; set; } = string.Empty;
    public string Ozip { get; set; } = string.Empty;
}

public class PayabliBoardingBankData
{
    public string Nickname { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string RoutingAccount { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string TypeAccount { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
    public string BankAccountHolderType { get; set; } = string.Empty;
    public int BankAccountFunction { get; set; }
}

public class PayabliBoardingCardServices
{
    public bool AcceptVisa { get; set; }
    public bool AcceptMastercard { get; set; }
    public bool AcceptDiscover { get; set; }
    public bool AcceptAmex { get; set; }
}

public class PayabliBoardingAchServices
{
    public bool AcceptWeb { get; set; }
    public bool AcceptPPD { get; set; }
    public bool AcceptCCD { get; set; }
}

public class PayabliBoardingServices
{
    public PayabliBoardingCardServices Card { get; set; } = new();
    public PayabliBoardingAchServices Ach { get; set; } = new();
}

public class PayabliBoardingSigner
{
    public string Name { get; set; } = string.Empty;
    public string Ssn { get; set; } = string.Empty;
    public string Dob { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}

public class PayabliBoardingAppRequest
{
    public int OrgId { get; set; }
    public int TemplateId { get; set; }
    public string Dbaname { get; set; } = string.Empty;
    public string Legalname { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Ein { get; set; } = string.Empty;
    public string Taxfillname { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string Licstate { get; set; } = string.Empty;
    public string Startdate { get; set; } = string.Empty;
    public string Phonenumber { get; set; } = string.Empty;
    public string Faxnumber { get; set; } = string.Empty;
    public string Baddress { get; set; } = string.Empty;
    public string Baddress1 { get; set; } = string.Empty;
    public string Bcity { get; set; } = string.Empty;
    public string Btype { get; set; } = string.Empty;
    public string Bstate { get; set; } = string.Empty;
    public string Bzip { get; set; } = string.Empty;
    public string Bcountry { get; set; } = string.Empty;
    public string Maddress { get; set; } = string.Empty;
    public string Maddress1 { get; set; } = string.Empty;
    public string Mcity { get; set; } = string.Empty;
    public string Mstate { get; set; } = string.Empty;
    public string Mzip { get; set; } = string.Empty;
    public string Mcountry { get; set; } = string.Empty;
    public string Mcc { get; set; } = string.Empty;
    public string Bsummary { get; set; } = string.Empty;
    public string WhenCharged { get; set; } = string.Empty;
    public string WhenProvided { get; set; } = string.Empty;
    public string WhenDelivered { get; set; } = string.Empty;
    public string WhenRefunded { get; set; } = string.Empty;
    public int Binperson { get; set; }
    public int Binphone { get; set; }
    public int Binweb { get; set; }
    public int Avgmonthly { get; set; }
    public int Ticketamt { get; set; }
    public int Highticketamt { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public bool RecipientEmailNotification { get; set; }
    public bool Resumable { get; set; }
    
    public List<PayabliBoardingContact> Contacts { get; set; } = new();
    public List<PayabliBoardingOwnership> Ownership { get; set; } = new();
    public List<PayabliBoardingBankData> BankData { get; set; } = new();
    public PayabliBoardingServices Services { get; set; } = new();
    public PayabliBoardingSigner Signer { get; set; } = new();
}

public class PayabliBoardingAppResponse
{
    public bool IsSuccess { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public int ResponseData { get; set; } // appId
}

public class PayabliQueryPaypointsResponse
{
    public bool IsSuccess { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public int TotalRecords { get; set; }
    // ResponseData usually contains list of paypoints, but we mainly care about count for this demo
}

// Simple read app shape since we might just want to see it exists or return dynamic
public class PayabliReadAppResponse
{
    public bool IsSuccess { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public object? ResponseData { get; set; }
}

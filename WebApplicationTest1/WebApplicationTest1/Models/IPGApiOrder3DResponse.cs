using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebApplicationTest1.Models
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeFDCC
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public BodyFDCC Body { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class BodyFDCC
    {
        [XmlElement(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public IPGApiOrderFDCCResponse IPGApiOrderResponse { get; set; }

        [XmlElement(ElementName = "IPGApiActionResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public IPGApiActionFDCCResponse IPGApiActionResponse { get; set; }

        [XmlElement(ElementName = "Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public FaultFDCC Fault { get; set; }
    }

    [XmlRoot(ElementName = "IPGApiActionResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
    public class IPGApiActionFDCCResponse
    {
        [XmlElement(ElementName = "successfully", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string Successfully { get; set; }

        [XmlElement(ElementName = "OrderId", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string OrderId { get; set; }

        [XmlElement(ElementName = "TransactionValues", Namespace = "http://ipg-online.com/ipgapi/schemas/a1")]
        public List<TransactionValuesFDCC> TransactionValues { get; set; }
    }

    [XmlRoot(ElementName = "CreditCardTxType", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class CreditCardTxTypeFDCC
    {
        [XmlElement(ElementName = "Type", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string Type { get; set; }
    }
    [XmlRoot(ElementName = "Payment", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class PaymentFDCC
    {
        [XmlElement(ElementName = "ChargeTotal", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string ChargeTotal { get; set; }
        [XmlElement(ElementName = "Currency", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string Currency { get; set; }
    }

    [XmlRoot(ElementName = "TransactionDetails", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class TransactionDetailsFDCC
    {
        [XmlElement(ElementName = "OrderId", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string OrderId { get; set; }
        [XmlElement(ElementName = "TDate", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string TDate { get; set; }
        [XmlElement(ElementName = "TransactionOrigin", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string TransactionOrigin { get; set; }
    }

    [XmlRoot(ElementName = "TransactionValues", Namespace = "http://ipg-online.com/ipgapi/schemas/a1")]
    public class TransactionValuesFDCC
    {
        [XmlElement(ElementName = "CreditCardTxType", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public CreditCardTxTypeFDCC CreditCardTxType { get; set; }
        
        [XmlElement(ElementName = "Payment", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public PaymentFDCC Payment { get; set; }

        [XmlElement(ElementName = "TransactionDetails", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public TransactionDetailsFDCC TransactionDetails { get; set; }

        [XmlElement(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public IPGApiOrderFDCCResponse IPGApiOrderResponse { get; set; }

        [XmlElement(ElementName = "Brand", Namespace = "http://ipg-online.com/ipgapi/schemas/a1")]
        public string Brand { get; set; }

        [XmlElement(ElementName = "TransactionType", Namespace = "http://ipg-online.com/ipgapi/schemas/a1")]
        public string TransactionType { get; set; }

        [XmlElement(ElementName = "TransactionState", Namespace = "http://ipg-online.com/ipgapi/schemas/a1")]
        public string TransactionState { get; set; }
    }

    [XmlRoot(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
    public class IPGApiOrderFDCCResponse
    {
        [XmlElement(ElementName = "ApprovalCode", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string ApprovalCode { get; set; }

        [XmlElement(ElementName = "Brand", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string Brand { get; set; }

        [XmlElement(ElementName = "Country", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string Country { get; set; }

        [XmlElement(ElementName = "CommercialServiceProvider", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string CommercialServiceProvider { get; set; }

        [XmlElement(ElementName = "OrderId", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string OrderId { get; set; }

        [XmlElement(ElementName = "IpgTransactionId", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string IpgTransactionId { get; set; }

        [XmlElement(ElementName = "PaymentType", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string PaymentType { get; set; }

        [XmlElement(ElementName = "TDate", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string TDate { get; set; }

        [XmlElement(ElementName = "TDateFormatted", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string TDateFormatted { get; set; }

        [XmlElement(ElementName = "TransactionResult", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string TransactionResult { get; set; }

        [XmlElement(ElementName = "TransactionTime", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string TransactionTime { get; set; }

        [XmlElement(ElementName = "Secure3DResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public Secure3DResponse Secure3DResponse { get; set; }

        [XmlElement(ElementName = "ErrorMessage", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public string ErrorMessage { get; set; }
    }

   
    [XmlRoot(ElementName = "Secure3DVerificationResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class Secure3DVerificationResponse
    {
        [XmlElement(ElementName = "VerificationRedirectResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public VerificationRedirectResponse VerificationRedirectResponse { get; set; }
    }

    [XmlRoot(ElementName = "VerificationRedirectResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class VerificationRedirectResponse
    {
        [XmlElement(ElementName = "AcsURL", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string AcsURL { get; set; }
        [XmlElement(ElementName = "PaReq", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string PaReq { get; set; }
        [XmlElement(ElementName = "MD", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string MD { get; set; }
        [XmlElement(ElementName = "TermUrl", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string TermUrl { get; set; }
    }


    [XmlRoot(ElementName = "Secure3DResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
    public class Secure3DResponse
    {
        [XmlElement(ElementName = "Secure3DVerificationResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public Secure3DVerificationResponse Secure3DVerificationResponse { get; set; }

        [XmlElement(ElementName = "ResponseCode3dSecure", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public string ResponseCode3dSecure { get; set; }
    }

    

    [XmlRoot(ElementName = "Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class FaultFDCC
    {
        [XmlElement(ElementName = "faultcode", Namespace = "")]
        public string FaultCode { get; set; }

        [XmlElement(ElementName = "faultstring", Namespace = "")]
        public string FaultString { get; set; }

        [XmlElement(ElementName = "detail", Namespace = "")]
        public FaultFDCCDetail Detail { get; set; }
    }

    [XmlRoot(ElementName = "detail")]
    public class FaultFDCCDetail
    {
        [XmlElement(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public IPGApiOrderFDCCResponse IPGApiOrderResponse { get; set; }
    }

}
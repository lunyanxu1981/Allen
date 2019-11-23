using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using WSIPGClient.WebReference;
using System;
using System.IO;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;
using WSIPGClient.ExtensionMethods;
using WSIPGClient.RequestSamples.Action;
using WSIPGClient.RequestSamples.Order;
using WSIPGClient.Certificate;
using System.Xml.Serialization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Soap;
using System.ServiceModel.Channels;

namespace WSIPGClient
{
    public class ClassA
    {
        public void MethodA()
        {
            ClassB.MethodB();
        }
    }
    public class ClassB
    {
        public static void MethodB()
        {

            var methodName = new StackTrace()?.GetFrame(1)?.GetMethod()?.DeclaringType?.Name ?? string.Empty;
        }
    }

    static class Program
    {
        static String TDate;

        static String OrderId;

        [STAThread]
        static void Main()
        {
            ClassA objA = new ClassA();
            objA.MethodA();
            // Disable Expect100Continue, when set to true I get an error
            // The request was aborted: Could not create SSL/TLS secure channel.
            //ServicePointManager.Expect100Continue = false;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //String RequestResponseMessage = "ABC@travelzoo.com";
            //List<string> toList = new List<string>();
            // toList.Add("abc@travelzoo.com");
            //toList.Add("xyz@db.com");
            //var ary = RequestResponseMessage.Split(',');
            //bool hasMatch = ary.Any(x => toList.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
            //Randomize5DigitNumber();
            //Action
            //RequestResponseMessage = doInitiateClearingActionRequest();
            //RequestResponseMessage = doInquiryOrderActionRequest();
            //RequestResponseMessage = doInquiryOrderActionRequest();


            //Order
            //RequestResponseMessage = doCreditCardTransactionOrderRequest();
            //RequestResponseMessage = doCreatePaymentURLActionRequest();
            //RequestResponseMessage = doValidateOrderRequest();
            //RequestResponseMessage = doRefundOrderRequest();
            //RequestResponseMessage = StringJoinArray();
            //Send3DVerRequest();
            //Console.WriteLine(RequestResponseMessage);
            //Console.ReadLine();
        }

        private static void CartesianJoin2()
        {
            string[] firstList = { "A", "B", "C"};
            string[] secondList = { "1", "2", "3", "4" };
            string[] thirdList = {"W", "X", "Y", "Z" };

            var result = from x in firstList
                         from y in secondList
                         from z in thirdList
                         select x + y + z;
            var result2 = firstList.ToList().Intersect(secondList.ToList());
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
            
            foreach (var item in result2)
            {
                Console.WriteLine(item);
            }

        }

        private static void CartesianJoin()
        {
            List<string[]> strListAry = new List<string[]>
            {
                new string[] { "A", "B", "C" },
                new string[] { "1", "2", "3", "4" },
                new string[] { "W", "X", "Y", "Z" }
            };

            IEnumerable<string> joinList(string[] firstList, string[] secondList)
            {
                var result =
                from x in firstList
                from y in secondList
                select x + y;
                return result;
            }

            string[] joinResult = strListAry[0];
            for (int i = 1; i < strListAry.Count ; ++i)
            {
                joinResult = joinList(joinResult, strListAry[i]).ToArray();
            }

            foreach (var item in joinResult)
            {
                Console.WriteLine(item);
            }
        }

        private static void TestRegEx()
        {
            var when = "May 09 to May 31";
            var str = "May 09 to May 31 Sydney Ticket selling for Sydney Opera House drama from May 09 to May 31.";
            Regex whenRegex = new Regex(when, RegexOptions.IgnoreCase);
            str = whenRegex.Replace(str, string.Empty, 1).Trim();
        }

        private static void Randomize5DigitNumber()
        {
            List<int> result = new List<int>();
            Random rand = new Random();
            int digit = rand.Next(1, 8);
            while (result.Count < 5)
            {
                if (!result.Contains(digit))
                    result.Add(digit);
                digit = rand.Next(1, 8);
            }
            Console.Write(String.Join(string.Empty, result.Select(e => e.ToString())));
        }



        private static string StringJoinArray()
        {
            int[] bookIds = { 1, 2, 3, 4 };
            string Ids = string.Join<int>(",", bookIds);
            string sql = $"delete from XXX where id in ({Ids})";
            return sql;
            
        }


        /// <summary>
        /// Method creates a IPG API Action Request, sends and recieves IPG API Action Response.
        /// </summary>
        /// <param name="oIPGApiActionRequest"></param>
        /// <returns>IPG API Action response as string</returns>
        private static String SendActionRequest(IPGApiActionRequest oIPGApiActionRequest)
        {
            var cert = CertificateHandler.LoadCertificate(@"D:\Work\GitHub\Allen\WSIPGClient\Certificate\WS4700000018._.1.p12", "P2T$u8%Fhm");//tester02@
            //var cert = CertificateHandler.LoadCertificate(@"C:\cert\ci\WS330995000._.1.p12", "tester02@");
            IPGApiOrderService oIPGApiOrderService = new IPGApiOrderService();
            String RequestResponseMessage = "";
            if (cert != null)
            {
                oIPGApiOrderService.ClientCertificates.Add(cert);
                
                oIPGApiOrderService.Url = @"https://test.ipg-online.com/ipgapi/services";
                // oIPGApiOrderService.Url = @"https://www4.ipg-online.com:443/ipgapi/services";  // Prod URL
                NetworkCredential nc = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");
                // NetworkCredential nc = new NetworkCredential("WS330995000._.1", "tester02@");
                oIPGApiOrderService.Credentials = nc;

               
               
                try
                {
                    //send action request and get response
                    IPGApiActionResponse oResponse = oIPGApiOrderService.IPGApiAction(oIPGApiActionRequest);
                    RequestResponseMessage = oResponse.IPGApiActionResponseToString() ?? "";
                }
                catch (SoapException se)
                {//SoapException: MerchantException or ProcessingException
                    RequestResponseMessage = se.SoapExceptionResponseToString() ?? "";
                }
                catch (Exception e)
                {
                    RequestResponseMessage = e.Message + Environment.NewLine;
                    RequestResponseMessage += e.InnerException + Environment.NewLine;
                    RequestResponseMessage += e.StackTrace;
                }
            }
            return RequestResponseMessage;
        }

        /// <summary>
        /// Method creates IPGAPI Order request, sends it and recieves IPG API Order Response
        /// </summary>
        /// <param name="oIPGApiOrderRequest"></param>
        /// <returns>IPG API Order Response as string</returns>
        private static String SendOrderRequest(IPGApiOrderRequest oIPGApiOrderRequest)
        {
            var cert = CertificateHandler.LoadCertificate(@"D:\Work\GitHub\Allen\WSIPGClient\Certificate\WS4700000018._.1.p12", "P2T$u8%Fhm");//tester02@


            IPGApiOrderService oIPGApiOrderService = new IPGApiOrderService();
            String RequestResponseMessage = "";
            if (cert != null)
            {
                oIPGApiOrderService.ClientCertificates.Add(cert);
                oIPGApiOrderService.Url = @"https://test.ipg-online.com/ipgapi/services";
                // oIPGApiOrderService.Url = @"https://www4.ipg-online.com:443/ipgapi/services";  // Prod URL

                //NetworkCredential nc = new NetworkCredential("tester02@", "tester02@");
                NetworkCredential nc = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");
                oIPGApiOrderService.Credentials = nc;

             
                try
                {
                    //send action request and get response
                    IPGApiOrderResponse oResponse = oIPGApiOrderService.IPGApiOrder(oIPGApiOrderRequest);
                    TDate = oResponse.TDate;
                    OrderId = oResponse.OrderId;
                    RequestResponseMessage = oResponse.IPGApiOrderResponseToString() ?? "";
                }
                catch (SoapException se)
                {//SoapException: MerchantException or ProcessingException
                    RequestResponseMessage = se.SoapExceptionResponseToString() ?? "";
                }
                catch (Exception e)
                {
                    RequestResponseMessage = e.Message + Environment.NewLine;
                    RequestResponseMessage += e.InnerException + Environment.NewLine;
                    RequestResponseMessage += e.StackTrace;
                }
            }
            return RequestResponseMessage;
        }

        /// <summary>
        /// Method creates an Initiate Clearing Action Request
        /// </summary>
        /// <returns></returns>
        private static String doInitiateClearingActionRequest()
        {
            //InitiateClearing
            InitiateClearingRequest oInitiateClearingRequest = new InitiateClearingRequest();
            IPGApiActionRequest oIPGApiActionRequest = oInitiateClearingRequest.InitiateClearingActionRequest;
            return SendActionRequest(oIPGApiActionRequest);
        }

        /// <summary>
        /// Method creates Inquiry Order Action Request
        /// </summary>
        /// <returns></returns>
        private static String doInquiryOrderActionRequest()
        {
            //InquiryOrder
            InquiryOrderRequest oInquiryOrderRequest = new InquiryOrderRequest();
            IPGApiActionRequest oIPGApiActionRequest = oInquiryOrderRequest.InquiryOrderActionRequest;
            return SendActionRequest(oIPGApiActionRequest);
        }

      


        /// <summary>
        /// Method returns Create Payment URL Action Request
        /// </summary>
        /// <returns></returns>
        private static String doCreatePaymentURLActionRequest()
        {
            //CreatePaymentURL
            CreatePaymentURLRequest oCreatePaymentURLRequest = new CreatePaymentURLRequest();
            IPGApiActionRequest oIPGApiActionRequest = oCreatePaymentURLRequest.CreatePaymentURLActionRequest;
            return SendActionRequest(oIPGApiActionRequest);
        }

        private static String doValidateOrderRequest()
        {
            //Validate
            ValidateRequest oValidateRequest = new ValidateRequest();
            IPGApiActionRequest oIPGApiActionRequest = oValidateRequest.ValidateActionRequest;
            return SendActionRequest(oIPGApiActionRequest);
        }

        private static String doCreditCardTransactionOrderRequest()
        {
            //CreditCardTransaction
            CreditCardTransactionOrderRequest oCreditCardTransactionOrderRequest = new CreditCardTransactionOrderRequest();
            IPGApiOrderRequest oIPGApiOrderRequest = oCreditCardTransactionOrderRequest.oCreditCardTransactionOrderRequest;
            return SendOrderRequest(oIPGApiOrderRequest);
        }

        private static String doRefundOrderRequest()
        {
            //CreditCardTransaction
            RefundOrderRequest oRefundOrderRequest = new RefundOrderRequest();
            IPGApiOrderRequest oIPGApiOrderRequest = oRefundOrderRequest.oRefundOrderRequest;
            return SendOrderRequest(oIPGApiOrderRequest);
        }

        private static void Send3DAuthRequest(Envelope3D authEnv)
        {
            var url = "https://test.ipg-online.com/ipgapi/services";
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
                <SOAP-ENV:Body>
                <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
                <ns2:Transaction>
                <ns2:CreditCardTxType><ns2:StoreId>4700000054</ns2:StoreId><ns2:Type>payerAuth</ns2:Type></ns2:CreditCardTxType>
                <ns2:CreditCard3DSecure><ns2:Secure3DRequest><ns2:Secure3DAuthenticationRequest><ns2:AcsResponse>
                <ns2:MD>" + authEnv.Body.IPGApiOrderResponse.Secure3DResponse.Secure3DVerificationResponse.VerificationRedirectResponse.MD + @"</ns2:MD>
                <ns2:PaRes>" + authEnv.Body.IPGApiOrderResponse.Secure3DResponse.Secure3DVerificationResponse.VerificationRedirectResponse.PaReq + @"</ns2:PaRes>
                </ns2:AcsResponse></ns2:Secure3DAuthenticationRequest></ns2:Secure3DRequest>
                </ns2:CreditCard3DSecure>
                <ns2:Payment/>
                <ns2:TransactionDetails><ns2:IpgTransactionId>" + authEnv.Body.IPGApiOrderResponse.IpgTransactionId + @"</ns2:IpgTransactionId></ns2:TransactionDetails>
                </ns2:Transaction>
                </ns4:IPGApiOrderRequest>
                </SOAP-ENV:Body>
                </SOAP-ENV:Envelope>");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            var cert = CertificateHandler.LoadCertificate(@"D:\Work\GitHub\Allen\WSIPGClient\Certificate\WS4700000054._.1.p12", "U\"bd?96ZwU");
            webRequest.ClientCertificates.Add(cert);
            NetworkCredential nc = new NetworkCredential("WS4700000054._.1", @"Kn@wM57\Rg");
            webRequest.Credentials = nc;

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    Console.WriteLine(soapResult);
                    
                }
            }
        }

        private static void Send3DVerRequest()
        {
            var url = "https://test.ipg-online.com/ipgapi/services";
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
                <SOAP-ENV:Body>
                <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
                <ns2:Transaction>
                <ns2:CreditCardTxType><ns2:StoreId>4700000054</ns2:StoreId><ns2:Type>payerAuth</ns2:Type></ns2:CreditCardTxType>
                <ns2:CreditCardData><ns2:CardNumber>4035874000424977</ns2:CardNumber><ns2:ExpMonth>12</ns2:ExpMonth><ns2:ExpYear>24</ns2:ExpYear><ns2:CardCodeValue>977</ns2:CardCodeValue></ns2:CreditCardData><ns2:CreditCard3DSecure>
                <ns2:AuthenticateTransaction>true</ns2:AuthenticateTransaction></ns2:CreditCard3DSecure>
                <ns2:Payment><ns2:ChargeTotal>13.99</ns2:ChargeTotal><ns2:Currency>344</ns2:Currency></ns2:Payment>
                </ns2:Transaction>
                </ns4:IPGApiOrderRequest>
                </SOAP-ENV:Body>
                </SOAP-ENV:Envelope>");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            var cert = CertificateHandler.LoadCertificate(@"D:\Work\GitHub\Allen\WSIPGClient\Certificate\WS4700000054._.1.p12", "U\"bd?96ZwU");
            webRequest.ClientCertificates.Add(cert);
            NetworkCredential nc = new NetworkCredential("WS4700000054._.1", @"Kn@wM57\Rg");
            webRequest.Credentials = nc;

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    var obj = SOAPToObject<Envelope3D>(soapResult);
                    Console.WriteLine(soapResult);
                    Send3DAuthRequest(obj);
                }
            }
        }

        public static T SOAPToObject<T>(string SOAP)
        {
            if (string.IsNullOrEmpty(SOAP))
            {
                throw new ArgumentException("SOAP can not be null/empty");
            }
            using (MemoryStream Stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(SOAP)))
            {
                XmlSerializer Formatter = new XmlSerializer(typeof(T));
                return (T)Formatter.Deserialize(Stream);
            }
        }
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

    [XmlRoot(ElementName = "Secure3DVerificationResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
    public class Secure3DVerificationResponse
    {
        [XmlElement(ElementName = "VerificationRedirectResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public VerificationRedirectResponse VerificationRedirectResponse { get; set; }
    }

    [XmlRoot(ElementName = "Secure3DResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
    public class Secure3DResponse
    {
        [XmlElement(ElementName = "Secure3DVerificationResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/v1")]
        public Secure3DVerificationResponse Secure3DVerificationResponse { get; set; }
    }

    [XmlRoot(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
    public class IPGApiOrder3DResponse
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
        [XmlAttribute(AttributeName = "ipgapi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ipgapi { get; set; }
        [XmlAttribute(AttributeName = "a1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string A1 { get; set; }
        [XmlAttribute(AttributeName = "v1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string V1 { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body3D
    {
        [XmlElement(ElementName = "IPGApiOrderResponse", Namespace = "http://ipg-online.com/ipgapi/schemas/ipgapi")]
        public IPGApiOrder3DResponse IPGApiOrderResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope3D
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body3D Body { get; set; }
        [XmlAttribute(AttributeName = "SOAP-ENV", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string SOAPENV { get; set; }
    }
}
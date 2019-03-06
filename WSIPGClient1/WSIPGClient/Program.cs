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

namespace WSIPGClient
{
    static class Program
    {
        static String TDate;

        static String OrderId;
        
        [STAThread]
        static void Main()
        {
            // Disable Expect100Continue, when set to true I get an error
            // The request was aborted: Could not create SSL/TLS secure channel.
            //ServicePointManager.Expect100Continue = false;
            ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
           // if (!DisableTLS12) 
             //   ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            
            String RequestResponseMessage = "";

            //Action
           // RequestResponseMessage = doInitiateClearingActionRequest();
           //RequestResponseMessage = doInquiryOrderActionRequest();


            //Order
            //RequestResponseMessage = doCreditCardTransactionOrderRequest();
            // RequestResponseMessage = doCreatePaymentURLActionRequest();
          //   RequestResponseMessage = doValidateOrderRequest();
          RequestResponseMessage = doRefundOrderRequest();

         
            Console.WriteLine(RequestResponseMessage);
            Console.ReadLine();
        }

        /// <summary>
        /// Method creates a IPG API Action Request, sends and recieves IPG API Action Response.
        /// </summary>
        /// <param name="oIPGApiActionRequest"></param>
        /// <returns>IPG API Action response as string</returns>
        private static String SendActionRequest(IPGApiActionRequest oIPGApiActionRequest)
        {
             var cert = CertificateHandler.LoadCertificate(@"C:\certificates\test\travelxoo\certs\WS4700000018._.1.p12", "P2T$u8%Fhm");
            IPGApiOrderService oIPGApiOrderService = new IPGApiOrderService();
            String RequestResponseMessage = "";
            if (cert != null)
            {
                oIPGApiOrderService.ClientCertificates.Add(cert);
                oIPGApiOrderService.Url = @"https://test.ipg-online.com:443/ipgapi/services";

                NetworkCredential nc = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");
                oIPGApiOrderService.Credentials = nc;

                //set proxy host and port
              IWebProxy webProxy = new WebProxy("fdcproxy.1dc.com", 8080);
              webProxy.Credentials = new NetworkCredential("famb7on", "Rudra$1234");
              oIPGApiOrderService.Proxy = webProxy;
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
            var cert = CertificateHandler.LoadCertificate(@"C:\certificates\test\travelxoo\certs\WS4700000018._.1.p12", "P2T$u8%Fhm");
           IPGApiOrderService oIPGApiOrderService = new IPGApiOrderService();
            String RequestResponseMessage = "";
            if (cert != null)
            {
                oIPGApiOrderService.ClientCertificates.Add(cert);
                oIPGApiOrderService.Url = @"https://test.ipg-online.com:443/ipgapi/services";

                NetworkCredential nc = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");
                oIPGApiOrderService.Credentials = nc;

                //set proxy host and port
               IWebProxy webProxy = new WebProxy("fdcproxy.1dc.com", 8080);
               webProxy.Credentials = new NetworkCredential("famb7on", "Rudra$1234");
               oIPGApiOrderService.Proxy = webProxy;
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
    }
}
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
            ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            String RequestResponseMessage = "ABC@travelzoo.com";
            List<string> toList = new List<string>();
           // toList.Add("abc@travelzoo.com");
            //toList.Add("xyz@db.com");
            var ary = RequestResponseMessage.Split(',');
            bool hasMatch = ary.Any(x => toList.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
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

            Console.WriteLine(RequestResponseMessage);
            Console.ReadLine();
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
    }
}
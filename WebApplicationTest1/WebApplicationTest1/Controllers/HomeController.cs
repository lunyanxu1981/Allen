﻿using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplicationTest1.Models;
using WebApplicationTest1.App_Start;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WebApplicationTest1.IPGWebReference;
using WebApplicationTest1.FirstDataExtentions;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;
using System.Web.Services.Protocols;

namespace WebApplicationTest1.Controllers
{
    public class HomeController : Controller
    {
        public const string WEBAPI_BASEURL= "http://dev.webapi-test.com/api/";
        public async Task<ActionResult> Index()
        {
            var context1 = System.Web.HttpContext.Current;
            var getProduct = GetProduct().ConfigureAwait(false);
            var product = await getProduct;
            var context2 = System.Web.HttpContext.Current;
            var abc = Url.RequestContext;

            //product.PayerAuthHTMLEncrypted = "abc";

            var responseHash = "d20040b4b3b63564c14fb6f17d9829db694b31bfd01b785fd650b1775a221dd4";
            //sharedsecret + approval_code + chargetotal + currency + txndatetime + storename
            var orderstring = $"v14Kx72QxdY:000000:4521240216:PPX :20000182771.001562018:12:24-05:21:124700000018";
            bool validated = FirstDataSHA256HashValidation(responseHash, orderstring);

            var qs = "a=1&b=2&c=3";
            var dic = HttpUtility.ParseQueryString(qs);

            return View(product);
        }

        public ActionResult Encrypt(Models.Product product)
        {
            if (!string.IsNullOrEmpty(product.PayerAuthHTML))
            {
                product.PayerAuthHTMLEncrypted = LightEncryptionHelper.Encrypt(product.PayerAuthHTML);
            }
            ModelState.Clear();
            return View("Index", product);
        }

        public ActionResult Decrypt(Models.Product product)
        {
            if (!string.IsNullOrEmpty(product.PayerAuthHTMLEncrypted))
            {
                product.PayerAuthHTML = LightEncryptionHelper.DecryptUrlEncoded(product.PayerAuthHTMLEncrypted);
            }
            ModelState.Clear();
            return View("Index", product);
        }

        public static bool FirstDataSHA256HashValidation(string hash, string orderStr)
        {
            string calcHash = FirstDataSHA256Encrypt(orderStr);
            return calcHash == hash;
        }

        public static string FirstDataSHA256Encrypt(string orderStr)
        {
            try
            {
                //Convert the created string to its ascii hexadecimal representation.
                byte[] asciiArray = System.Text.Encoding.ASCII.GetBytes(orderStr);
                System.Text.StringBuilder asciiHexStr = new System.Text.StringBuilder();
                for (int i = 0; i < asciiArray.Length; i++)
                {
                    asciiHexStr.Append(asciiArray[i].ToString("x2"));
                }

                using (var hash = System.Security.Cryptography.SHA256.Create())
                {
                    // compute the SHA-256 hash.
                    byte[] data = hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(asciiHexStr.ToString()));

                    System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                    //Format to hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Return the hexadecimal string as SHA-256 sign string.
                    return sBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Models.Product> GetProduct()
        {
            Models.Product result = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WEBAPI_BASEURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("products/GetProduct?id=1").ConfigureAwait(false);
                if (res.IsSuccessStatusCode)
                {
                    var resStr = res.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<Models.Product>(resStr);
                }
                return result;
            }
        }

        private void TestGET(string request)
        {
            using (var client = new HttpClient())
            {
                
            }
        }

        public string getHMAC_MD5(string key, string data)
        {
            byte[] bKey, bData, bRet;
            string ret = "";
            UTF8Encoding encoder = new UTF8Encoding();
            bKey = encoder.GetBytes(key);
            bData = encoder.GetBytes(data);

            HMACMD5 c = new HMACMD5(bKey);
            bRet = c.ComputeHash(bData);
            foreach (byte b in bRet)
            {
                ret += String.Format("{0:x2}", b);
            }

            return ret;
        }

        public ActionResult PayeaseGiftCardRefundInquiry()
        {
            return View("GiftCardRefundInquiry", new Models.VirtualCard()
            {
                CardNo = string.Empty,
                Password = string.Empty,
                Ymd = System.DateTime.Now.ToString("yyyyMMdd"),
                RequesString = string.Empty,
                ResponseString = string.Empty
            });
        }

        public ActionResult PerformPayeaseGiftCardRefundInquiry(VirtualCard virtualCard)
        {
            string privateKey = "75cdf8e01fa354774b1fd7bfa969ff2f";
            string v_mid = "5911";
            string v_mac = getHMAC_MD5(privateKey, $"{v_mid}{virtualCard.Ymd}");
            string requestString = $"https://pay.yizhifubj.com/merchant/ack/virtualcard_refund_list.jsp?v_mid={v_mid}" +
                $"&v_ymd={virtualCard.Ymd}" +
                $"&v_mac={v_mac}";
            HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
            string resonse = r.Get(requestString);
            virtualCard.RequesString = requestString;
            virtualCard.ResponseString = resonse;
            return View("GiftCardRefundInquiry", virtualCard);
        }

        public ActionResult PayeaseGiftCardOrderInquiry()
        {
            return  View("GiftCardOrderInquiry", new Models.VirtualCard() {
                CardNo = string.Empty,
                Password = string.Empty,
                Ymd = System.DateTime.Now.ToString("yyyyMMdd"),
                RequesString = string.Empty,ResponseString=string.Empty });
        }

        public ActionResult PerformPayeaseGiftCardOrderInquiry(VirtualCard virtualCard)
        {
            string privateKey = "75cdf8e01fa354774b1fd7bfa969ff2f";
            string v_mid = "5911";
            string v_mac = getHMAC_MD5(privateKey, $"{v_mid}{virtualCard.Ymd}");
            string requestString = $"https://pay.yizhifubj.com/merchant/ack/virtualcard_order_list.jsp?v_mid={v_mid}" +
                $"&v_ymd={virtualCard.Ymd}" +
                $"&v_mac={v_mac}";
            HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
            string resonse = r.Get(requestString);
            virtualCard.RequesString = requestString;
            virtualCard.ResponseString = resonse;
            return View("GiftCardOrderInquiry", virtualCard);
        }

        public ActionResult PayeaseGiftCardInquiry()
        {
            return View("GiftCardInquiry", new Models.VirtualCard() { CardNo =string.Empty, Password = string.Empty, RequesString = string.Empty, ResponseString = string.Empty});
        }

        public ActionResult PerformPayeaseGiftCardInquiry(Models.VirtualCard virtualCard)
        {
            //string publicKey = "m=b831414e0b4613922bd35b4b36802bc1e1e81c95a27c958f5382003df646154ca92fc1ce02c3be047a45e9b02a9089b4b90278237c965192a0fcc86bb49bc82ae6fdc2de709006b86c7676efdf597626fad633a4f7dc48c445d37eb55fcb3b1abb95baaa826d5390e15fd14ed403fa2d0cb841c650609524ec555e3bc56ca957;e=010001;";
            string privateKey = "75cdf8e01fa354774b1fd7bfa969ff2f";
            //string v_cardpass = string.Empty;
            //string v_cardno = string.Empty;
            string v_mid = "5911";
            string v_mac = getHMAC_MD5(privateKey, $"{v_mid}{virtualCard.CardNo}{virtualCard.Password}");
            string requestString = $"https://pay.yizhifubj.com/merchant/ack/virtualcard_card_list.jsp?v_mid={v_mid}" +
                $"&v_cardno={HttpUtility.UrlEncode(virtualCard.CardNo, Encoding.UTF8)}" +
                $"&v_cardpass={HttpUtility.UrlEncode(virtualCard.Password, Encoding.UTF8)}&v_mac={v_mac}";
            HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
            string resonse = r.Get(requestString);
            virtualCard.RequesString = requestString;
            virtualCard.ResponseString = resonse;
            return View("GiftCardInquiry", virtualCard);
        }

       

        /// <summary>
        /// CalculateXYZ
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "CalculateXYZ";
            ViewBag.ResultText = CalculateXYZ();

            return View();
        }

        /// <summary>
        /// X + Y + Z = 3600
        /// 2X + 3Y + 4Z = 13000
        /// Z = 2200 + X
        /// Y = 1400 - 2X
        /// </summary>
        /// <returns></returns>
        private string CalculateXYZ()
        {
            StringBuilder result = new StringBuilder();

            const int HEAD_COUNT = 3600;
            const int FEET_COUNT = 13000;

            int X = 0;
            Func<int, int> Y = x => 1400 - 2 * x;
            Func<int, int> Z = x => 2200 + x;
            while (Y(X) >= 0)
            {
                if ((X + Y(X) + Z(X)) == HEAD_COUNT && (2 * X + 3 * Y(X) + 4 * Z(X)) == FEET_COUNT)
                {
                    result.AppendLine($"X={X} Y={Y(X)} Z={Z(X)} <br>");
                }

                X++;
            }

            return result.ToString();
        }

        [HttpPost]
        public ActionResult FirstDataFailCallbackTest()
        {
            string htmlStr = string.Empty;
            foreach (var key in Request.Form.AllKeys)
            {
                htmlStr += $"{key}={Request.Form[key]}<br>";
            }
            return Content(htmlStr);
        }

        [HttpPost]
        public ActionResult FirstDataSuccessCallbackTest()
        {
            string htmlStr = string.Empty;
            foreach (var key in Request.Form.AllKeys)
            {
                htmlStr += $"{key}={Request.Form[key]}<br>";
            }
            return Content(htmlStr);
        }

        public ActionResult FirstDataSaleTest()
        {
            string storeName = "4700000018";
            string sharedsecret = "v14Kx72Qxd";
            string url = "https://test.ipg-online.com/connect/gateway/processing";
            if (this.Request.QueryString["isprod"] == "1")
            {
                storeName = "4510118120259";
                sharedsecret = "Br34sHakwc";
                url = "https://www4.ipg-online.com/connect/gateway/processing";
            }
            string orderForm = string.Empty;
            string responseFailURL = "http://dev.webmvc-test.com/Home/FirstDataFailCallbackTest/";
            string responseSuccessURL = "http://dev.webmvc-test.com/Home/FirstDataSuccessCallbackTest/";
            string merchantOrderId = $"{ DateTime.Now.Year.ToString()}{ DateTime.Now.Month.ToString()}{ DateTime.Now.Day.ToString()}{ DateTime.Now.Millisecond.ToString()}";
           
            string trxnDate = DateTime.UtcNow.ToString("yyyy:MM:dd-HH:mm:ss");
            string chargetotal = "10.00";
            string currency = "156";
            
            string sharaw = $"{storeName}{trxnDate}{chargetotal}{currency}{sharedsecret}";
            string sha256HashASCII = SHA256Gen2(sharaw);
            string line1 = $"400005;Voucher;1;{chargetotal}";

            orderForm += $"<form method=\"post\" action=\"{url}\">";
            orderForm += "txntype:<input type=\"input\" name=\"txntype\" value=\"sale\"><br>";
            orderForm += $"storename:<input type=\"input\" name=\"storename\" value=\"{storeName}\"><br>";
            orderForm += $"merchantTransactionId:<input type=\"input\" name=\"merchantTransactionId\" value=\"{merchantOrderId}\"><br>";
            orderForm += $"oid:<input type=\"input\" name=\"oid\" value=\"{merchantOrderId}\"><br>";
            orderForm += "timezone:<input type=\"input\" name=\"timezone\" value=\"GMT\"><br>";
            orderForm += $"txndatetime:<input type=\"input\" name=\"txndatetime\" value=\"{trxnDate}\"><br>";
            orderForm += "paymentMethod:<input type=\"input\" name=\"paymentMethod\" value=\"CUP_domestic\"><br>";
            orderForm += "mobileMode:<input type=\"input\" name=\"mobileMode\" value=\"false\"><br>";
            orderForm += "mode:<input type=\"input\" name=\"mode\" value=\"payonly\"><br>";
            orderForm += "full_bypass:<input type=\"input\" name=\"full_bypass\" value=\"true\"><br>";
            orderForm += "language:<input type=\"input\" name=\"language\" value=\"zh_CN\"><br>";
            orderForm += $"item1:<input type=\"input\" name=\"item1\" value=\"{line1}\"><br>";
            orderForm += $"currency:<input type=\"input\" name=\"currency\" value=\"{currency}\"><br>"; //CNY
            orderForm += $"chargetotal:<input type=\"input\" name=\"chargetotal\" value=\"{chargetotal}\"><br>";
            orderForm += $"customParam_domesticBankId:<input type=\"input\" name=\"customParam_domesticBankId\" value=\"ccb\"><br>";
            orderForm += $"responseFailURL:<input type=\"input\" name=\"responseFailURL\" value=\"{responseFailURL}\"><br>";
            orderForm += $"responseSuccessURL:<input type=\"input\" name=\"responseSuccessURL\" value=\"{responseSuccessURL}\"><br>";
            orderForm += "hash_algorithm:<input type=\"input\" name=\"hash_algorithm\" value=\"SHA256\"><br>";
            orderForm += $"hash:<input type=\"input\" name=\"hash\" value=\"{sha256HashASCII}\"><br>"; //storename, txndatetime, chargetotal, currency and sharedsecret
            orderForm += "<input type=\"submit\" value=\"Submit\" id=\"btnFDSubmit\">";
            orderForm += $"<br><br>SHARaw: {sharaw} <br><br>SHA256Hash_ASCII={sha256HashASCII}<br>";
            //orderForm += "<script type='text/javascript'>window.onload = function() { document.getElementById('btnFDSubmit').click(); };</script>";
            orderForm += "</form>";

            return Content(orderForm);
        }

        private string SHA256Gen2(string raw)
        {
            byte[] asciiArray = System.Text.Encoding.ASCII.GetBytes(raw);
            //ASCII Hex 
            System.Text.StringBuilder asciiHexStr = new System.Text.StringBuilder();


            for (int i = 0; i < asciiArray.Length; i++)
            {
                asciiHexStr.Append(asciiArray[i].ToString("x2"));
            }

            using (var hash = System.Security.Cryptography.SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(asciiHexStr.ToString()));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }


        public ActionResult FirstDataRefund(string id, bool isprod = false)
        {
            ViewBag.Message = "FirstData order refund.";
            if (String.IsNullOrEmpty(id))
                return View();

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            X509Certificate2 certificate = null;
            try
            {
                certificate = isprod
                    ? new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"certs\WS4510118120259._.1.p12"), "s$efm]33PQ")
                    : new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"certs\WS4700000018._.1.p12"), "P2T$u8%Fhm");
            }
            catch { }
            if (certificate != null)
            {
                IPGApiOrderService Request = new IPGApiOrderService();
                Request.ClientCertificates.Add(certificate);
                Request.Url = isprod? @"https://www4.ipg-online.com/ipgapi/services" : @"https://test.ipg-online.com/ipgapi/services";
                Request.Credentials = isprod ? new NetworkCredential("WS4510118120259._.1", "RiS;3X2)gK") : new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");

                IPGApiOrderRequest oRefundOrderRequest = new IPGApiOrderRequest();
                Transaction oTransaction = new Transaction();
                CreditCardTxType oCreditCardTxType = new CreditCardTxType();

                oCreditCardTxType.StoreId = isprod ? "4510118120259" : "4700000018";
                oCreditCardTxType.Type = CreditCardTxTypeType.@return;

                oTransaction.Items = new Object[] { oCreditCardTxType };

                Payment oPayment = new Payment();
                oPayment.SubTotal = 1;
                oPayment.ChargeTotal = 1;
                oPayment.Currency = "156";
                oTransaction.Payment = oPayment;

                TransactionDetails oTransactionDetails = new TransactionDetails();
                oTransactionDetails.OrderId = id;
                oTransaction.TransactionDetails = oTransactionDetails;

                oRefundOrderRequest = new IPGApiOrderRequest();
                oRefundOrderRequest.Item = oTransaction;
                string request = string.Empty;
                try
                {
                    XmlSerializer reqXmlSerializer = new XmlSerializer(oRefundOrderRequest.GetType());
                    
                    using (StringWriter textWriter = new StringWriter())
                    {
                        reqXmlSerializer.Serialize(textWriter, oRefundOrderRequest);
                        request = textWriter.ToString();
                    }

                    IPGApiOrderResponse oResponse = Request.IPGApiOrder(oRefundOrderRequest);



                    XmlSerializer respXmlSerializer = new XmlSerializer(oResponse.GetType());
                    string response = string.Empty;
                    using (StringWriter textWriter = new StringWriter())
                    {
                        respXmlSerializer.Serialize(textWriter, oResponse);
                        response = textWriter.ToString();
                    }
                    ViewBag.RequestString = request;
                    ViewBag.ResponseString = response;



                }
                catch (SoapException se)
                {//SoapException: MerchantException or ProcessingException
                    ViewBag.RequestString = request;
                    ViewBag.ResponseString = se.SoapExceptionResponseToString() ?? "";
                }
                catch (Exception ex)
                {
                    ViewBag.RequestString = request;
                    ViewBag.ResponseString += ex.ToString();
                }
            }

            return View();
        }

        /// <summary>
        /// FirstData Inquiry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult About(string id, bool isprod = false)
        {
            ViewBag.Message = "FirstData order inquiry.";
            if (String.IsNullOrEmpty(id))
                id = "20191288";

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            X509Certificate2 certificate = null;
            try
            {
                certificate = isprod
                    ? new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"certs\WS4510118120259._.1.p12"), "s$efm]33PQ")
                    : new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"certs\WS4700000018._.1.p12"), "P2T$u8%Fhm");
            }
            catch { }

            if (certificate != null)
            {
                IPGApiOrderService Request = new IPGApiOrderService();
                Request.ClientCertificates.Add(certificate);
                Request.Url = isprod ? @"https://www4.ipg-online.com/ipgapi/services" : @"https://test.ipg-online.com/ipgapi/services";
                Request.Credentials = isprod ? new NetworkCredential("WS4510118120259._.1", "RiS;3X2)gK") 
                    : new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");

                InquiryOrder oInquiryOrder = new InquiryOrder()
                {
                    StoreId = isprod ? "4510118120259" : "4700000018",
                    OrderId = id
                };



                IPGWebReference.Action oAction = new IPGWebReference.Action()
                {
                    Item = oInquiryOrder,
                    ClientLocale = new ClientLocale()
                    {
                        Country = "CN",
                        Language = "zh"
                    }
                };

                IPGApiActionRequest ActionRequest = new IPGApiActionRequest();
                ActionRequest.Item = oAction;
                string request = string.Empty;
                try
                {
                    XmlSerializer reqXmlSerializer = new XmlSerializer(ActionRequest.GetType());

                    using (StringWriter textWriter = new StringWriter())
                    {
                        reqXmlSerializer.Serialize(textWriter, ActionRequest);
                        request = textWriter.ToString();
                    }
                    IPGApiActionResponse oResponse = Request.IPGApiAction(ActionRequest);

                    XmlSerializer respXmlSerializer = new XmlSerializer(oResponse.GetType());
                    string response = string.Empty;
                    using (StringWriter textWriter = new StringWriter())
                    {
                        respXmlSerializer.Serialize(textWriter, oResponse);
                        response = textWriter.ToString();
                    }
                    ViewBag.RequestString = request;
                    ViewBag.ResponseString = response;



                }
                catch (Exception ex)
                {
                    ViewBag.RequestString += request;
                    ViewBag.ResponseString += ex.ToString();
                }
            }

            return View();
        }
    }

    
}
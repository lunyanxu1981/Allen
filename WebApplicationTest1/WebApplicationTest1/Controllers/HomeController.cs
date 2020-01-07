using Newtonsoft.Json;
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
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace WebApplicationTest1.Controllers
{
    public class HomeController : Controller
    {
        public const string WEBAPI_BASEURL= "http://dev.webapi-test.com/api/";
        public const string BV_API_URL = "https://bpi.briteverify.com/emails.json?address={0}&apikey=bcb33a21-7433-469e-9187-0e6e1bab2673";

        public static string UseParams(params int[] list)
        {
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                result = list[i] + " ";
            }
            return result;
        }

        private static (string, string, string) GetTuple()
        {
            return ("str1", "str2", "str3");
        }

        public class EntityTest
        {
            public string Item1 { get; set; }
            public string Item2 { get; set; }
            public string Item3 { get; set; }
        }

        public string TestWelcome(string name, int num = 1)
        {
            return HttpUtility.HtmlEncode("Hello " + name + ", NumTimes is: ");
        }

        

        public ActionResult TestTuple()
        {
            string boolstr = "False";
            bool bvar = true;
            if (bool.TryParse(boolstr, out bvar))
            {

            }
            EntityTest obj = new EntityTest();
            (obj.Item1, obj.Item2, obj.Item3) = GetTuple();
            return Content($"{obj.Item1} {obj.Item2} {obj.Item3}");
        }

        public ActionResult TestParamsArray()
        {
            var result = UseParams();
            return Content(result);
        }

        public ActionResult ValidateEmail([System.Web.Http.FromUri]string email)
        {
            var url = string.Format(BV_API_URL, email);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string resStr = string.Empty;
            try
            {
                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        resStr = rd.ReadToEnd();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(resStr);

                        string status = jsonResponse?.status ?? string.Empty;
                        bool disposable = ((string)jsonResponse?.disposable ).IsTrue();
                        string error = jsonResponse?.error ?? string.Empty;
                        string errorCode = jsonResponse?.error_code ?? string.Empty;


                    }
                }
            }
            catch (WebException ex)
            {
                resStr = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                resStr = string.IsNullOrEmpty(resStr) ? ex.Message + "<br>" + ex.InnerException + "<br>" : resStr;
            }

            string orderForm = $"<b>BriteVerify Request:</b>{url}<br><br><b>Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";

            return Content(orderForm);

        }

        public ActionResult TestAction()
        {
            bool is3DS = true;
            Func<string, string> getStoreAttrValue = (settings) =>
            {
                string attrVal = string.Empty;                
                string[] attrValAry = settings.Split('|');
                attrVal = is3DS ? attrValAry[0] : attrValAry[1];
                return attrVal;
            };
            var attributeValue = getStoreAttrValue("4700000054");
            return Content(attributeValue);
        }

        public ActionResult TestURLRegEx()
        {
            string content = @"<p><a href=""//www.youtube.com/embed/c3oJ4G46dDg?rel=0""  width=""560"" height=""315"" frameborder=""0"" rel=""noopener"" allowfullscreen=""allowfullscreen""></a></p>
<p><a href=""//www.youtube.com/embed/c3oJ4G46dDg2?rel=0"" width=""560"" height=""315"" frameborder=""0"" allowfullscreen=""allowfullscreen""></a></p>
<p><a rel=""noopener"" href=""//www.youtube.com/embed/c3oJ4G46dDg3?rel=0"" width=""560"" height=""315"" frameborder=""0"" allowfullscreen=""allowfullscreen""></a></p>";
            string anchorPattern = $@"(<a[^>]+>)";
            string anchorReplacePattern = "<(a)([^>]+)>";
            string hrefPattern = $@"<a[^>]*?href=\""([^\""]*)\""[^<]*?>";
            string relPattern = $@"<a[^>]*?rel=\""([^\""]*)\""[^<]*?>";
            
            string result = "";
            var anchorRegMatch = Regex.Matches(content, anchorPattern);
            if (anchorRegMatch.Count>0 )
            {
                for (int i = 0; i< anchorRegMatch.Count; i++)
                {
                    var ancherStr = anchorRegMatch[i].Value;
                    
                    
                    var relRegMatch = Regex.Match(ancherStr, relPattern);
                    if (relRegMatch.Success && relRegMatch.Groups.Count > 1)
                    {
                        string rel = relRegMatch.Groups[1].Value;
                        string newVal = rel.ToLower();
                        newVal = newVal.Contains("noopener") ? newVal : $"noopener {newVal}";
                        newVal = newVal.Contains("sponsored") ? newVal : $"sponsored {newVal}";
                        newVal = newVal.Contains("nofollow") ? newVal : $"nofollow {newVal}";

                        ancherStr = Regex.Replace( ancherStr,rel, newVal);
                    }
                    else
                    {
                        string rel = @"rel=""nofollow sponsored""";
                        ancherStr = Regex.Replace(ancherStr, anchorReplacePattern, $"<$1 {rel} $2>");

                    }
                    content = content.Replace(anchorRegMatch[i].Value, ancherStr);
                }
                
            }
            return Content(content);
        }

        public ActionResult TestRegEx()
        {
            string content = @" < td width=""100"" ></td> 
 </tr> 
<!--voucher_banner_start-->
<tr style=""page-break-inside: avoid"" > 
 <td width=""100"" ></td> 
 <td style=""padding-top:20px;"" > 
 	<a href=""https://www.travelzoo.com/uk/gift-experiences/"" style=""text-decoration:none;"" target=""_blank"">
        <img src=""https://beta.ssl.tzoo-img.com/images/voucher-banner-image-template-1500.jpg"" style=""max-width:100%""/>
    </a>
 </td> 
 <td width=""100"" ></td> 
 </tr> 
 <!--voucher_banner_end-->
 </tbody> 
 </table> 
 </body>";
            string pattern = $@"<!--voucher_banner_start-->([\s\S]*)<!--voucher_banner_end-->";
            var tagTypeRegexMatch = Regex.Match(content, pattern);
            if (tagTypeRegexMatch.Success && tagTypeRegexMatch.Groups.Count >0)
            {
                content = tagTypeRegexMatch.Groups[0].Value;
            }
            return Content(content);
        }

        public ActionResult TestMast([System.Web.Http.FromUri]string tagType = "CardNumber")
        {
            string content = @"<DataStorageItem xmlns=""http://ipg-online.com/ipgapi/schemas/ipgapi"">
                <CreditCardData xmlns=""http://ipg-online.com/ipgapi/schemas/a1"">
                    <v1:CardNumber xmlns=""http://ipg-online.com/ipgapi/schemas/v1"">4035874000424977</v1:CardNumber>
                    <ExpMonth xmlns=""http://ipg-online.com/ipgapi/schemas/v1"">12</ExpMonth>
                </CreditCardData>";

            string pattern = $@"(<(ns1:|ns2:|ns3:|v1:|a1:|ipgapi:)*{tagType}[\s\S]*>)([\s\S]+?)(</(ns1:|ns2:|ns3:|v1:|a1:|ipgapi:)*{tagType}>)";
            var tagTypeRegexMatch = Regex.Match(content, pattern);
            if (tagTypeRegexMatch.Groups.Count == 6)
            {
                content = content.Replace(tagTypeRegexMatch.Groups[0].Value,
                    $"{tagTypeRegexMatch.Groups[1].Value}{MaskCreditCard(tagTypeRegexMatch.Groups[3].Value)}{tagTypeRegexMatch.Groups[4].Value}");
            }
            return Content(content);
        }

        public static string MaskCreditCard(string creditCardNumber)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length > 4)
            {
                int maskDigit = creditCardNumber.Length - 4;
                return String.Format("{0}{1}", String.Empty.PadLeft(maskDigit, '*'), creditCardNumber.Substring(maskDigit));
            }
            else
                return creditCardNumber;
        }

        public static string MaskCVV(string cvv)
        {
            if (String.IsNullOrEmpty(cvv))
                return string.Empty;
            return String.Empty.PadLeft(cvv.Length, '*');
        }



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

        public ActionResult Decrypt()
        {
            Models.Product product = new Models.Product();
            UpdateModel(product);
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
        
        
        #region HK FirstData Solution
        private T SOAPToObject<T>(string SOAP)
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

        private string CreateMerchantOrderId()
        {
            return $"{ DateTime.Now.Year.ToString()}{ DateTime.Now.Month.ToString()}{ DateTime.Now.Day.ToString()}{ DateTime.Now.Millisecond.ToString()}";
        }

        private string GetFirstDataAPIUrl()
        {
            return @"https://test.ipg-online.com/ipgapi/services";
        }

        private string GetFirstDataStoreIdHK(bool is3DS = true)
        {
            if (is3DS)
            {
                return "4700000054";
            }
            else
            {
                return "4700000064";
            }
        }

        private X509Certificate2 GetFirstDataHKCert(bool is3DS = true)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            X509Certificate2 certificate = null;
            try
            {
                if (is3DS)
                {
                    certificate = new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"certs\WS4700000054._.1.p12"), "U\"bd?96ZwU");
                }
                else
                {
                    certificate = new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"certs\WST41583125615._.1.p12"), "Z;4`6JMHcR");
                }
                
                
            }
            catch { }
            return certificate;
        }

        private NetworkCredential GetFirstDataHKNetCredential(bool is3DS = true)
        {
            if (is3DS)
            {
                return new NetworkCredential("WS4700000054._.1", @"Kn@wM57\Rg");
            }
            else
            {
                return new NetworkCredential("WST41583125615._.1", @"sz'6.xT4Ve");
            }
        }

        private HttpWebRequest CreateHttpWebRequest(bool is3DS = true)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(GetFirstDataAPIUrl());
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.ClientCertificates.Add(GetFirstDataHKCert(is3DS));
            webRequest.Credentials = GetFirstDataHKNetCredential(is3DS);
            return webRequest;
        }

        private string PostHttpRequstAndParseResponse(string request, bool is3DS = true)
        {
            string response = string.Empty;
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(request);
            HttpWebRequest webRequest = CreateHttpWebRequest(is3DS);

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            //Envelope3D obj = null;
            try
            {
                using (WebResponse httpResponse = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string res = rd.ReadToEnd();
                        //obj = SOAPToObject<Envelope3D>(res);
                        response = XDocument.Parse(res).ToString();
                    }
                }
            }
            catch (SoapException ex)
            {
                //var soapExResponseObject = ex.SoapExceptionResponseToObject();
            }
            catch (WebException ex)
            {
                response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                response = string.IsNullOrEmpty(response) ? ex.Message + "<br>" + ex.InnerException + "<br>" : response;
            }
            catch (Exception ex)
            {
                response = ex.Message + "<br>" + ex.InnerException;
            }
            return response;
        }

        /// <summary>
        /// By SOAP Service Proxy
        /// Tokenize credit card by merchant proposed tokenId(hostedDataId)
        /// </summary>
        /// <param name="hostedDataId">merchant proposed tokenId</param>
        /// <returns></returns>
        private Tuple<IPGApiActionRequest, IPGApiActionResponse> FirstDataTokenizeCustomerProfile(string hostedDataId = "")
        {

            IPGApiOrderService Request = new IPGApiOrderService();
            Request.Url = GetFirstDataAPIUrl();
            Request.ClientCertificates.Add(GetFirstDataHKCert());
            Request.Credentials = GetFirstDataHKNetCredential();

            IPGWebReference.Action oAction = new IPGWebReference.Action()
            {
                Item = new StoreHostedData()
                {
                    DataStorageItem = new DataStorageItem[] {
                        new DataStorageItem()
                        {
                            HostedDataID = hostedDataId,
                            Item = new CreditCardData()
                            {
                                ItemsElementName = new ItemsChoiceType[]
                                {
                                    ItemsChoiceType.CardNumber,
                                    ItemsChoiceType.ExpMonth,
                                    ItemsChoiceType.ExpYear,
                                    ItemsChoiceType.CardCodeValue
                                },
                                Items = new Object[]
                                {
                                    "5426064000424979", //Account Number
                                    "12", //Month
                                    "24", //Year
                                    "979" //CVV
                                }
                            }
                            
                        }
                    }
                }
            };
            IPGApiActionRequest r2 = new IPGApiActionRequest();

            r2.Item = oAction;

            string request = string.Empty;
            string response = string.Empty;
            IPGApiActionResponse oResponse = null;
            try
            {
                oResponse = Request.IPGApiAction(r2);
            }
            catch { }
            return new Tuple<IPGApiActionRequest, IPGApiActionResponse>(r2, oResponse);
        }

        /// <summary>
        /// By SOAP Service Proxy
        /// Retrieve credit card data by stored token id(hostedDataId)
        /// </summary>
        /// <param name="hostedDataId">stored tokenId</param>
        /// <returns></returns>
        private Tuple<IPGApiActionRequest, IPGApiActionResponse> FirstDataRetrieveCustomerProfileCommon(string hostedDataId = "", bool is3DS = true)
        {
            IPGApiOrderService Request = new IPGApiOrderService();
            Request.Url = GetFirstDataAPIUrl();
            Request.ClientCertificates.Add(GetFirstDataHKCert(is3DS));
            Request.Credentials = GetFirstDataHKNetCredential(is3DS);
            IPGApiActionRequest r2 = new IPGApiActionRequest()
            {
                Item = new IPGWebReference.Action()
                {
                    Item = new StoreHostedData()
                    {
                        StoreId = GetFirstDataStoreIdHK(is3DS),
                        DataStorageItem = new DataStorageItem[]
                        {
                            new DataStorageItem()
                            {
                                HostedDataID = hostedDataId,
                                Item = WebApplicationTest1.IPGWebReference.DataStorageItemFunction.display
                            }
                        }
                    }
                }
            };

            string request = string.Empty;
            string response = string.Empty;
            IPGApiActionResponse oResponse = null;
            try
            {
                Func<CreditCardData, ItemsChoiceType, string> getCCAttr = 
                    (c, attr) =>
                    {
                        string attrVal = string.Empty;
                        int index = -1;
                        if (c == null || c.ItemsElementName == null || c.ItemsElementName.Length <= 0)
                            return attrVal;
                        for (int i = 0; i < c.ItemsElementName.Length; ++i)
                        {
                            if (c.ItemsElementName[i] == attr)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index > -1 && c.Items !=null && c.Items.Length > index)
                        {
                            attrVal = (string)c.Items[index];
                        }
                        return attrVal;
                    };
                oResponse = Request.IPGApiAction(r2);
                var accountNumber = getCCAttr((CreditCardData)oResponse.DataStorageItem[0].Item, ItemsChoiceType.CardNumber);
                var expYear = getCCAttr((CreditCardData)oResponse.DataStorageItem[0].Item, ItemsChoiceType.ExpYear);
                var expMonth = getCCAttr((CreditCardData)oResponse.DataStorageItem[0].Item, ItemsChoiceType.ExpMonth);
            }            
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                response = ex.Message + "<br>" + ex.InnerException + "<br>" + resp;
            }
            catch (Exception ex)
            {
                response = ex.Message + "<br>" + ex.InnerException;
            }
            return new Tuple<IPGApiActionRequest, IPGApiActionResponse>(r2, oResponse);
        }

        /// <summary>
        /// Retrieve customer payment token by stored token id (hostedDataId)
        /// </summary>
        /// <param name="hostedDataId"></param>
        /// <returns></returns>
        public ActionResult FirstDataRetrieveCustomerProfile([System.Web.Http.FromUri]string hostedDataId="", [System.Web.Http.FromUri]bool is3DS = true)
        {
            string request = string.Empty;
            string response = string.Empty;
            try
            {
                var result = FirstDataRetrieveCustomerProfileCommon(hostedDataId, is3DS);

                XmlSerializer reqXmlSerializer = new XmlSerializer(result.Item1.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    reqXmlSerializer.Serialize(textWriter, result.Item1);
                    request = textWriter.ToString();
                }
                IPGApiActionResponse oResponse = result.Item2;

                XmlSerializer respXmlSerializer = new XmlSerializer(oResponse.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    respXmlSerializer.Serialize(textWriter, oResponse);
                    response = textWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                response = ex.Message + "<br>" + ex.InnerException;
            }
            string orderForm = $"<b>Retrieve Profile Request:</b><br><xmp>{request}</xmp><br><br><b>Retrieve Profile Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{response}</textarea><br><br><br>";
            return Content(orderForm);
        }

        public ActionResult FirstDataHKRefund([System.Web.Http.FromUri]string orderId, [System.Web.Http.FromUri]bool is3DS = true)
        {

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            X509Certificate2 certificate = null;
            try
            {
                certificate = GetFirstDataHKCert(is3DS);
            }
            catch { }
            
            IPGApiOrderService Request = new IPGApiOrderService();
            Request.ClientCertificates.Add(certificate);
            Request.Url = GetFirstDataAPIUrl();
            Request.Credentials = GetFirstDataHKNetCredential(is3DS) ;

            IPGApiOrderRequest oRefundOrderRequest = new IPGApiOrderRequest();
            Transaction oTransaction = new Transaction();
            CreditCardTxType oCreditCardTxType = new CreditCardTxType();

            oCreditCardTxType.StoreId = GetFirstDataStoreIdHK(is3DS);
            oCreditCardTxType.Type = CreditCardTxTypeType.@return;

            oTransaction.Items = new Object[] { oCreditCardTxType };

            Payment oPayment = new Payment();
            oPayment.SubTotal = 1;
            oPayment.ChargeTotal = 1;
            oPayment.Currency = "344";
            oTransaction.Payment = oPayment;

            TransactionDetails oTransactionDetails = new TransactionDetails();
            oTransactionDetails.OrderId = orderId;
            oTransaction.TransactionDetails = oTransactionDetails;

            oRefundOrderRequest = new IPGApiOrderRequest();
            oRefundOrderRequest.Item = oTransaction;
            string request = string.Empty;
            string response = string.Empty;
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
                    
                using (StringWriter textWriter = new StringWriter())
                {
                    respXmlSerializer.Serialize(textWriter, oResponse);
                    response = textWriter.ToString();
                }
            }
            catch (WebException ex)
            {
                response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                response = string.IsNullOrEmpty(response) ? ex.Message + "<br>" + ex.InnerException + "<br>" : response;
            }
            catch (SoapException se)
            {//SoapException: MerchantException or ProcessingException
                   
                response = se.SoapExceptionResponseToString() ?? "";
            }
            catch (Exception ex)
            {
                response += ex.ToString();
            }
           

            string orderForm = $"<b>Order Refund Request:</b><br><xmp>{request}</xmp><br><br><b>Order Refund Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{response}</textarea><br><br><br>";
            return Content(orderForm);
        }

        public ActionResult FirstDataHKInquiry([System.Web.Http.FromUri]string orderId, [System.Web.Http.FromUri]bool is3DS = true)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string storeId = GetFirstDataStoreIdHK(is3DS);
            string request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiActionRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/a1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/v1"">
        <ns2:Action>
            <ns2:InquiryOrder>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:OrderId>"+ orderId + @"</ns2:OrderId>
            </ns2:InquiryOrder>
          </ns2:Action>
    </ns4:IPGApiActionRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>"; 


            string resStr = string.Empty;
            try
            {
                
                var httpWebRequest = new HttpWebRequester(Encoding.UTF8)
                {
                    IgnoreHttpsCertificateValidation = true,
                    ContentType = "text/xml;charset=\"utf-8\"",
                    Accept = "text/xml"
                };
                string res = httpWebRequest.SoapPost(request, GetFirstDataAPIUrl(), GetFirstDataHKCert(is3DS), GetFirstDataHKNetCredential(is3DS));
                EnvelopeFDCC obj = SOAPToObject<EnvelopeFDCC>(res);
                resStr = res;
                resStr = XDocument.Parse(resStr).ToString();
            }
            /*
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(request);
            HttpWebRequest webRequest = CreateHttpWebRequest(is3DS);

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            EnvelopeFDCC obj = null;
            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string res = rd.ReadToEnd();
                        obj = SOAPToObject<EnvelopeFDCC>(res);
                        resStr = res;
                        resStr = XDocument.Parse(resStr).ToString();
                    }
                }
            }*/
            catch (WebException ex)
            {
                resStr = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                resStr = string.IsNullOrEmpty(resStr) ? ex.Message + "<br>" + ex.InnerException + "<br>" : resStr;
            }
            catch (Exception ex)
            {
                resStr = ex.Message + "<br>" + ex.InnerException;
            }
            
            string orderForm = $"<b>Order Inquiry Request:</b><br><xmp>{request}</xmp><br><br><b>Order Inquiry Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";
            return Content(orderForm);
            
        }

        /// <summary>
        /// Do tokenization
        /// </summary>
        /// <returns></returns>
        public ActionResult FirstDataCreatePaymentToken()
        {
            var token = Guid.NewGuid().ToString();
            var result = FirstDataRetrieveCustomerProfileCommon(token);
            while (result?.Item2 != null && (result?.Item2?.Error == null || result?.Item2?.Error?.Length <=0))
            {
                token = Guid.NewGuid().ToString();
                result = FirstDataRetrieveCustomerProfileCommon(token);
            }
            var tokenResult = FirstDataTokenizeCustomerProfile(token);

            string request = string.Empty;
            string response = string.Empty;
            XmlSerializer reqXmlSerializer = new XmlSerializer(tokenResult.Item1.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                reqXmlSerializer.Serialize(textWriter, tokenResult.Item1);
                request = textWriter.ToString();
            }
            IPGApiActionResponse oResponse = tokenResult.Item2;

            XmlSerializer respXmlSerializer = new XmlSerializer(oResponse.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                respXmlSerializer.Serialize(textWriter, oResponse);
                response = textWriter.ToString();
            }

            string orderForm = $"<b>Tokenize Request:</b><br><xmp>{request}</xmp><br><br><b>Tokenize Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{response}</textarea><br><br><br>";
            return Content(orderForm);
        }

        /// <summary>
        /// Do 3D purchase
        /// </summary>
        /// <param name="useToken"></param>
        /// <param name="byToken"></param>
        /// <returns></returns>
        public ActionResult FirstData3DPurchase([System.Web.Http.FromUri]bool useToken = false, [System.Web.Http.FromUri]string token = "", [System.Web.Http.FromUri]string card = "vc")
        {
            string cardMC = "5426064000424979";
            string cardVC = "4035874000424977";

            string cvvMC = "979";
            string cvvVC = "977";
            string cardnumber = card.Equals("vc", StringComparison.InvariantCultureIgnoreCase) ? cardVC : cardMC;
            string cvv = card.Equals("vc", StringComparison.InvariantCultureIgnoreCase) ? cvvVC : cvvMC;
            bool is3DS = true;
            string storeId = GetFirstDataStoreIdHK(is3DS);
            string merchantOrderId = $"{ DateTime.Now.Year.ToString()}{ DateTime.Now.Month.ToString()}{ DateTime.Now.Day.ToString()}{ DateTime.Now.Millisecond.ToString()}";
            var request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>payerAuth</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:CardNumber>"+ cardnumber + @"</ns2:CardNumber>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>"+ cvv + @"</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:CreditCard3DSecure>
                <ns2:AuthenticateTransaction>true</ns2:AuthenticateTransaction>
            </ns2:CreditCard3DSecure>
            <ns2:Payment>
                <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
                <ns2:AssignToken>true</ns2:AssignToken>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Billing>
                <ns2:Firstname>Allen</ns2:Firstname>
                <ns2:Surname>Test</ns2:Surname>
                <ns2:Address1>Flat 412a 123 London Rd</ns2:Address1>
                <ns2:Address2>Test District</ns2:Address2>
                <ns2:City>London</ns2:City>
                <ns2:Zip>CH488AQ</ns2:Zip>
                <ns2:Country>GB</ns2:Country>
                <ns2:Phone>01522113356</ns2:Phone>
                <ns2:Email>youremail@email.com</ns2:Email>
            </ns2:Billing>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>12345</ns2:ID>
                    <ns2:Description>Voucher Product</ns2:Description>
                    <ns2:SubTotal>13.99</ns2:SubTotal>
                    <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            

            if (useToken && token != string.Empty)
            {
                request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>payerAuth</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>977</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:CreditCard3DSecure>
                <ns2:AuthenticateTransaction>true</ns2:AuthenticateTransaction>
            </ns2:CreditCard3DSecure>
            <ns2:Payment>
                <ns2:HostedDataID>"+ token + @"</ns2:HostedDataID>
                <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>12345</ns2:ID>
                    <ns2:Description>Voucher Product</ns2:Description>
                    <ns2:SubTotal>13.99</ns2:SubTotal>
                    <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            }

            string resStr = string.Empty;
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(request);
            HttpWebRequest webRequest = CreateHttpWebRequest(is3DS);

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            EnvelopeFDCC obj = null;
            
            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string res = rd.ReadToEnd();
                        obj = SOAPToObject<EnvelopeFDCC>(res);
                        resStr = res;
                        resStr = XDocument.Parse(resStr).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                resStr = ex.Message + "<br>" + ex.InnerException;
            }
            string orderForm = $"<b>Step #1 <br>VerRequest:</b><xmp>{request}</xmp><br><br><b>VerResponse:</b><br><textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";
            var AcsURL = obj?.Body?.IPGApiOrderResponse?.Secure3DResponse?.Secure3DVerificationResponse?.VerificationRedirectResponse?.AcsURL;
            var MD = obj?.Body?.IPGApiOrderResponse?.Secure3DResponse?.Secure3DVerificationResponse?.VerificationRedirectResponse?.MD;
            var PaReq = obj?.Body?.IPGApiOrderResponse?.Secure3DResponse?.Secure3DVerificationResponse?.VerificationRedirectResponse?.PaReq;
            string TermUrl = $"http://dev.webmvc-test.com/Home/FirstData3DAuthCallbackTest/?IpgTransactionId={obj?.Body?.IPGApiOrderResponse?.IpgTransactionId}";
            orderForm += $"<b>Step #2</b><br><b>AcsURL:</b> {AcsURL}<br>" +
                $"<b>MD:</b> {MD}<br>" +
                $"<b>PaReq:</b> {PaReq}<br>" +
                $"<b>TermUrl:</b> {TermUrl}<br><br>";

            orderForm += $"<b>Acs Form</b><br><form method=\"post\" action=\"{AcsURL}\">" +
                $"<b>MD:</b><input type=\"input\" name=\"MD\" value=\"{MD}\"><br>" +
                $"<b>PaReq:</b><input type=\"input\" name=\"PaReq\" value=\"{PaReq}\"><br>" +
                $"<b>TermUrl:</b><input type=\"input\" name=\"TermUrl\" value=\"{TermUrl}\"><br>" +
                $"<input type=\"submit\" value=\"Submit\" id=\"btnFDSubmit\">" +
                $"</form><br><br>";
            return Content(orderForm);
        }

        /// <summary>
        /// 3D callback
        /// </summary>
        /// <param name="ipgTransactionId"></param>
        /// <returns></returns>
        [HttpPost, Route("FirstData3DAuthCallbackTest")]
        public ActionResult FirstData3DAuthCallbackTest([System.Web.Http.FromUri]string ipgTransactionId)
        {
            bool is3DS = true;
            string storeId = GetFirstDataStoreIdHK(is3DS);
            string htmlStr = "<b>Step #2 Acs Response:</b><br><textarea style='width:1280px;height:500px;border:0'>";
            foreach (var key in Request.Form.AllKeys)
            {
                htmlStr += $"{key}={Request.Form[key]}";
            }
            htmlStr += "</textarea><br><br>";
            var PaRes = Request.Form["PaRes"];
            var MD = Request.Form["MD"];
            
            var request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>"+ storeId+ @"</ns2:StoreId>
                <ns2:Type>payerAuth</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCard3DSecure>
                <ns2:Secure3DRequest>
                    <ns2:Secure3DAuthenticationRequest>
                        <ns2:AcsResponse>
                            <ns2:MD>" + MD+ @"</ns2:MD>
                            <ns2:PaRes>" + PaRes+ @"</ns2:PaRes>
                        </ns2:AcsResponse>
                    </ns2:Secure3DAuthenticationRequest>
                </ns2:Secure3DRequest>
            </ns2:CreditCard3DSecure>
            <ns2:TransactionDetails>
                <ns2:IpgTransactionId>" + ipgTransactionId + @"</ns2:IpgTransactionId>
            </ns2:TransactionDetails>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";

            string resStr = PostHttpRequstAndParseResponse(request, is3DS);
            EnvelopeFDCC obj = SOAPToObject<EnvelopeFDCC>(resStr);
            var completeTrxnStr = CompleteFirstData3DTransaction(storeId, ipgTransactionId);

            htmlStr += $"<b>Step #3 <br>AuthRequest:</b><br><textarea style='width:1280px;height:900px;border:0'>{request}</textarea><br><br>" +
                $"<b>AuthResponse:</b><br>" +
                $"<textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>{completeTrxnStr}";

            return Content(htmlStr);
        }

        private string CompleteFirstData3DTransaction(string storeId, string ipgTransactionId)
        {
            bool is3DS = true;
            var request = @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
    <SOAP-ENV:Body>
        <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>sale</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:TransactionDetails>
                <ns2:IpgTransactionId>" + ipgTransactionId + @"</ns2:IpgTransactionId>
            </ns2:TransactionDetails>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";

            string resStr = PostHttpRequstAndParseResponse(request, is3DS);

            string htmlStr = $"<b>Step #4 <br>SaleRequest:</b><br><textarea style='width:1280px;height:450px;border:0'>{request}</textarea><br><br>" +
                $"<b>AuthResponse:</b><br>" +
                $"<textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";
            return htmlStr;
        }

        public ActionResult FirstDataNon3DPurchase([System.Web.Http.FromUri]bool useToken = false, [System.Web.Http.FromUri]string token = "", [System.Web.Http.FromUri] string card="vc")
        {
            string cardMC = "5426064000424979";
            string cardVC = "4035874000424977";

            string cvvMC = "979";
            string cvvVC = "977";
            string cardnumber = card.Equals("vc", StringComparison.InvariantCultureIgnoreCase) ? cardVC : cardMC;
            string cvv = card.Equals("vc", StringComparison.InvariantCultureIgnoreCase) ? cvvVC : cvvMC;
            bool is3DS = false;
            string storeId = GetFirstDataStoreIdHK(is3DS);
            string parentStoreId = GetFirstDataStoreIdHK(true);
            string merchantOrderId = CreateMerchantOrderId();
            var request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>sale</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:CardNumber>"+ cardnumber + @"</ns2:CardNumber>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>"+ cvv + @"</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:Payment>
                <ns2:HostedDataStoreID>" + parentStoreId + @"</ns2:HostedDataStoreID>
                <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
                <ns2:AssignToken>true</ns2:AssignToken>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>12345</ns2:ID>
                    <ns2:Description>Voucher Product</ns2:Description>
                    <ns2:SubTotal>13.99</ns2:SubTotal>
                    <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";


            if (useToken && token!=string.Empty)
            {
                request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>sale</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>977</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:Payment>
                <ns2:HostedDataID>" + token+ @"</ns2:HostedDataID>
                <ns2:HostedDataStoreID>" + parentStoreId + @"</ns2:HostedDataStoreID>
                <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>12345</ns2:ID>
                    <ns2:Description>Voucher Product</ns2:Description>
                    <ns2:SubTotal>13.99</ns2:SubTotal>
                    <ns2:ChargeTotal>13.99</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            }

            string resStr = PostHttpRequstAndParseResponse(request, is3DS);

            string orderForm = $"<b>Sale Request:</b><xmp>{request}</xmp><br><br><b>Sale Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";
            
            return Content(orderForm);

        }

        public ActionResult FirstDataPreAuth([System.Web.Http.FromUri]bool useToken = false, [System.Web.Http.FromUri]string token = "")
        {
            bool is3DS = false;
            string storeId = GetFirstDataStoreIdHK(is3DS);
            string parentStoreId = GetFirstDataStoreIdHK(true);
            string merchantOrderId = $"{ DateTime.Now.Year.ToString()}{ DateTime.Now.Month.ToString()}{ DateTime.Now.Day.ToString()}{ DateTime.Now.Millisecond.ToString()}";
            var request = @"<?xml version=""1.0"" encoding=""UTF-8""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>preAuth</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:CardNumber>4035874000424977</ns2:CardNumber>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>977</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:Payment>
                <ns2:HostedDataStoreID>" + parentStoreId + @"</ns2:HostedDataStoreID>
                <ns2:ChargeTotal>1.00</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
                <ns2:AssignToken>true</ns2:AssignToken>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>67890</ns2:ID>
                    <ns2:Description>Booking Product</ns2:Description>
                    <ns2:SubTotal>1.00</ns2:SubTotal>
                    <ns2:ChargeTotal>1.00</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";


            if (useToken && token != string.Empty)
            {
                request = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
<SOAP-ENV:Header/>
<SOAP-ENV:Body>
    <ns4:IPGApiOrderRequest xmlns:ns4=""http://ipg-online.com/ipgapi/schemas/ipgapi"" 
        xmlns:ns2=""http://ipg-online.com/ipgapi/schemas/v1"" 
        xmlns:ns3=""http://ipg-online.com/ipgapi/schemas/a1"">
        <ns2:Transaction>
            <ns2:CreditCardTxType>
                <ns2:StoreId>" + storeId + @"</ns2:StoreId>
                <ns2:Type>preAuth</ns2:Type>
            </ns2:CreditCardTxType>
            <ns2:CreditCardData>
                <ns2:ExpMonth>12</ns2:ExpMonth>
                <ns2:ExpYear>24</ns2:ExpYear>
                <ns2:CardCodeValue>977</ns2:CardCodeValue>
            </ns2:CreditCardData>
            <ns2:Payment>
                <ns2:HostedDataID>" + token + @"</ns2:HostedDataID>
                <ns2:HostedDataStoreID>" + parentStoreId + @"</ns2:HostedDataStoreID>
                <ns2:ChargeTotal>1.00</ns2:ChargeTotal>
                <ns2:Currency>344</ns2:Currency>
            </ns2:Payment>
            <ns2:TransactionDetails>
                <ns2:OrderId>" + merchantOrderId + @"</ns2:OrderId>
            </ns2:TransactionDetails>
            <ns2:Basket>
                <ns2:Item>
                    <ns2:ID>67890</ns2:ID>
                    <ns2:Description>Booking Product</ns2:Description>
                    <ns2:SubTotal>1.00</ns2:SubTotal>
                    <ns2:ChargeTotal>1.00</ns2:ChargeTotal>
                    <ns2:Currency>344</ns2:Currency>
                    <ns2:Quantity>1</ns2:Quantity>
                </ns2:Item>
            </ns2:Basket>
        </ns2:Transaction>
    </ns4:IPGApiOrderRequest>
</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            }

            string resStr = PostHttpRequstAndParseResponse(request, is3DS);

            string orderForm = $"<b>Booking Request:</b><xmp>{request}</xmp><br><br><b>Booking Response:</b><br><textarea style='width:1280px;height:300px;border:0'>{resStr}</textarea><br><br><br>";

            return Content(orderForm);

        }


        #endregion
        [HttpGet]
        public ActionResult Index1()
        {
            SignupUser user = new SignupUser();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpAjax(SignupUser viewModel)
        {
            bool isSuccess = true;

            if (ModelState.IsValid)
            {
                // check if the same member id exists in system
                if (viewModel.SysUserId == "test")
                {
                    ModelState.AddModelError("SysUserId", "The user id has already existed in system");
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    
                }

            }

            var returnData = new
            {
                
                IsSuccess = isSuccess,
                // ModelState錯誤訊息 
                ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
            };
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(returnData), "application/json");
        }
    }

    
}

public static class ExtentionCls
{
    public static bool IsTrue(this string str)
    {
        if (str == null) return false;

        switch (str.Trim().ToLowerInvariant())
        {
            case "1":
            case "true":
            case "yes":
            case "y":
            case "f":
            case "on":
            case "checked":
            case "active":
                return true;
            default:
                return false;
        }

    }
}
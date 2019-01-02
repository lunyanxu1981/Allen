using Newtonsoft.Json;
using System;
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
            return View(product);
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";


            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            X509Certificate2 certificate = null;
            try
            { 
                certificate = new X509Certificate2(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                                    @"certs\WS4700000018._.1.p12"), "P2T$u8%Fhm");
            }
            catch {}

            IPGApiOrderService Request = new IPGApiOrderService();
            String RequestResponse = "";
            if (certificate != null)
            {
                Request.ClientCertificates.Add(certificate);
                Request.Url = @"https://test.ipg-online.com:443/ipgapi/services";
                Request.Credentials = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");

                InquiryOrder oInquiryOrder = new InquiryOrder()
                {
                    StoreId = "4700000018", OrderId = "84521240216"
                };
                
                IWebProxy webProxy = new WebProxy("test.ipg-online.com", 443);
                webProxy.Credentials = new NetworkCredential("WS4700000018._.1", "dJV_.2n7uS");
                Request.Proxy = webProxy;

                IPGWebReference.Action oAction = new IPGWebReference.Action()
                {
                    Item = oInquiryOrder,
                    ClientLocale = new ClientLocale()
                    {
                        Country = "UK",
                        Language = "en"
                    }
                };
                
                IPGApiActionRequest ActionRequest = new IPGApiActionRequest();
                ActionRequest.Item = oAction;
                try
                {
                    IPGApiActionResponse oResponse = Request.IPGApiAction(ActionRequest);
                    RequestResponse += "Succesfully:" + oResponse.successfully;
                }
                catch (Exception e) { }
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

    }
}
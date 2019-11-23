using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedisWebApplication.App_Start;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace RedisWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string API_KEY = "7a9eaba0-8af2-4354-950a-6833333442cc";
        public static readonly string VERSION = "1.0";
        public static readonly string API_AFFILIATEID = "1218";
        

        public ActionResult Index()
        {
            TestRESTCall();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void GetAllDeals()
        {
            
        }

        public ActionResult RedisCache()
        {
            ViewBag.Message = "A simple example with Azure Cache for Redis on ASP.NET.";
            
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                var options = ConfigurationOptions.Parse(cacheConnection);
                options.Ssl = true;
                options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                return ConnectionMultiplexer.Connect(options);
            });



            // Connection refers to a property that returns a ConnectionMultiplexer
            // as shown in the previous example.
            IDatabase cache = lazyConnection.Value.GetDatabase();
            RedisAgent cache2 = new RedisAgent();
            // Perform cache operations using the cache object...
            //RedisAgent cache = new RedisAgent();
            // Simple PING command
            ViewBag.command1 = "PING";
            //ViewBag.command1Result = cache.ExcuteRedisCommand(ViewBag.command1)?.ToString();
            ViewBag.command1Result = cache.Execute(ViewBag.command1)?.ToString();

            // Simple get and put of integral data types into the cache
            ViewBag.command2 = "GET Message";
            //ViewBag.command2Result = cache.GetRedisItem("Message").ToString();
            ViewBag.command2Result = cache.StringGet("Message").ToString();

            ViewBag.command3 = "SET Message \"Hello! The cache is working from ASP.NET!\"";
            //ViewBag.command3Result = cache.SetRedisItem("Message", "Hello! The cache is working from ASP.NET!").ToString();
            ViewBag.command3Result = cache.StringSet("Message", "Hello! The cache is working from ASP.NET!").ToString();

            // Demonstrate "SET Message" executed as expected...
            ViewBag.command4 = "GET Message";
            //ViewBag.command4Result = cache2.GetRedisItem("Message").ToString();
            ViewBag.command4Result = cache.StringGet("Message").ToString();

            // Get the client list, useful to see if connection list is growing...
            ViewBag.command5 = "CLIENT LIST";
            //ViewBag.command5Result = cache.ExcuteRedisCommand("CLIENT", "LIST")?.ToString()?.Replace(" id=", "\rid=");
            ViewBag.command5Result = cache.Execute("CLIENT", "LIST")?.ToString()?.Replace(" id=", "\rid=");



            return View();
        }

        public  string GetMessageWithAcceptHeader(string restURI, string method, string contentType, string acceptHeaderType, string apiKey)
        {
            HttpWebRequest request = CreateWebRequestWithAcceptHeader(restURI, method, contentType, acceptHeaderType);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
                }
                return responseValue;
            }
        }

        public HttpWebRequest CreateWebRequestWithAcceptHeader(string restURI, string method, string requestContentType, string acceptHeaderType)
        {
            var request = (HttpWebRequest)WebRequest.Create(restURI);
            request.Method = method;
            request.ContentLength = 0;
            request.ContentType = requestContentType;
            request.Accept = acceptHeaderType;
            return request;
        }

        public HttpWebRequest CreateWebRequestWithAcceptHeader(string restURI, string method, string requestContentType, string acceptHeaderType, string apiKey)
        {
            var request = (HttpWebRequest)WebRequest.Create(restURI);
            request.Headers.Add("ApiKey", API_KEY);
            request.Method = method;
            request.ContentLength = 0;
            request.ContentType = requestContentType;
            request.Accept = acceptHeaderType;
            return request;
        }

        public string HttpPost(string restURI, string method, string contentType, Dictionary<string, string> paramList, string acceptHeaderType)
        {
            HttpWebRequest req = CreateWebRequestWithAcceptHeader(restURI, method, contentType, acceptHeaderType);
            // Build a string with all the params, properly encoded.
            StringBuilder paramz = new StringBuilder();
            if (paramList != null)
            {
                foreach (var keyValue in paramList)
                {
                    paramz.Append(keyValue.Key);
                    paramz.Append("=");
                    paramz.Append(System.Web.HttpUtility.UrlEncode(keyValue.Value));
                    paramz.Append("&");
                }
            }
            // Encode the parameters as form data:
            byte[] formData = UTF8Encoding.UTF8.GetBytes(paramz.ToString());
            req.ContentLength = formData.Length;
            // Send the request:
            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }
            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }

        

        protected  void TestRESTCall()
        {
            StringBuilder theResp = new StringBuilder();
            try
            {
                Stopwatch sWatch = new Stopwatch();
                theResp.AppendFormat("\n----------------- Timer start : {0}", System.DateTime.Now.TimeOfDay);
                sWatch.Start();
                string RESTUri = $"https://beta.ssl.localws.travelzoo.com/affiliate/AffiliateAPIService.svc/Deals?ApiKey={API_KEY}&ApiVersion={VERSION}&ApiAffiliateId={API_AFFILIATEID}&AffiliateId={API_AFFILIATEID}&LocaleCode=cn&Offset=0&limit=1000";
                string httpVerb = "GET";
                string requestType = "multipart/form-data"; //You can also send the query string parameters as a form encoded name value collection
                                                            //Impt: As part of the PCI Security update, the ApiKey will be read from the Request Header. Pass the key in the request header with name 'ApiKey'
                string apiKey = "SomeGuid";
                string acceptHeaderType = "application/json";
                string theResponse = GetMessageWithAcceptHeader(RESTUri, httpVerb, requestType, acceptHeaderType, apiKey);
                sWatch.Stop();
                theResp.AppendFormat("\n----------------- Time elapsed : {0}", sWatch.Elapsed);
                theResp.AppendFormat("\n----------------- The response : \n\n {0}", theResponse);
            }
            catch (Exception ex)
            {
                theResp.Append(ex.ToString());
            }
            var txtResults = theResp.ToString(); //Display the results in the UI textbox
        }
    }
}
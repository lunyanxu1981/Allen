using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumConsoleApp
{
    public class HttpWebRequester
    {
        private const string DefaultUserAgent = "";
        private const string DefaultContentType = "application/x-www-form-urlencoded";

        private Encoding _encoding;
        private Dictionary<string, string> headerDic;

        public string UserAgent { get; set; }
        public string ContentType { get; set; }

        public bool? Expect100Continue { get; set; }
        public bool? IgnoreHttpsCertificateValidation { get; set; }

        public Func<List<X509Certificate2>> GetCertDelegate { get; set; }

        private SecurityProtocolType? originType;
        /// <summary>
        /// When this is set, means we want to use our own indicated protocol type per this request.
        /// However since .NET 4.5.1 doesn't support to set per request, we actually set it globally then retrieve it back.
        /// It might cause some improper impact when in multiple-threads situation.
        /// We'll wait .NET framework 4.7.2 (maybe?) to get it solved perfectly.
        /// </summary>
        public SecurityProtocolType? SecurityProtocolType { get; set; }

        public HttpWebRequester(Encoding enc)
        {
            this._encoding = enc;
            this.headerDic = new Dictionary<string, string>();
            this.Expect100Continue = ServicePointManager.Expect100Continue;

            this.UserAgent = DefaultUserAgent;
            this.ContentType = DefaultContentType;
        }

        #region Extra Information : Header && Certification

        public void AddHeader(string key, string val)
        {
            this.headerDic[key] = val;
        }

        #endregion

        #region Get & Post & Put

        private void AddExtraInformation(HttpWebRequest httpRequest)
        {
            //add extra header
            foreach (var item in this.headerDic)
            {
                httpRequest.Headers[item.Key] = item.Value;
            }

            //add certificate
            if (this.GetCertDelegate != null)
            {
                httpRequest.ClientCertificates.AddRange(this.GetCertDelegate().ToArray());
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //always accept
            //notice : actually we can do some validation here.
            return true;
        }

        private HttpWebRequest CreateHttpRequest(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new Exception("Create http request failed.");
            }

            httpRequest.UserAgent = this.UserAgent;
            httpRequest.ContentType = this.ContentType;

            //if set expect-100-continue
            if ((httpRequest.ServicePoint != null) &&
                (this.Expect100Continue.HasValue))
            {
                httpRequest.ServicePoint.Expect100Continue = this.Expect100Continue.Value;
            }

            //if set ignore https certification
            if ((string.Equals(httpRequest.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) &&
                this.IgnoreHttpsCertificateValidation.HasValue))
            {
                //Notice: in .NET 4.5 this can be set via each http request.
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
            }

            //add extra info.
            this.AddExtraInformation(httpRequest);

            return httpRequest;
        }

        private System.Drawing.Image GetResponseImage(HttpWebRequest httpRequest)
        {
            System.Drawing.Image img = null;

            using (Stream responseStream = httpRequest.GetResponse().GetResponseStream())
            {
                img = System.Drawing.Image.FromStream(responseStream);
            }

            return img;
        }

        private Stream GetResponseStream(HttpWebRequest httpRequest)
        {
            WebResponse wr = null;
            try
            {
                wr = httpRequest.GetResponse();
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    wr = webEx.Response;
                }
                else
                {
                    throw webEx;
                }
            }

            return wr.GetResponseStream();
        }

        private string GetResponse(HttpWebRequest httpRequest)
        {
            string responseStr = string.Empty;

            using (Stream responseStream = GetResponseStream(httpRequest))
            {
                using (StreamReader sr = new StreamReader(responseStream, this._encoding))
                {
                    responseStr = sr.ReadToEnd();
                }
            }

            return responseStr;
        }

        private async Task<string> GetResponseAsync(HttpWebRequest httpRequest)
        {
            string responseStr = string.Empty;

            using (Stream responseStream = GetResponseStream(httpRequest))
            {
                using (StreamReader sr = new StreamReader(responseStream, this._encoding))
                {
                    responseStr = await sr.ReadToEndAsync();
                }
            }

            return responseStr;
        }

        private string Submit(byte[] data, string url, string method)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = method;
                httpRequest.ContentLength = data.Length;

                using (Stream requestStream = httpRequest.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }

                return this.GetResponse(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.Submit] failed. Url:{0}, Method:{1}", url, method);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }

        }

        private async Task<string> SubmitAsync(byte[] data, string url, string method)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = method;
                httpRequest.ContentLength = data.Length;

                using (Stream requestStream = httpRequest.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }

                return await this.GetResponseAsync(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.Submit] failed. Url:{0}, Method:{1}", url, method);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }

        }

        public string Post(byte[] data, string url)
        {
            return this.Submit(data, url, "POST");
        }

        public async Task<string> PostAsync(byte[] data, string url)
        {
            return await this.SubmitAsync(data, url, "POST");
        }

        public string Post(string content, string url)
        {
            byte[] data = this._encoding.GetBytes(content);
            return this.Submit(data, url, "POST");
        }

        public async Task<string> PostAsync(string content, string url)
        {
            byte[] data = this._encoding.GetBytes(content);
            return await this.SubmitAsync(data, url, "POST");
        }

        public string Put(byte[] data, string url)
        {
            return this.Submit(data, url, "PUT");
        }

        public string Put(string content, string url)
        {
            byte[] data = this._encoding.GetBytes(content);
            return this.Submit(data, url, "PUT");
        }

        public string Get(string url)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = "Get";
                return this.GetResponse(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.Get] failed. Url:{0}", url);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }
        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = "Get";
                return await this.GetResponseAsync(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.Get] failed. Url:{0}", url);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }
        }

        public Stream GetStream(string url)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = "Get";
                return this.GetResponseStream(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.GetStream] failed. Url:{0}", url);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }
        }

        public System.Drawing.Image GetImage(string url)
        {
            try
            {
                this.SetSecurityProtocolType();

                HttpWebRequest httpRequest = this.CreateHttpRequest(url);
                httpRequest.Method = "Get";
                return this.GetResponseImage(httpRequest);
            }
            catch (Exception ex)
            {
                string message = string.Format("[HttpWebRequester.GetImage] failed. Url:{0}", url);
                //LogHelper.Warning(message, ex);
                throw;
            }
            finally
            {
                this.RetrieveSecurityProtocolType();
            }
        }

        #endregion

        #region Save & Retrieve <SecurityProtocolType>

        private void SetSecurityProtocolType()
        {
            //preserve original protocal type
            if (this.SecurityProtocolType.HasValue)
            {
                this.originType = ServicePointManager.SecurityProtocol;
                // ECP-2805: use bitwise or for the protocols, adding the specific protocol instead of overwriting the original one completely.
                // This is necessary, because even if this request requires the specified protocol, the ServicePointManager.SecurityProtocol is a global setting
                // and all other threads' protocols are also overwritten, causing some other services to not be able to negotiate a correct protocol.
                ServicePointManager.SecurityProtocol = this.SecurityProtocolType.Value | ServicePointManager.SecurityProtocol;
            }
        }

        private void RetrieveSecurityProtocolType()
        {
            //retrieve protocol
            if ((this.SecurityProtocolType.HasValue) && (this.originType.HasValue))
            {
                ServicePointManager.SecurityProtocol = this.originType.Value;
                this.originType = null;
            }
        }

        #endregion
    }
}

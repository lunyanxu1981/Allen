using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace SeleniumConsoleApp
{
    class Program
    {
        private static void ImportOfficialAccountUsers(WechatAppType appType)
        {
            Console.WriteLine($"{appType.ToString()} followers import starts!");
            string tokenValue = RefreshAccessToken(appType);
            if (string.IsNullOrEmpty(tokenValue))
            {
                Console.WriteLine("Failed in getting access token!");
                return;
            }
            
            Task<UserListResponse> userListTask = GetUserList(appType, tokenValue);
            userListTask.Wait();
            int count = userListTask.Result.Count;
            string nextOpenId = userListTask.Result.NextOpenId;
            Console.WriteLine($"Total: {userListTask.Result.Total}");
            Console.WriteLine($"Count: {count}");
            Console.WriteLine($"NextOpenId: {userListTask.Result.NextOpenId}");
            List<string> openIds = userListTask.Result.Data.OpenIds;

            tokenValue = SaveToDatabase(tokenValue, openIds, appType);
            while (userListTask.Result.Count > 0)
            {
                userListTask = GetUserList(appType, tokenValue, nextOpenId);
                userListTask.Wait();
                count += userListTask.Result.Count;
                nextOpenId = userListTask.Result.NextOpenId;
                Console.WriteLine($"Count: {count}");
                Console.WriteLine($"NextOpenId: {userListTask.Result.NextOpenId}");
                if (userListTask.Result.Count > 0)
                {
                    openIds = userListTask.Result.Data.OpenIds;
                    tokenValue = SaveToDatabase(tokenValue, openIds, appType);
                }
            }
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.WriteLine($"{appType.ToString()} followers import is done!");

        }

        static void Main(string[] args)
        {
            #region Test
            //Test Selenium
            // IWebDriver driver = new ChromeDriver();
            //driver.Navigate().GoToUrl("https://www.travelzoo.com/cn/");
            //Test Radis
            //Console.WriteLine("Saving random data in cache");
            //SaveBigData();

            //Console.WriteLine("Reading data from cache");
            //var value = ReadData();
            //Console.WriteLine($"{value}");
            #endregion

            ImportOfficialAccountUsers(WechatAppType.SubscriptionAccount);

            Console.ReadLine();
        }

        #region Wechat Users API
        private static string SubscriptionAccountAppId = "wx291c2a757ad92bd5";
        private static string SubscriptionAccountSecret = "8bf010fa292fbd940199402fc7cb0b2c";
        private static string ServiceAccountAppId = "wx52bfb22f725f74de";
        private static string ServiceAccountSecret = "d115e25e3f6a34a7869249721fbe0835";

        /// <summary>
        /// Create access_token
        /// </summary>
        /// <returns></returns>
        private async static Task<AccessToken> GetAccessToken(WechatAppType appType)
        {
            string requestString = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={GetAppId(appType)}&secret={GetAppSecret(appType)}";
            HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
            string response = await r.GetAsync(requestString).ConfigureAwait(false);
            AccessToken rtn = JsonConvert.DeserializeObject<AccessToken>(response);

            return rtn;
        }

        private  static string RefreshAccessToken(WechatAppType appType)
        {
            Task<AccessToken> tokenTask =  GetAccessToken(appType);
            tokenTask.Wait();
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.WriteLine("Renew Access token:");
            Console.WriteLine(tokenTask?.Result?.Token ?? "Null");
            return tokenTask?.Result?.Token ?? "Null";
        }

        private static string GetAppId(WechatAppType appType)
        {
            switch (appType)
            {
                case WechatAppType.SubscriptionAccount:
                    return SubscriptionAccountAppId;
                case WechatAppType.ServiceAccount:
                    return ServiceAccountAppId;
                default:
                    return string.Empty;
            }
        }

        private static string GetAppSecret(WechatAppType appType)
        {
            switch (appType)
            {
                case WechatAppType.SubscriptionAccount:
                    return SubscriptionAccountSecret;
                case WechatAppType.ServiceAccount:
                    return ServiceAccountSecret;
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Get full user list  (10000 openid)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="nextOpenId"></param>
        /// <returns></returns>
        private async static Task<UserListResponse> GetUserList(WechatAppType appType, string accessToken, string nextOpenId = null)
        {
            bool isCompleted = false;
            int returnCount = 0;
            while (returnCount < 3 && !isCompleted)
            {
                string requestString = $"https://api.weixin.qq.com/cgi-bin/user/get?access_token={accessToken}&next_openid={nextOpenId ?? string.Empty}";
                HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
                string response = await r.GetAsync(requestString).ConfigureAwait(false);
                UserListResponse rtn = JsonConvert.DeserializeObject<UserListResponse>(response);
                if (rtn.IsSuccess)
                {
                    return rtn;
                }
                else
                {
                    ++returnCount;
                    accessToken = RefreshAccessToken(appType);
                }
            }
            return null;
        }

        /// <summary>
        /// Get batch users info (100 user info)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIds"></param>
        /// <returns></returns>
        private async static Task<BatchGetResponse> GetUserInfoList(WechatAppType appType, string accessToken, List<string> openIds)
        {
            BatchGetRequest batch = new BatchGetRequest()
            {
                UserList = new List<UserGetRequest>()
            };
            foreach (var openId in openIds)
            {
                batch.UserList.Add(new UserGetRequest() { Language = "zh_CN", OpenId = openId });
            }
            
            string requestData = JsonConvert.SerializeObject(batch);

            bool isCompleted = false;
            int returnCount = 0;
            while (returnCount < 3 && !isCompleted)
            {
                string requestString = $"https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={accessToken}";
                HttpWebRequester r = new HttpWebRequester(Encoding.UTF8);
                r.ContentType = "application/json";
                string response = await r.PostAsync(requestData, requestString).ConfigureAwait(false);
                BatchGetResponse rtn = JsonConvert.DeserializeObject<BatchGetResponse>(response);
                if (rtn.IsSuccess)
                {
                    return rtn;
                }
                else
                {
                    ++returnCount;
                    accessToken = RefreshAccessToken(appType);
                }
            }
            return null;
        }

        /// <summary>
        /// Save to staging table
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openIds"></param>
        /// <param name="appType"></param>
        private static string SaveToDatabase(string accessToken, List<string> openIds, WechatAppType appType)
        {
            List<List<string>> lists = openIds.SplitList(100)?.ToList();
            if (lists != null)
            {
                foreach (var openIdBat in lists)
                {
                    bool isCompleted = false;
                    int returnCount = 0;
                    while(returnCount < 3 && !isCompleted)
                    { 
                        Task<BatchGetResponse> batchGetUsersInfoTask = GetUserInfoList(appType, accessToken, openIdBat);
                        batchGetUsersInfoTask.Wait();
                        if (batchGetUsersInfoTask.Result.IsSuccess)
                        {
                            isCompleted = true;
                            var userInfoList = batchGetUsersInfoTask?.Result?.UserInfoList;
                            var wechatUsersList = userInfoList.
                                Select(o => new members_g_wechat_staging()
                                {
                                    AppType = (int)appType,
                                    OpenId = o.OpenId,
                                    UnionId = o.UnionId,
                                    Subscribe = o.Subscribe == 1 ? true : false,
                                    Nickname = o.Nickname,
                                    Sex = o.Sex,
                                    Language = o.Language,
                                    City = o.City,
                                    Province = o.Province,
                                    Country = o.Country,
                                    HeadimgUrl = o.HeadImgURL,
                                    SubscribeTime = o.SubscribeTime,
                                    GroupId = o.GroupId,
                                    TagIdList = o.TagIdList == null ? string.Empty : String.Join(", ", o.TagIdList),
                                    SubscribeScene = o.SubscribeScene,
                                    QRScene = o.QRScene,
                                    QRSceneStr = o.QRSceneStr,
                                    Remark = o.Remark,
                                    Created = DateTime.UtcNow
                                }).ToList();

                            var ctx = new Travelzoo1Entities();
                            try
                            {
                                //ctx.Configuration.AutoDetectChangesEnabled = false;
                                //foreach (var user in wechatUsersList)
                                //{
                                //    ctx.members_g_wechat_staging.Add(user);

                                //}
                                ctx.BulkInsert(wechatUsersList);
                                //ctx.SaveChanges();
                                
                            }
                            finally
                            {
                                //ctx.Configuration.AutoDetectChangesEnabled = true;
                                Console.WriteLine(DateTime.Now.ToLongTimeString());
                            }
                        }
                        else
                        {
                            ++returnCount;
                            accessToken = RefreshAccessToken(appType);
                        }
                    }
                }
            }

            return accessToken;
        }
        #endregion

        #region Redis

        private static string ReadData()
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            var value = cache.StringGet("TestKey");
            return value;
        }

        public static void SaveBigData()
        {

            var cache = RedisConnectorHelper.Connection.GetDatabase();


            cache.StringSet("TestKey", "Hello World!");

        }

        #endregion
    }

    #region Wechat POCO class

    public static class ListStringExtension
    {
        public static IEnumerable<List<string>> SplitList(this List<string> list, int size=100)
        {
            if (list == null || list.Count <= 0)
                yield return null;
            for (int i = 0; i < list.Count; i += size)
            {
                yield return list.GetRange(i, Math.Min(size, list.Count - i));
            }
        }
    }

    public enum WechatAppType
    {
        SubscriptionAccount = 1,
        ServiceAccount = 2,
        MiniProgram = 3
    }

    public class UserListResponse : WechatResponseBase
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public OpenIdList Data { get; set; }

        [JsonProperty("next_openid")]
        public string NextOpenId { get; set; }
    }

    public class OpenIdList
    {
        [JsonProperty("openid")]
        public List<string> OpenIds { get; set; }
    }

    public class BatchGetRequest
    {
        [JsonProperty("user_list")]
        public List<UserGetRequest> UserList { get; set; }
    }

    public class BatchGetResponse : WechatResponseBase
    {
        [JsonProperty("user_info_list")]
        public List<UserInfo> UserInfoList { get; set; }
    }

    public class UserGetRequest
    {
        [JsonProperty("openid")]
        public string OpenId { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("subscribe")]
        public int Subscribe { get; set; }

        [JsonProperty("openid")]
        public string OpenId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("sex")]
        public int Sex { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("headimgurl")]
        public string HeadImgURL { get; set; }

        [JsonProperty("subscribe_time")]
        public int SubscribeTime { get; set; }

        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("groupid")]
        public int GroupId { get; set; }

        [JsonProperty("tagid_list")]
        public List<string> TagIdList { get; set; }

        [JsonProperty("subscribe_scene")]
        public string SubscribeScene { get; set; }

        [JsonProperty("qr_scene")]
        public int QRScene { get; set; }

        [JsonProperty("qr_scene_str")]
        public string QRSceneStr { get; set; }
    }

    public class AccessToken : WechatResponseBase
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiredSeconds { get; set; }
    }

    public class WechatResponseBase
    {
        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }

        public bool IsSuccess
        {
            get
            {
                return (this.ErrorCode == 0);
            }
        }

        public WechatResponseBase()
        {
            this.ErrorCode = 0;
        }
    }

    #endregion
}

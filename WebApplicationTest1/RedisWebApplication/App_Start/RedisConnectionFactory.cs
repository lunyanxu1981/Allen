using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RedisWebApplication.App_Start
{
    public class RedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        private static readonly object SyncLock = new object();

        private static readonly string REDIS_CONNECTIONSTRING = "CacheConnection";

        

        public static IDatabase GetDatabase()
        {
            if (lazyConnection == null || !lazyConnection.IsValueCreated || !lazyConnection.Value.IsConnected || !lazyConnection.Value.GetDatabase().IsConnected(default(RedisKey)))
            {
                lock (SyncLock)
                {
                    try
                    {
                        lazyConnection = new Lazy<ConnectionMultiplexer>(
                        () =>
                        {
                            var options = ConfigurationOptions.Parse(ConfigurationManager.AppSettings[REDIS_CONNECTIONSTRING].ToString());
                            options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                            return ConnectionMultiplexer.Connect(options);
                        });
                    }
                    catch {}
                    
                }
                    
            }

            return lazyConnection.IsValueCreated ? lazyConnection.Value.GetDatabase() : null;
        }



        public static void Dispose()
        {
            if (lazyConnection.IsValueCreated)
            {
                lazyConnection.Value.Dispose();
            }
        }
    }
}
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisWebApplication.App_Start
{
    public class RedisAgent
    {
        private static IDatabase _database;

        public RedisAgent()
        {
            var connection = RedisConnectionFactory.GetDatabase();
        }

        public RedisValue GetRedisItem(string key)
        {
            try
            {
                return _database.StringGet(key);
            }
            catch(Exception ex)
            {
                return RedisValue.Null;
            }
        }

        public bool SetRedisItem(string key, RedisValue value)
        {
            try
            {
               return _database.StringSet(key, value);
            }
            catch
            {
                return false;
            }
            
        }

        public bool DeleteRedisItem(string key)
        {
            try
            {
                return _database.KeyDelete(key);
            }
            catch
            {
                return false;
            }
        }

        public RedisResult ExcuteRedisCommand(string command, params object[]args)
        {
            try
            {
               return _database.Execute(command, args);
            }
            catch
            {
                return null;
            }
        }
    }
}
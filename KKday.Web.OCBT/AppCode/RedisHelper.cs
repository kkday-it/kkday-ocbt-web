using System;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace KKday.Web.OCBT.AppCode
{
    public interface IRedisHelper
    {
        void SetRedis(string obj, string redisKey, int expireMinute);

        string getRedis(string redisKey);

        void delRedis(string guidKey, string redisKey);

        void Push(RedisKey stackName, RedisValue value);

        RedisValue Pop(RedisKey stackName);

        //IRedisHelper reChkConnect();
        ConnectionMultiplexer reChkConnect();
    }


    public class RedisHelper : IRedisHelper
    {
        public ConnectionMultiplexer kkrds { get; set; }
        public string redisConnectStr { get; private set; }

        public RedisHelper(IConfiguration configuration)
        {
            //redis 
            this.redisConnectStr = configuration["Redis:kkday"];
            kkrds = ConnectionMultiplexer.Connect(redisConnectStr);
        }

        //存到redis
        public void SetRedis(string obj, string redisKey, int expireMinute)
        {
            try
            {
                //kkredis  
                this.kkrds = reChkConnect();
                IDatabase db = kkrds.GetDatabase();
                db.StringSet(redisKey, obj, TimeSpan.FromMinutes(expireMinute));
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"setRedis_error:" + ex.Message.ToString());
            }
        }
        public void delRedis(string guidKey, string redisKey)
        {
            try
            {
                //kkredis  
                this.kkrds = reChkConnect();
                IDatabase db = kkrds.GetDatabase();
                db.KeyDelete(redisKey);

                //刷新redis server 
                Clear();
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"guidKey : {guidKey}; delRedis_error:" + ex.Message.ToString());
            }
        }
        public void Clear()
        {
            var endpoints = kkrds.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = kkrds.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }
        public string getRedis(string redisKey)
        {
            try
            {
                //kkredis  
                this.kkrds = reChkConnect();
                IDatabase db = kkrds.GetDatabase();

                string obj = db.StringGet(redisKey);
                return obj;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"getRedis_error:" + ex.Message.ToString());
                return null;
            }
        }

        public ConnectionMultiplexer reChkConnect()
        {
            if (kkrds == null)
            {
                kkrds = ConnectionMultiplexer.Connect(this.redisConnectStr);
            }
            return kkrds;
        }

        public void Push(RedisKey stackName, RedisValue value)
        {
            this.kkrds = reChkConnect();
            this.kkrds.GetDatabase().ListRightPush(stackName, value);
        }
        public RedisValue Pop(RedisKey stackName)
        {
            this.kkrds = reChkConnect();
            return this.kkrds.GetDatabase().ListLeftPop(stackName);
        }
    }
}

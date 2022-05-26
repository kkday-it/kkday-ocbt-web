using System;
using System.Threading;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KKday.Web.OCBT.Service
{
    public class OrderBackgroundService:IHostedService
    {
        private readonly IConfiguration _configuration;
        private IRedisHelper _redisHelper;
        public OrderBackgroundService(IConfiguration configuration,IRedisHelper redisHelper)
        {
            _configuration = configuration;
            _redisHelper = redisHelper;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_configuration["Switch"] == "ON")
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var keyforOrder = "ComboBookingKeys";
                    var msg = _redisHelper.Pop(keyforOrder);
                    //ComboBookingFlow(msg);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KKday.Web.OCBT.Service
{
    
    public class OrderBackgroundService:IHostedService,IDisposable
    {
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private Task _executingTask;
        private Timer _timer;
        private readonly IConfiguration _configuration;
        private IRedisHelper _redisHelper;
        private KKday.Web.OCBT.Models.Repository.ComboBookingRepository _comboRepos;
        public OrderBackgroundService(IConfiguration configuration,IRedisHelper redisHelper, KKday.Web.OCBT.Models.Repository.ComboBookingRepository comboRepos)
        {
            _configuration = configuration;
            _redisHelper = redisHelper;
            _comboRepos = comboRepos;
        }
        private void DoWorkAsync(Object state)
        {
            //Scope Service
            var keyforOrder = "ComboBookingKeys";
            var msg = _redisHelper.Pop(keyforOrder);
            if (!string.IsNullOrEmpty(msg))
            {
                _comboRepos.ComboBookingFlow(msg);

            }
            
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _timer = new Timer(DoWorkAsync, cancellationToken, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return Task.CompletedTask;
            }
            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
            }
            return Task.CompletedTask;
        }
        public virtual void Dispose()
        {

        }
        
    }
}

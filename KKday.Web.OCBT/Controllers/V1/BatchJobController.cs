using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Repository;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class BatchJobController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly BatchJobRepository _batchJobRepos;
        public BatchJobController(IRedisHelper redisHelper, BatchJobRepository batchJobRepos)
        {
            _redisHelper = redisHelper;
            _batchJobRepos = batchJobRepos;
        }

        [HttpGet("SetParentOrderStatusBack")]
        public IActionResult SetParentOrderStatusBack()
        {
            try
            {
                Task.Run(() => DoChkParentOrder());
                return StatusCode(200, true);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboBooking_SetParentOrderStatusBack_exception:Message={ex.Message}, StackTrace={ex.StackTrace}");
                return StatusCode(400, false);
            }
        }


        private async void DoChkParentOrder()
        {
            string guidKey = Guid.NewGuid().ToString();

            try
            {
                //TimeSpan e = new TimeSpan(0, 1, 0);
                //Thread.Sleep(e);

                _batchJobRepos.SetParentBack(guidKey);

                var dd = "test";
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboBooking_DoChkParentOrder_exception:GuidKey={guidKey}, Message={ex.Message}, StackTrace={ex.StackTrace}");

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Repository;
using KKday.Web.OCBT.Models.Model.DataModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class BatchJobController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly BatchJobRepository _batchJobRepos;
        private readonly OrderRepository _orderRepos;
        private readonly SlackHelper _slack;
        private readonly ComboBookingRepository _comboBookRepos;
        public BatchJobController(IRedisHelper redisHelper, BatchJobRepository batchJobRepos, OrderRepository orderRepos, SlackHelper slack
            , ComboBookingRepository comboBookRepos)
        {
            _redisHelper = redisHelper;
            _batchJobRepos = batchJobRepos;
            _orderRepos = orderRepos;
            _slack = slack;
            _comboBookRepos = comboBookRepos;
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

        /// <summary>
        /// 檢查母單已超過時間但is_callback=false
        /// </summary>
        [HttpGet("CheckCallBack")]
        public void CheckCallBack()
        {
            string guidKey = Guid.NewGuid().ToString();

            try
            {
                // 取得尚未 CallBack 的母單(is_callback=false)
                var master = _orderRepos.GetTimeOutMaster();
                if (master.result == "0000" && master.count > 0)
                {
                    master.order_mst_list?.ForEach(x =>
                    {
                        // 1. 觸發 CallBackJava
                        RequestJson callBackJson = new RequestJson
                        {
                            orderMid = x.order_mid,
                            requestUuid = guidKey,
                            metadata = new RequesteMetaModel
                            {
                                status = "2010",
                                description = string.IsNullOrEmpty(x.monitor_start_datetime) ? "成立子單失敗" : "取憑證時效過期或成立子單失敗"
                            }
                        };
                        _comboBookRepos.CallBackJava(callBackJson);
                    });
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"CheckCallBack_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
                _slack.SlackPost(guidKey, "CheckCallBack", "BatchJobController/CheckCallBack", $"檢查已超時但尚未CallBack的母單異常！", $"Msg={ex.Message}, StackTrace={ex.StackTrace}");
            }
        }
    }
}

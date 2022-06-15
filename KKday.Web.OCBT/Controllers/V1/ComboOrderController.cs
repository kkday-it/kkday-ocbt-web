using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using KKday.Web.OCBT.Models.Repository;
using KKday.Web.OCBT.Models.Model.Order;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class ComboOrderController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly OrderRepository _orderRepos;
        public ComboOrderController(IRedisHelper redisHelper, OrderRepository comboSupRepos)
        {
            _redisHelper = redisHelper;
            _orderRepos = comboSupRepos;
        }
        // GET: api/values
        [HttpPost("NotifyParentMemo")]
        public OrderMemoResponseModel NotifyParentMemo([FromBody] OrderMemoRequstModel request)
        {
            try
            {
                _orderRepos.NotifyParentMemo(request);

                return new OrderMemoResponseModel
                {
                    metadata = new ResponseMetaModel
                    {
                        status = "1000",
                        description = "正確無誤"
                    }
                };//立即return 回java
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"OComboBooking_NotifyParentMemo_exception:GuidKey={request?.requestUuid}, Message={ex.Message}, StackTrace={ex.StackTrace}");
                return new OrderMemoResponseModel
                {
                    metadata = new ResponseMetaModel
                    {
                        status = "9999",
                        description = "系統異常"+ex.Message
                    }
                };//立即return 回java
            }
        }

    }
}

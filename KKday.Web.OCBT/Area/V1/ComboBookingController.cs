using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKday.Web.B2D.N3.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.Area.V1
{
    [Route("api/v1/[controller]")]
    public class ComboBookingController : Controller
    {
        private IRedisHelper _redisHelper;
        public ComboBookingController(IRedisHelper redisHelper)
        {
            _redisHelper = redisHelper;
        }
        // GET: api/values
        [HttpPost("CrtOrder")]
        public ResponseJavaModel CrtOrder([FromBody] BookingRequestModel request)
        {
            _redisHelper.Push("ComboBookingKeys", JsonConvert.SerializeObject(request));//將Java資料傳入redisQueue
            return new ResponseJavaModel {
                metadata=new ResponseMetaModel {
                    status= "1000",
                    description="正確無誤"
                }
            };//立即return 回java
        }

    }
}

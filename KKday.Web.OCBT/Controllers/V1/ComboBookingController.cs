using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using KKday.Web.OCBT.Models.Repository;
using KKday.Web.OCBT.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class ComboBookingController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly ComboSupplierRepository _comboSupRepos;
        private readonly AmazonS3Service _amazonS3Service;
        public ComboBookingController(IRedisHelper redisHelper, ComboSupplierRepository comboSupRepos, AmazonS3Service amazonS3Service)
        {
            _redisHelper = redisHelper;
            _comboSupRepos = comboSupRepos;
            _amazonS3Service = amazonS3Service;
        }
        // GET: api/values
        [HttpPost("CrtOrder")]
        public ResponseJson CrtOrder([FromBody] BookingRequestModel request)
        {
            _redisHelper.Push("ComboBookingKeys", JsonConvert.SerializeObject(request));//將Java資料傳入redisQueue
            return new ResponseJson
            {
                metadata=new ResponseMetaModel {
                    status= "1000",
                    description="正確無誤"
                }
            };//立即return 回java
        }


        [HttpGet("ThrowQueue")]
        public string throwQueue(string order_mid)
        {
            var pushData = new 
            {
                master_order_mid=order_mid
            };
            _redisHelper.Push("ComboBookingVoucher", JsonConvert.SerializeObject(pushData));//將Java資料傳入redisQueue
            return "OK";
        }


        [HttpGet("TestSetVoucher")]
        public string TestSetVoucher(string order_mid)
        {
            var _order = HttpContext.RequestServices.GetService<OrderRepository>();
            //1.查詢憑證List
            var voucherList = _order.QueryVouchers(order_mid);
            if (voucherList.file.Count > 0)
            {
                voucherList.file.ForEach(x =>
                {
                    // 2. 下載憑證至memory
                    var file = _order.DownloadVoucher(order_mid, x.order_file_id);
                    if (file.result == "00" && file.result_msg == "OK")
                    {
                        var a = file.file.FirstOrDefault().content_type;
                        byte[] bytes = Convert.FromBase64String(file.file.First().encode_str);
                        // 3. 上傳至 s3 (必須為PDF)
                        var upload = _amazonS3Service.UploadObject(x.file_name, "application/pdf", bytes).Result;
                    }
                });
            }
            return "OK";
        }

    }
}

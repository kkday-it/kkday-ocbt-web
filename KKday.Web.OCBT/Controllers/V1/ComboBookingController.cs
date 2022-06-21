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

        /// <summary>
        /// Get From S3, Byte[] Convert To Base64
        /// </summary>
        /// <returns></returns>
        [HttpGet("ConvertBase64")]
        //[HttpPost("ConvertBase64")]
        public ConvertBase64Rs ConvertBase64(RequestJson rq)
        {
            ConvertBase64Rs rs = new ConvertBase64Rs();
            rs.metadata = new ResponseMetaModel
            {
                status = "3002",
                description = "回傳檔案失敗"
            };

            try
            {
                // Get From S3
                var getByte = _amazonS3Service.GetObject(rq.fileUrl).Result;
                if (getByte != null)
                {
                    if (getByte.Success)
                    {
                        rs.metadata.description = $"Json = {JsonConvert.SerializeObject(getByte)} , ";
                        // Byte[] Convert to Base64
                        rs.data.base64str = Convert.ToBase64String(getByte.DataBytes);
                        rs.metadata.status = "3001";
                        //rs.metadata.description = "回傳檔案成功";
                    }
                }
            }
            catch (Exception ex)
            {
                //Website.Instance.logger.Fatal($"ComboBooking_ChkCancel_exception:GuidKey ={rq?.request_uuid}, Message={ex.Message}, StackTrace={ex.StackTrace}");
                rs.metadata.description += $"Msg = {ex.Message} , StackTrace = {ex.StackTrace}";
            }

            return rs;
        }

        [HttpGet("TestSetVoucher")]
        public ResponseJson TestSetVoucher(string order_mid)
        {
            ResponseJson rs = new ResponseJson();
            rs.metadata = new ResponseMetaModel { status = "FAIL" };
            try
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
                            if (upload.Success)
                            {
                                rs.metadata.description = $"S3 Key = {upload.FileName}";
                                rs.metadata.status = upload.ToString();
                            }

                            var base64str = Convert.ToBase64String(bytes);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                rs.metadata.description = $"Exception : Msg={ex.Message} , StackTrace={ex.StackTrace}";
            }
            return rs;
        }

    }
}

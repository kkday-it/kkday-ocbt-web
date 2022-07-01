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
using KKday.Web.OCBT.Models.Model.Order;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class ComboBookingController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly ComboBookingRepository _comboRepos;
        private readonly AmazonS3Service _amazonS3Service;
        private readonly SlackHelper _slack;
        public ComboBookingController(IRedisHelper redisHelper, ComboBookingRepository comboRepos, AmazonS3Service amazonS3Service, SlackHelper slack)
        {
            _redisHelper = redisHelper;
            _comboRepos = comboRepos;
            _amazonS3Service = amazonS3Service;
            _slack = slack;

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
        public string throwQueue(string order_mid,string request_uuid)
        {
            var pushData = new 
            {
                master_order_mid=order_mid,
                request_uuid= Guid.NewGuid().ToString()
        };
            _redisHelper.Push("ComboBookingVoucher", JsonConvert.SerializeObject(pushData));//將Java資料傳入redisQueue
            return "OK";
        }

        /// <summary>
        /// Get From S3, Byte[] Convert To Base64
        /// </summary>
        /// <returns></returns>
        [HttpPost("ConvertBase64")]
        public ConvertBase64Rs ConvertBase64([FromBody] ConvertBase64Rq rq)
        {
            Website.Instance.logger.Info($"ConvertBase64 start = {JsonConvert.SerializeObject(rq)}",rq?.requestUuid);
            string guidKey = Guid.NewGuid().ToString();
            ConvertBase64Rs rs = new ConvertBase64Rs();
            rs.metadata = new ResponseMetaModel
            {
                status = "3002",
                description = "回傳檔案失敗:"
            };

            try
            {
                if (string.IsNullOrEmpty(rq?.fileUrl))
                {
                    rs.metadata.description += "fileUrl can not null";
                }
                else
                {
                    // Rq Log
                    Website.Instance.logger.Info($"ConvertBase64 Start Get S3: FileName = {rq.fileUrl}", rq?.requestUuid);

                    // Get From S3
                    var getByte = _amazonS3Service.GetObject(rq.fileUrl).Result;
                    // Rs Log
                    Website.Instance.logger.Info($"ConvertBase64 Get S3 Rs = {JsonConvert.SerializeObject(getByte)}", rq?.requestUuid);

                    if (getByte != null)
                    {
                        if (getByte.Success)
                        {
                            rs.metadata.status = "3001";
                            rs.metadata.description = "回傳檔案成功";
                            // Byte[] Convert to Base64
                            rs.data = new ResponseDataModel
                            {
                                base64str = Convert.ToBase64String(getByte.DataBytes)
                            };
                        }
                        else
                        {
                            _slack.SlackPost(guidKey, "ConvertBase64", "ComboBookingController/ConvertBase64", $"不存在此檔！", $"{JsonConvert.SerializeObject(rq)}");
                            // Get S3 Fail
                            rs.metadata.description = "不存在此檔案";
                        }
                    }

                    try
                    {
                        // 取出 母+子 單 Xid
                        var xid = _comboRepos.GetBookingDtlInfo(rq);
                        if (xid != null)
                        {
                            // 先將狀態CB
                            _comboRepos.UpdateDtlVoucherStatus(xid.booking_dtl_xid, "CB", rq?.requestUuid);

                            var updGL = _comboRepos.UpdateDtlVoucherStatus(xid.booking_dtl_xid, "GL", rq?.requestUuid);
                            // Check All Dtl GL then Update Mst.Status GL
                            var dtlList = _comboRepos.QueryBookingDtl(xid.booking_mst_xid);
                            if (dtlList?.Count > 0)
                            {
                                var glList = dtlList.Where(s => s.booking_dtl_voucher_status == "GL")?.Count() ?? 0;
                                if (dtlList.Count == glList)
                                {
                                    // Update Mst.Status GL 
                                    var updMstGL = _comboRepos.UpdateMstVoucherStatus(xid.booking_mst_xid, "GL");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _slack.SlackPost(guidKey, "ConvertBase64", "ComboBookingController/ConvertBase64", $"找不到booking_dtl！", $"{JsonConvert.SerializeObject(rq)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboBooking_ConvertBase64_Exception:GuidKey ={rq?.requestUuid}, Message={ex.Message}, StackTrace={ex.StackTrace}", rq?.requestUuid);
                rs.metadata.description += $" Msg = {ex.Message} , StackTrace = {ex.StackTrace}";
            }

            return rs;
        }

        /// <summary>
        /// 檢查來自 B2D Webhook 通知的單是否為OCBT子單
        /// </summary>
        /// <param name="rq"></param>
        [HttpPost("CheckOrderFromB2D")]
        public void CheckOrderFromB2D([FromBody] CheckOrderFromB2dRqModel rq)
        {
            try
            {
                if (!string.IsNullOrEmpty(rq?.result_type))
                {
                    if (rq?.result_type == "order" && !string.IsNullOrEmpty(rq.order?.order_no))
                    {
                        // 檢查是否為OCBT子單，同時update 為處理中
                        _comboRepos.CheckOrderFromB2d(rq.order.order_no);
                    }
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboBooking_CheckOrderFromB2D_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
            }
        }
    }
}
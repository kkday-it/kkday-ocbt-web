using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using KKday.Web.OCBT.Models.Repository;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace KKday.Web.OCBT.Service
{
    public class VoucherBackgroundService : IHostedService, IDisposable
    {
        private readonly OrderRepository _orderRepos;
        private readonly ComboBookingRepository _comboBookRepos;
        private IRedisHelper _redisHelper;
        private readonly AmazonS3Service _amazonS3Service;
        private readonly SlackHelper _slack;
        private Timer _timer1;

        public VoucherBackgroundService(OrderRepository orderRepos, ComboBookingRepository comboBookRepos, IRedisHelper redisHelper
            , AmazonS3Service amazonS3Service, SlackHelper slack)
        {
            _orderRepos = orderRepos;
            _comboBookRepos = comboBookRepos;
            _redisHelper = redisHelper;
            _amazonS3Service = amazonS3Service;
            _slack = slack;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (Website.Instance.Configuration["Switch"] == "ON")
            {
                // Get Voucher
                _timer1 = new Timer(FetchQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            }

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public async void FetchQueue(Object state)
        {
            try
            {
                // Get From RedisQ
                var keyforOrder = "ComboBookingVoucher";
                var getQueue = _redisHelper.Pop(keyforOrder);
                Website.Instance.logger.Info($"Voucher Background Service Start!!");

                if (getQueue.HasValue)
                {
                    var orderMid = JsonConvert.DeserializeObject<QueueModel>(getQueue)?.master_order_mid;
                    if (!string.IsNullOrEmpty(orderMid)) await Task.Run(() => DoWork(orderMid));
                }
            }
            catch(Exception ex)
            {
                Website.Instance.logger.Info($"FetchQueue Exception: Msg={ex.Message}, StackTrace={ex.StackTrace}");
            }
        }
        /// <summary>
        /// Get Voucher
        /// </summary>
        /// <param name="state"></param>
        private async void DoWork(string orderMid)
        {
            string guidKey = Guid.NewGuid().ToString();

            try
            {
                Website.Instance.logger.Info($"DoWork Start Voucher: order_mid={orderMid}");
                while (true)
                {
                    // 取出母單
                    var main = _orderRepos.QueryBookingMst(orderMid);
                    Website.Instance.logger.Info($"Voucher Get Order Master: {JsonConvert.SerializeObject(main)}");
                    if (main != null)
                    {
                        // 暫定=>沒填 Default 等待20min
                        var deadLine = main.voucher_deadline == 0 ? 20 : main.voucher_deadline;
                        var voucherDeadline = Convert.ToDateTime(main.monitor_start_datetime).AddMinutes(deadLine);
                        // 判斷是否過時
                        if (DateTime.Now > voucherDeadline)
                        {
                            // Update booking_mst_voucher_status=FAIL & is_callback=true
                            var updVoucher = _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "FAIL", "true");
                            // CallBackJava
                            RequestJson callBackJson = new RequestJson
                            {
                                orderMid = main.order_mid,
                                metadata = new RequesteMetaModel
                                {
                                    status = "2011",
                                    description = "超過等待時間"
                                }
                            };
                            _comboBookRepos.CallBackJava(callBackJson);
                            // 時間超過後結束回圈
                            break;
                        }
                        else
                        {
                            // 取得所有待處理子單(PROCESS)
                            var subOrder = _orderRepos.QueryBookingDtl(main.booking_mst_xid, "PROCESS");
                            if (subOrder.result == "0000" && subOrder.count > 0)
                            {
                                var processOrder = subOrder.order_dtl_list.Where(s => s.booking_dtl_voucher_status == "PROCESS")?.Select(x => x.order_mid).ToArray();
                                // Call WMS: 取訂單明細
                                var _subOrders = _orderRepos.QueryOrders(processOrder);
                                foreach (var sub in _subOrders?.order)
                                {
                                    if (sub.orderStatus == "GO_OK")
                                    {
                                        // 1. 查詢憑證List
                                        var voucherList = _orderRepos.QueryVouchers(sub.orderMid);
                                        if (voucherList.file.Count > 0)
                                        {
                                            List<string> fileInfo = new List<string>();
                                            voucherList.file.ForEach(x =>
                                            {
                                                // 2. 下載憑證至memory
                                                var file = _orderRepos.DownloadVoucher(sub.orderMid, x.order_file_id);
                                                if (file.result == "00" && file.result_msg == "OK")
                                                {
                                                    byte[] bytes = Convert.FromBase64String(file.file.First().encode_str);
                                                    // 3. 上傳至 s3
                                                    var upload = _amazonS3Service.UploadObject(x.file_name, "application/pdf", bytes).Result;
                                                    if (upload.Success) fileInfo.Add(x.file_name);
                                                }
                                            });

                                            // 4. 更改子單voucher_status='VOUCHER_OK'
                                            var updVoucher = _orderRepos.UpdateDtlVoucherStatus(sub.orderMid, "VOUCHER_OK", fileInfo);
                                        }
                                        else
                                        {
                                            // 取無憑證=>紀錄Kinbana Log 
                                            Website.Instance.logger.Fatal($"NonVoucher MainOrderMid={main.order_mid}, SubOrderMid={sub.orderMid} ");
                                        }
                                    }
                                }
                            }
                            // 重新檢查是否所有子單憑證到齊，到齊CallBack Java
                            if (_orderRepos.CheckDtl(main.booking_mst_xid, main.order_mid))
                            {
                                // 所有子單憑證到齊=>修改母單訂單狀態
                                _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "VOUCHER_OK", "true");
                                // Is True 代表所有憑證到齊且CallBack Java => End;
                                break;
                            }

                            // Sleep (1min)
                            System.Threading.Thread.Sleep(new TimeSpan(0, 1, 0));
                        }
                    }
                    else
                    {
                        Website.Instance.logger.Info($"Can not get from booking_mst by {orderMid}!!");
                        break;
                    }
                }
                Website.Instance.logger.Info($"Voucher Background Service End!!");
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"VoucherBackgroundService_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
                // Send Slack
                _slack.SlackPost(guidKey, "VoucherBackgroundService", "VoucherBackgroundService/DoWork", $"{orderMid} 取憑證異常！", $"Msg={ex.Message}, StackTrace={ex.StackTrace}");
            }
        }

        public void Dispose()
        {
            _timer1?.Dispose();
        }

        public class QueueModel
        {
            public string master_order_mid { get; set; }
        }
    }
}
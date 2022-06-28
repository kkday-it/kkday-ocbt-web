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
                    await Task.Run(() => DoWork(JsonConvert.DeserializeObject<QueueModel>(getQueue)?.master_order_mid));
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
                // 取出母單
                var main = _orderRepos.QueryBookingMst(orderMid);
                if (main == null)
                {
                    Website.Instance.logger.Info($"Can not get from booking_mst by {orderMid}!!");
                    return;
                }
                Website.Instance.logger.Info($"Voucher Get Order Master: {JsonConvert.SerializeObject(main)}");

                while (true)
                {
                    // 暫定=>沒填 Default 等待20min
                    var deadLine = main.voucher_deadline == 0 ? 20 : main.voucher_deadline;
                    var voucherDeadline = Convert.ToDateTime(main.monitor_start_datetime).AddMinutes(deadLine);
                    // 判斷是否過時
                    if (DateTime.Now > voucherDeadline)
                    {
                        // 時間超過後結束回圈，交由排程接手執行
                        break;
                    }
                    else
                    {
                        // 取得所有待處理子單(PROCESS)
                        var subOrders = _orderRepos.QueryBookingDtl(main.booking_mst_xid);
                        // PROCESS + VOUCHER_OK = Total 子單
                        var totalCount = subOrders?.Count() ?? 0;
                        // 應處理的子單筆數
                        var processOrders = subOrders.Where(s => s.booking_dtl_voucher_status == "PROCESS").ToList();

                        if (processOrders?.Count() > 0)
                        {
                            var processOrderMids = processOrders?.Select(x => x.order_mid).ToArray();
                            // Call WMS: 取訂單明細
                            var _processOrderMids = _orderRepos.QueryOrders(processOrderMids);
                            foreach (var sub in _processOrderMids?.order)
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

                                        subOrders.Where(s => s.order_mid == sub.orderMid).ToList().ForEach(o =>
                                        {
                                            o.booking_dtl_voucher_status = "VOUCHER_OK";
                                            o.voucher_file_info = fileInfo;
                                        });
                                    }
                                    else
                                    {
                                        // 取無憑證=>紀錄Kinbana Log 
                                        Website.Instance.logger.Fatal($"Fetch No Voucher MainOrderMid={main.order_mid}, SubOrderMid={sub.orderMid} ");
                                    }
                                }
                            }
                        }
                        // 已處理的子單筆數
                        var vouhOkOrders = subOrders.Where(s => s.booking_dtl_voucher_status == "VOUCHER_OK").ToList();
                        // 重新檢查是否所有子單憑證到齊，到齊CallBack Java
                        if (vouhOkOrders.Count() == totalCount)
                        {
                            // 所有子單憑證到齊=>修改母單訂單狀態
                            _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "VOUCHER_OK", "false");

                            // CallBackJava Rq
                            var callBackJson = new RequestJson
                            {
                                orderMid = main.order_mid,
                                metadata = new RequesteMetaModel
                                {
                                    status = "2000",
                                    description = "OCBT取得憑證OK"
                                },
                                data = new RequestDataModel()
                            };
                            // 將檔案清單放置 CallBackJava
                            subOrders.ForEach(x =>
                            {
                                callBackJson.data.orderinfo.Add(new RequestOrderInfoModel
                                {
                                    kkOrderNo = x.order_mid,
                                    ticket = x.voucher_file_info,
                                    type = "URL",
                                    fileExtention = "PDF",
                                    result = "OK"
                                });
                            });
                            Website.Instance.logger.Info($"BS_Voucher CallBackJava Rq: {JsonConvert.SerializeObject(callBackJson)} , Order Mid = {main.order_mid} ");
                            // CallBackJava
                            _comboBookRepos.CallBackJava(callBackJson, main.order_mid);

                            // 所有子單憑證到齊=>修改母單訂單狀態
                            _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "VOUCHER_OK", "true");
                            // 代表所有憑證到齊且CallBack Java => End;
                            break;
                        }

                        // Sleep (1min)
                        System.Threading.Thread.Sleep(new TimeSpan(0, 1, 0));
                    }
                }
                Website.Instance.logger.Info($"{orderMid} Voucher Background Service End!!");
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"{orderMid} VoucherBackgroundService_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
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
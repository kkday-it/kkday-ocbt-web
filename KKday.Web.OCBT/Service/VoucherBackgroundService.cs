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

namespace KKday.Web.OCBT.Service
{
    public class VoucherBackgroundService : IHostedService, IDisposable
    {
        private readonly OrderRepository _orderRepos;
        private readonly ComboBookingRepository _comboBookRepos;
        private IRedisHelper _redisHelper;
        private readonly AmazonS3Service _amazonS3Service;
        private Timer _timer1;

        public VoucherBackgroundService(OrderRepository orderRepos, ComboBookingRepository comboBookRepos, IRedisHelper redisHelper, AmazonS3Service amazonS3Service)
        {
            _orderRepos = orderRepos;
            _comboBookRepos = comboBookRepos;
            _redisHelper = redisHelper;
            _amazonS3Service = amazonS3Service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (Website.Instance.Configuration["Switch"] == "ON")
            {
                // Get Voucher
                _timer1 = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            }

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// Get Voucher
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object state)
        {
            try
            {
                while (true)
                {
                    // Get From RedisQ
                    var keyforOrder = "ComboBookingVouchers";
                    var getQueue = _redisHelper.Pop(keyforOrder);
                    //Website.Instance.logger.Info($"Voucher GetQueue: {getQueue}");
                    if (getQueue.HasValue)
                    {
                        Website.Instance.logger.Info($"Voucher GetQueue: order_mid={JsonConvert.DeserializeObject<QueueModel>(getQueue).master_order_mid}");
                        var queue = new string[] { JsonConvert.DeserializeObject<QueueModel>(getQueue).master_order_mid };
                        // 取出母單
                        var mainOrders = _orderRepos.QueryBookingMst(queue);
                        Website.Instance.logger.Info($"Voucher Get Order Master: {JsonConvert.SerializeObject(mainOrders)}");
                        foreach (var main in mainOrders.order_mst_list)
                        {
                            // 暫定=>沒填 Default 等待20min
                            var deadLine = main.voucher_deadline == 0 ? 20 : main.voucher_deadline;
                            var voucherDeadline = Convert.ToDateTime(main.monitor_start_datetime).AddMinutes(deadLine);
                            // 取出應處理的子單
                            var subOrders = _orderRepos.FetchOrderDtlData(main.booking_mst_xid.ToString(), voucherStatus: "PROCESS").order_dtl_list?.Select(x => x.order_mid).ToArray();
                            if (subOrders != null)
                            {
                                // 應處理的子單筆數
                                var voucher_count = subOrders.Count();
                                // Call WMS: 取訂單明細
                                var _subOrders = _orderRepos.QueryOrders(subOrders);
                                foreach (var sub in _subOrders.order)
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
                                                    var a = file.file.FirstOrDefault().content_type;
                                                    byte[] bytes = Convert.FromBase64String(file.file.First().encode_str);
                                                    // 3. 上傳至 s3 (必須為PDF)
                                                    var upload = _amazonS3Service.UploadObject(x.file_name, "application/pdf", bytes).Result;
                                                    if (upload.Success) fileInfo.Add(x.file_name);
                                                }
                                            });

                                            // 4. 更改子單voucher_status='VOUCHER_OK'
                                            var updVoucher = _orderRepos.UpdateDtlVoucherStatus(sub.orderMid, "VOUCHER_OK", fileInfo);
                                            if (updVoucher.result == "0000") voucher_count--;
                                        }
                                        else
                                        {
                                            // 取無憑證=>紀錄Kinbana Log 
                                            Website.Instance.logger.Fatal($"NonVoucher MainOrderMid={main.order_mid}, SubOrderMid={sub.orderMid} ");
                                        }
                                    }
                                }
                                // 所有子單處理完畢
                                if (voucher_count == 0)
                                {
                                    var updVoucher = _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "VOUCHER_OK");
                                    // CallBackJava
                                    RequestJson callBackJson = new RequestJson
                                    {
                                        orderMid = main.order_mid,
                                        metadata = new RequesteMetaModel
                                        {
                                            status = "2000",
                                            description = "OCBT取得憑證OK"
                                        }
                                    };
                                    _comboBookRepos.CallBackJava(callBackJson);
                                }
                                // 現在時間是否超過等待時間
                                else if (DateTime.Now > voucherDeadline)
                                {
                                    var updVoucher = _orderRepos.UpdateMstVoucherStatus(main.booking_mst_xid, "FAIL");
                                    // CallBackJava
                                    RequestJson callBackJson = new RequestJson
                                    {
                                        orderMid = main.order_mid,
                                        metadata = new RequesteMetaModel
                                        {
                                            status = "",
                                            description = ""
                                        }
                                    };
                                    _comboBookRepos.CallBackJava(callBackJson);
                                }

                                // Sleep (1min)
                                System.Threading.Thread.Sleep(new TimeSpan(0, 1, 0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"VoucherBackgroundService_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
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

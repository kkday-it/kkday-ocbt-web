using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using KKday.Web.OCBT.Models.Repository;
using Microsoft.Extensions.Hosting;

namespace KKday.Web.OCBT.Service
{
    public class VoucherBackgroundService : IHostedService, IDisposable
    {
        private readonly OrderRepository _orderRepos;
        private readonly ComboBookingRepository _comboBookRepos;
        private IRedisHelper _redisHelper;
        private readonly AmazonS3Service _amazonS3Service;
        private Timer _timer1;
        private Timer _timer2;

        public VoucherBackgroundService(OrderRepository orderRepos, ComboBookingRepository comboBookRepos, IRedisHelper redisHelper, AmazonS3Service amazonS3Service)
        {
            _orderRepos = orderRepos;
            _comboBookRepos = comboBookRepos;
            _redisHelper = redisHelper;
            _amazonS3Service = amazonS3Service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var keyforOrder = "ComboBookingVoucher";
            var msg = _redisHelper.Pop(keyforOrder);
            // Get Voucher
            _timer1 = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            // 每分鐘檢查=>母單已超過時間但is_callback=false
            _timer2 = new Timer(DoWork2, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                while (true)
                {
                    // 模擬Dequeue from ConcurrentList<T>
                    var queue = new string[] { "22KK272931961" };
                    // 取出母單
                    var mainOrders = _orderRepos.QueryBookingMst(queue);
                    foreach(var main in mainOrders.order_mst_list)
                    {
                        var voucherDeadline = Convert.ToDateTime(main.monitor_start_datetime).AddMinutes(main.voucher_deadline);

                        // 取出應處理的子單
                        var subOrders = _orderRepos.FetchOrderDtlData(main.booking_mst_xid.ToString(), voucherStatus: "PROCESS").order_dtl_list?.Select(x => x.order_mid).ToArray();
                        if (subOrders != null)
                        {
                            // 應處理的子單筆數
                            var voucher_count = subOrders.Count();
                            // Call WMS: 取訂單明細
                            var _subOrders = _orderRepos.QueryOrders(subOrders);
                            foreach(var sub in _subOrders.order)
                            {
                                if (sub.orderStatus == "GO_OK")
                                {
                                    // 1. 查詢憑證List
                                    var voucher = _orderRepos.QueryVouchers(sub.orderMid);
                                    if (voucher.file.Count > 0)
                                    {
                                        // 2. 下載憑證至memory
                                        // 3. 上傳至 s3 (必須為PDF)
                                        byte[] fileByte = null;
                                        voucher.file.ForEach(x =>
                                        {
                                            var upload = _amazonS3Service.UploadObject(x.file_name, "application/pdf", fileByte).Result;
                                        });
                                        // 4. 更改子單voucher_status='VOUCHER_OK'
                                        var updVoucher = _orderRepos.UpdateDtlVoucherStatus(sub.orderMid, "VOUCHER_OK");
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
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"VoucherBackgroundService_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
            }
        }
        /// <summary>
        /// 檢查母單已超過時間但is_callback=false
        /// </summary>
        /// <param name="state"></param>
        private void DoWork2(object state)
        {
            // 取得尚未 CallBack 的母單(is_callback=false)
            var master = _orderRepos.GetTimeOutMaster();
            if (master.result == "0000" && master.count > 0)
            {
                master.order_mst_list.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.monitor_start_datetime))
                    {
                        var sDateTime = Convert.ToDateTime(x.monitor_start_datetime);
                        // 暫定沒填 Default 等待20min
                        var deadLine = x.voucher_deadline == 0 ? 20 : x.voucher_deadline;
                        if (DateTime.Now > sDateTime.AddMinutes(deadLine))
                        {
                            // 1. Update DB
                            var upd = _orderRepos.UpdateIsCallBack(x.booking_mst_xid, true);
                            if (upd.result == "0000")
                            {
                                // 2. 觸發 CallBackJava
                                RequestJson callBackJson = new RequestJson
                                {
                                    orderMid = x.order_mid
                                };
                                // 3. 通知b2d slack?
                            }
                        }
                    }
                });
            }
            else if (master.result == "9999")
            {
                // Exception 處理？
            }
        }

        public void Dispose()
        {
            _timer1?.Dispose();
            _timer2?.Dispose();
        }
    }
}

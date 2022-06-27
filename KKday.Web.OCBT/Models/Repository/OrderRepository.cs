using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model;
using KKday.Web.OCBT.Models.Model.DataModel;
using KKday.Web.OCBT.Models.Model.Order;
using KKday.Web.OCBT.Proxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace KKday.Web.OCBT.Models.Repository
{
    public class OrderRepository
    {
        private readonly ComboBookingRepository _comboBookingRepos;
        private readonly OrderProxy _orderProxy;
        public OrderRepository(ComboBookingRepository comboBookingRepos, OrderProxy orderProxy)
        {
            _comboBookingRepos = comboBookingRepos;
            _orderProxy = orderProxy;
        }

        /// <summary>
        /// Query Order Master Data
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public OrderRsModel FetchOrderMstData(string filter, string sort, string order, int offset, int limit)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();

                string sqlStmt = @"SELECT DISTINCT m.* FROM booking_mst m
LEFT JOIN booking_dtl d ON m.booking_mst_xid =d.booking_mst_xid WHERE 1=1 {FILTER}";

                var _filter = OrderMstFilterParsing(filter, sort, order);
                sqlStmt = sqlStmt.Replace("{FILTER}", !string.IsNullOrEmpty(_filter.sql) ? _filter.sql : string.Empty);
                // 相同條件下取出總筆數
                string sqlCount = $@"SELECT COUNT(c.*) FROM ({sqlStmt}) c";
                // 最後再加上分頁條件
                sqlStmt += " LIMIT :limit OFFSET :offset";
                _filter.args.Add("limit", limit);
                _filter.args.Add("offset", offset);
                //Start Query
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new ObjectJsonMapper());
                    rs.total_count = conn.QuerySingle<int>(sqlCount, _filter.args);
                    rs.order_mst_list = conn.Query<OrderMstModel>(sqlStmt, _filter.args).ToList();
                    rs.count = rs.order_mst_list.Count();
                    rs.result = "0000";
                }

                return rs;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Query Order Detail Data By booking_mst_xid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrderRsModel FetchOrderDtlData(string id, string voucherStatus=null)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel() { result = "0001", result_message = "Xid 不可為空且須為正整數" };
                // Xid 不可為空且須為正整數
                if (!string.IsNullOrEmpty(id) && id.All(char.IsDigit))
                {
                    string sql = @"SELECT * FROM booking_dtl WHERE booking_mst_xid=:booking_mst_xid";
                    if (!string.IsNullOrEmpty(voucherStatus)) sql += " AND booking_dtl_voucher_status=:voucherStatus";

                    using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                    {
                        SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new ObjectJsonMapper());
                        SqlMapper.AddTypeHandler(typeof(SkuOid), new ObjectJsonMapper());

                        rs.order_dtl_list = conn.Query<OrderDtlModel>(sql, new { booking_mst_xid = Convert.ToInt64(id), voucherStatus }).ToList();
                        rs.count = rs.order_dtl_list.Count();
                        rs.result = "0000";
                    }
                }

                return rs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public (string sql, DynamicParameters args) OrderMstFilterParsing(string strJson, string sort, string order)
        {
            try
            {
                var _dynamic = new DynamicParameters();
                var _sql = "";
                // Filter
                if (!string.IsNullOrEmpty(strJson))
                {
                    var _filter = JsonConvert.DeserializeObject<OrderSearch>(strJson);
                    if (_filter.main_prod_oid > 0)
                    {
                        _sql += " AND m.prod_oid = :main_prod_oid";
                        _dynamic.Add("main_prod_oid", _filter.main_prod_oid);
                    }
                    //if (!string.IsNullOrEmpty(_filter.main_pkg_oid))
                    //{
                    //    _sql += " AND m.pkg_oid = :main_pkg_oid";
                    //    _dynamic.Add("main_pkg_oid", _filter.main_pkg_oid);
                    //}
                    if (!string.IsNullOrEmpty(_filter.main_order_mid ))
                    {
                        _sql += " AND m.order_mid = :main_order_mid";
                        _dynamic.Add("main_order_mid", _filter.main_order_mid);
                    }
                    if (!string.IsNullOrEmpty(_filter.sub_order_mid ))
                    {
                        _sql += " AND d.order_mid = :sub_order_mid";
                        _dynamic.Add("sub_order_mid", _filter.sub_order_mid);
                    }
                    if (!string.IsNullOrEmpty(_filter.main_order_status))
                    {
                        _sql += " AND m.booking_mst_order_status = :main_order_status";
                        _dynamic.Add("main_order_status", _filter.main_order_status);
                    }
                    if (!string.IsNullOrEmpty(_filter.main_voucher_status))
                    {
                        _sql += " AND m.booking_mst_voucher_status = :main_voucher_status";
                        _dynamic.Add("main_voucher_status", _filter.main_voucher_status);
                    }

                    if (!string.IsNullOrEmpty(_filter.sub_order_status))
                    {
                        _sql += " AND d.booking_dtl_order_status = :sub_order_status";
                        _dynamic.Add("sub_order_status", _filter.sub_order_status);
                    }
                    if (!string.IsNullOrEmpty(_filter.sub_voucher_status))
                    {
                        _sql += " AND d.booking_dtl_voucher_status = :sub_voucher_status";
                        _dynamic.Add("sub_voucher_status", _filter.sub_voucher_status);
                    }
                }
                // Sort
                if (!string.IsNullOrEmpty(sort)) _sql += $" ORDER BY {sort} ";
                if (!string.IsNullOrEmpty(order)) _sql += $" {order} \n";


                return (_sql, _dynamic);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region Voucher BackgroundService

        /// <summary>
        /// Query BookingMst by Batch order_mid
        /// </summary>
        /// <param name="req">string array order_mid</param>
        /// <returns></returns>
        public OrderMstModel QueryBookingMst(string order_mid)
        {
            try
            {
                string sql = @"SELECT m.booking_mst_xid, m.order_mid, m.voucher_deadline, m.monitor_start_datetime
FROM booking_mst m WHERE m.booking_mst_order_status='GL' AND m.order_mid=:order_mid AND m.is_callback=false ";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.QuerySingleOrDefault<OrderMstModel>(sql, new { order_mid });
                }
            }
            catch(Exception ex)
            {
                Website.Instance.logger.Fatal($"OrderRepos QueryBookingMst Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        public OrderRsModel QueryBookingDtl(int booking_mst_xid)//, string voucherStatus)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();
                string sql = @"SELECT order_mid, booking_dtl_voucher_status
FROM booking_dtl WHERE booking_mst_xid=:booking_mst_xid";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    rs.order_dtl_list = conn.Query<OrderDtlModel>(sql, new { booking_mst_xid }).ToList();//, voucherStatus }).ToList();
                    rs.count = rs.order_dtl_list.Count();
                    rs.result = "0000";
                }
                return rs;
            }
            catch(Exception ex)
            {
                Website.Instance.logger.Fatal($"OrderRepos QueryBookingDtl Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        public RsModel UpdateDtlVoucherStatus(string order_mid, string booking_dtl_voucher_status, List<string> voucher_file_info = null)
        {
            try
            {
                RsModel rs = new RsModel() { result = "0001", result_message = "order_mid & booking_dtl_voucher_status 不可為空" };
                if(!string.IsNullOrEmpty(order_mid) && !string.IsNullOrEmpty(booking_dtl_voucher_status))
                {
                    var _dynamic = new DynamicParameters();

                    string sql = @"UPDATE public.booking_dtl 
SET booking_dtl_voucher_status=:booking_dtl_voucher_status, modify_datetime=NOW()
";

                    if (voucher_file_info?.Count > 0)
                    {
                        sql += " , voucher_file_info=:voucher_file_info::jsonb \n";
                        _dynamic.Add("voucher_file_info", voucher_file_info);
                    }

                    sql += " WHERE order_mid=:order_mid ";

                    _dynamic.Add("order_mid", order_mid);
                    _dynamic.Add("booking_dtl_voucher_status", booking_dtl_voucher_status);

                    SqlMapper.AddTypeHandler(typeof(List<string>), new ObjectJsonMapper());
                    using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                    {
                        if (conn.Execute(sql, _dynamic) > 0)
                        {
                            rs.result = "0000";
                            rs.result_message = "OK";
                        }
                        else
                        {
                            rs.result_message = "Update Fail";
                        }
                    }
                }

                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"OrderRepos UpdateDtlVoucherStatus Exception: Message={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        public RsModel UpdateMstVoucherStatus(int mst_xid, string status, string is_callback = null)//, bool callBack=null)
        {
            try
            {
                RsModel rs = new RsModel() { result = "0001", result_message = "mst_xid & booking_dtl_voucher_status 不可為空" };
                if (mst_xid > 0 && !string.IsNullOrEmpty(status))
                {
                    string sql = @"UPDATE public.booking_mst 
SET booking_mst_voucher_status=:status, modify_datetime=NOW()";

                    if (!string.IsNullOrEmpty(is_callback)) sql += " , is_callback=true ";

                    sql += "WHERE booking_mst_xid=:mst_xid ";

                    using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                    {
                        if (conn.Execute(sql, new { mst_xid, status }) > 0)
                        {
                            rs.result = "0000";
                            rs.result_message = "OK";
                        }
                        else
                        {
                            rs.result_message = "Update Fail";
                        }
                    }
                }

                return rs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public (bool is_true, OrderRsModel rs) CheckDtl(int booking_mst_xid, string order_mid)
        {
            var is_true = false;
            OrderRsModel rs = new OrderRsModel();
            rs.order_dtl_list = new List<OrderDtlModel>();
            try
            {
                var dtl = QueryBookingDtl(booking_mst_xid);
                if (dtl.order_dtl_list.Count > 0)
                {
                    rs.order_dtl_list = dtl.order_dtl_list;
                    // PROCESS + VOUCHER_OK = Total 子單
                    var totalCount = rs.order_dtl_list.Count;
                    // Select PROCESS Order
                    var processOrder = rs.order_dtl_list.Where(s => s.booking_dtl_voucher_status == "PROCESS")?.Select(x => x.order_mid).ToArray();
                    // 應處理的子單筆數
                    var processCount = processOrder.Count();
                    // Select VOUCHER_OK Order
                    var vouhOkCount = rs.order_dtl_list.Where(s => s.booking_dtl_voucher_status == "VOUCHER_OK").Count();
                    if (processCount == 0&& totalCount == (processCount + vouhOkCount))
                    {
                        is_true = true;
                        // CallBackJava
                        RequestJson callBackJson = new RequestJson
                        {
                            orderMid = order_mid,
                            metadata = new RequesteMetaModel
                            {
                                status = "2000",
                                description = "OCBT取得憑證OK"
                            }
                        };
                        _comboBookingRepos.CallBackJava(callBackJson);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (is_true, rs);
        }

        #endregion Voucher BackgroundService

        #region Call WMS
        public OrderListModel QueryOrders(string[] orders)
        {
            try
            {
                QueryOrderModel rq = new QueryOrderModel
                {
                    locale_lang = "zh -tw",
                    account_type = "01",
                    option = new Option
                    {
                        time_zone = "Asia/Taipei",
                        kkday_orders = orders
                    }
                };
                string url = string.Format("{0}{1}", Website.Instance.Configuration["WMS_API:URL"], "v2/order/QueryOrders");
                return JsonConvert.DeserializeObject<OrderListModel>(CommonProxy.Post(url, JsonConvert.SerializeObject(rq)));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public VoucherRsModel QueryVouchers(string order)
        {
            try
            {
                Dictionary<string, object> rq = new Dictionary<string, object>
                {
                    { "is_KKday" , true },
                    { "locale_lang" , "zh-tw" },
                    { "order_no" , order }
                };
                string url = string.Format("{0}{1}", Website.Instance.Configuration["WMS_API:URL"], "Voucher/QueryVoucherList");
                return JsonConvert.DeserializeObject<VoucherRsModel>(CommonProxy.Post(url, JsonConvert.SerializeObject(rq)));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public VoucherRsModel DownloadVoucher(string order_no, string order_file_id)
        {
            try
            {
                Dictionary<string, object> rq = new Dictionary<string, object>
                {
                    { "is_KKday" , true },
                    { "locale_lang" , "zh-tw" },
                    { "order_no" , order_no },
                    { "order_file_id" , order_file_id }
                };
                string url = string.Format("{0}{1}", Website.Instance.Configuration["WMS_API:URL"], "Voucher/Download");
                return JsonConvert.DeserializeObject<VoucherRsModel>(CommonProxy.Post(url, JsonConvert.SerializeObject(rq)));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion Call WMS

        #region 排程檢查母單是否超過時間且is_callback=false
        public OrderRsModel GetTimeOutMaster()
        {
            OrderRsModel rs = new OrderRsModel
            {
                result = "9999",
                count = 0
            };

            try
            {
                string sqlStmt = @"SELECT * FROM booking_mst WHERE is_callback=false ";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new ObjectJsonMapper());
                    rs.order_mst_list = conn.Query<OrderMstModel>(sqlStmt).ToList();
                    rs.count = rs.order_mst_list.Count();
                    rs.result = "0000";
                }
            }
            catch(Exception ex)
            {
                rs.result_message = $"GetTimeOutMaster_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}";
            }

            return rs;
        }
        /// <summary>
        /// Update booking_mst.is_callback
        /// </summary>
        /// <param name="mst_xid"></param>
        /// <param name="is_callback"></param>
        /// <returns></returns>
        public RsModel UpdateIsCallBack(int mst_xid, bool is_callback = false)
        {
            RsModel rs = new RsModel { result = "0001", result_message = "Update Fail" };

            try
            {
                string sql = @"UPDATE public.booking_mst 
SET is_callback=:is_callback, modify_datetime=NOW()
WHERE booking_mst_xid=:mst_xid ";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    if (conn.Execute(sql, new { mst_xid, is_callback }) > 0)
                    {
                        rs.result = "0000";
                        rs.result_message = "OK";
                    }
                }
            }
            catch (Exception ex)
            {
                rs.result = "9999";
                rs.result_message = $"UpdateIsCallBack_Exception: Message={ex.Message}, StackTrace={ex.StackTrace}";
            }

            return rs;
        }
        #endregion 排程檢查母單是否超過時間且is_callback=false

        public void NotifyParentMemo(OrderMemoRequstModel request)
        {
            try
            {
                string orderMemo = "";
                if (request?.@event != "BE2_CANCEL" || request?.@event != "PART_REFUND")
                {
                    //滿足呼叫的內容
                    if (request?.@event == "BE2_CANCEL")
                    {
                        //order_memo 範例  （如下）   
                        //子訂單 21KK219206036
                        //OP已啟動特殊原因取消，請進行退款作業
                        //原因：Fraudulent - Voucher Sent

                        orderMemo = $"{orderMemo}子訂單{request?.orderMid}\r\n";
                        orderMemo = $"{orderMemo}OP已啟動特殊原因取消，請進行退款作業\r\n";
                        orderMemo = $"{ orderMemo}原因：『‘{request?.modifyReasonCodeTransTW}’ 』{request?.modifyReasonDesc}\r\n";
                        //sub_order  21KK219206036
                        //OP has started the special  reason cancellation process , please do the refund operation
                        //Reason：Fraudulent - Voucher Sent
                        orderMemo = $"{orderMemo}sub_order{request?.orderMid}\r\n";
                        orderMemo = $"{orderMemo}OP has started the special  reason cancellation process , please do the refund operation\r\n";
                        orderMemo = $"{ orderMemo}Reason：『‘{request?.modifyReasonCodeTransEn}’ 』{request?.modifyReasonDesc}\r\n";
                    }
                    else if (request?.@event == "PART_REFUND")
                    {
                        //子訂單 『 order_mid 』
                        //OP已啟動部分退款，請進行退款作業
                        //原因：『‘’modifyReasonCode’’ 』(『  "modifyReasonDesc"』)  
                        orderMemo = $"{orderMemo}子訂單{request?.orderMid}\r\n";
                        orderMemo = $"{orderMemo}OP已啟動部分退款，請進行退款作業\r\n";
                        orderMemo = $"{ orderMemo}原因：『‘{request?.modifyReasonCodeTransTW}’ 』{request?.modifyReasonDesc}\r\n";
                        //sub_order  『 order_mid 』
                        //OP has started a partial refund, please do the refund operation
                        //Reason：『‘’modifyReasonCode’’ 』(『  "modifyReasonDesc"』)
                        orderMemo = $"{orderMemo}sub_order{request?.orderMid}\r\n";
                        orderMemo = $"{orderMemo}OP has started a partial refund, please do the refund operation\r\n";
                        orderMemo = $"{ orderMemo}Reason：『‘{request?.modifyReasonCodeTransEn}’ 』{request?.modifyReasonDesc}\r\n";
                    }

                    //呼叫java
                    //呼叫java母子狀態
                    OrderApiRqModel orderApi = new OrderApiRqModel()
                    {
                        apiKey = Website.Instance.Configuration["KKdayAPI:Body:ApiKey"],
                        userOid = Website.Instance.Configuration["KKdayAPI:Body:UserOid"],
                        locale = "zh-tw",
                        ipaddress = _comboBookingRepos.GetLocalIPAddress()
                    };

                    Dictionary<string, object> json = new Dictionary<string, object>();
                    json.Add("memoType", "01");
                    json.Add("memoMsg", orderMemo);
                    orderApi.json = json;

                    string url = $"{Website.Instance.Configuration["ApiUrl:JAVA"]}/v2/order/memo//" + request?.parentOrderMid;
                    string result = _orderProxy.Post(url, JsonConvert.SerializeObject(orderApi,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                }), request?.requestUuid);
                }
                else
                {
                    throw new Exception("非指定的event:"+ request?.@event);
                }

                //依據event 組出回覆的內容
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"OrderRepository_NotifyParentMemo_exception:GuidKey={request?.requestUuid}, Message={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
    }
}
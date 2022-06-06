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
        /// <summary>
        /// Query BookingMst by Batch order_mid
        /// </summary>
        /// <param name="req">string array order_mid</param>
        /// <returns></returns>
        public OrderRsModel QueryBookingMst(string [] orders)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();
                string sql = @"SELECT m.* FROM booking_mst m WHERE m.booking_mst_order_status='GL' 
AND m.order_mid=ANY(:orders)";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    rs.order_mst_list = conn.Query<OrderMstModel>(sql, new { orders }).ToList();
                }

                return rs;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public RsModel UpdateDtlVoucherStatus(string orderMid, string status)
        {
            try
            {
                RsModel rs = new RsModel() { result = "0001", result_message = "order_mid & booking_dtl_voucher_status 不可為空" };
                if(!string.IsNullOrEmpty(orderMid) && !string.IsNullOrEmpty(status))
                {
                    string sql = @"UPDATE public.booking_dtl 
SET booking_dtl_voucher_status=:status, modify_datetime=NOW()
WHERE order_mid=:orderMid ";

                    using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                    {
                        if (conn.Execute(sql, new { orderMid, status }) > 0)
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
        public RsModel UpdateMstVoucherStatus(int mst_xid, string status)
        {
            try
            {
                RsModel rs = new RsModel() { result = "0001", result_message = "mst_xid & booking_dtl_voucher_status 不可為空" };
                if (mst_xid > 0 && !string.IsNullOrEmpty(status))
                {
                    string sql = @"UPDATE public.booking_mst 
SET booking_mst_voucher_status=:status, modify_datetime=NOW()
WHERE booking_mst_xid=:mst_xid ";

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
                VoucherRqModel rq = new VoucherRqModel
                {
                    is_KKday = true,
                    locale_lang = "zh-tw",
                    order_no = order
                };
                string url = string.Format("{0}{1}", Website.Instance.Configuration["WMS_API:URL"], "Voucher/QueryVoucherList");
                return JsonConvert.DeserializeObject<VoucherRsModel>(CommonProxy.Post(url, JsonConvert.SerializeObject(rq)));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion Call WMS
    }
}
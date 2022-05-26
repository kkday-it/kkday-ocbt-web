using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.Order;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace KKday.Web.OCBT.Models.Repository
{
    public class OrderRepository
    {
        private string FetchOrderMstSQL = @"SELECT DISTINCT m.* FROM booking_mst m
LEFT JOIN booking_dtl d ON m.booking_mst_xid =d.booking_mst_xid WHERE 1=1 {FILTER}";

        public OrderRsModel FetchOrderMstData(string filter, string sort, string order, int offset, int limit)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();

                SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new ObjectJsonMapper());
                string sqlStmt = FetchOrderMstSQL;
                var _filter = OrderMstFilterParsing(filter, offset, limit);
                sqlStmt = sqlStmt.Replace("{FILTER}", !string.IsNullOrEmpty(_filter.sql) ? _filter.sql : string.Empty);
                // 相同條件下取出總比數
                string sqlCount = $@"SELECT COUNT(c.*) FROM ({sqlStmt}) c";
                // 最後再加上分頁條件
                sqlStmt += " LIMIT :limit OFFSET :offset";
                _filter.args.Add("limit", limit);
                _filter.args.Add("offset", offset);
                // Start Query
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    rs.total_count = conn.QuerySingle<int>(sqlCount);
                    rs.order_mst_list = conn.Query<OrderMstModel>(sqlStmt, _filter.args).ToList();
                    rs.result = "0000";
                }

                return rs;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public OrderRsModel FetchOrderDtlData(string id)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();

                SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new ObjectJsonMapper());

                string sql = @"SELECT * FROM booking_dtl WHERE booking_mst_xid=:booking_mst_xid";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    var booking_mst_xid = !string.IsNullOrEmpty(id) ? Convert.ToInt16(id) : 0;
                    rs.order_dtl_list = conn.Query<OrderDtlModel>(sql, new { booking_mst_xid }).ToList();
                }
                return rs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public (string sql, DynamicParameters args) OrderMstFilterParsing(string strJson, int offset, int limit)
        {
            try
            {
                var _dynamic = new DynamicParameters();
                var _sql = "";

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
                    if (_filter.main_order_oid > 0)
                    {
                        _sql += " AND m.order_oid = :main_order_oid";
                        _dynamic.Add("main_order_oid", _filter.main_order_oid);
                    }
                    if (!string.IsNullOrEmpty(_filter.main_status))
                    {
                        _sql += " AND m.booking_mst_order_status = :main_status";
                        _dynamic.Add("main_status", _filter.main_status);
                    }
                }

                return (_sql, _dynamic);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using KKday.Web.OCBT.Models.Model.Order;
using Newtonsoft.Json;

namespace KKday.Web.OCBT.Models.Repository
{
    public class OrderRepository
    {
        public OrderRsModel FetchOrderMstData(string filter, string sort, string order, int offset, int limit)
        {
            try
            {
                OrderRsModel rs = new OrderRsModel();
                rs.order_mst_list = new List<OrderMstModel>();
                // Fake Data
                rs.order_mst_list.Add(new OrderMstModel
                {
                    booking_mst_xid = 1,
                    order_mid = "KK202205230001",
                    order_oid = 220523001,
                    main_master_oid = 0,
                    booking_mst_order_status = "NW",
                    voucher_deadline = 2,
                    prod_oid = 10000,
                    prod_name = "Test1",
                    package_oid = 10001,
                    package_name = "Package 10001",
                    create_user = "P.Wang"
                });
                rs.order_mst_list.Add(new OrderMstModel
                {
                    booking_mst_xid = 2,
                    order_mid = "KK202205230002",
                    order_oid = 220523002,
                    main_master_oid = 0,
                    booking_mst_order_status = "NW",
                    voucher_deadline = 2,
                    prod_oid = 20000,
                    prod_name = "Test2",
                    package_oid = 20001,
                    package_name = "Package 20001",
                    create_user = "P.Wang"
                });

                var a = JsonConvert.SerializeObject(rs);

                return JsonConvert.DeserializeObject<OrderRsModel>(a);
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
                rs.order_dtl_list = new List<OrderDtlModel>();
                // Fake Data
                rs.order_dtl_list.Add(new OrderDtlModel
                {
                    order_mid = "11",
                    order_oid = 11,
                    order_master_oid = 1111,
                    booking_dtl_order_status = "NW",
                    prod_oid = 1111111,
                    package_oid = 223344
                });
                var a = JsonConvert.SerializeObject(rs);
                return JsonConvert.DeserializeObject<OrderRsModel>(a);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

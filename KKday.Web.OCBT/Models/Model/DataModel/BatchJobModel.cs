using System;
using System.Collections.Generic;
namespace KKday.Web.OCBT.Models.Model.DataModel
{

    public class ParentOrderModel :BookingMstModel
    {
        public string is_open_date { get; set; } 
        public List<ChildOrderModel> child_order { get; set; }
    }


    public class ChildOrderModel 
    {
        public int? booking_dtl_xid { get; set; }        // 明細檔xid
        public string booking_dtl_order_status { get; set; }    // 訂單狀態
        public string booking_dtl_voucher_status { get; set; }  // 憑證狀態
        public int? order_oid { get; set; }              // 子單order_oid
        public string order_mid { get; set; }           // 子單order_mid

    }
}

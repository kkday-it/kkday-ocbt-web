using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class BookingMstModel
    {
        public int booking_mst_xid { get; set; } // 主檔
        public string order_mid { get; set; }
        public int order_oid { get; set; }
        public int prod_oid { get; set; }
        public Dictionary<string, string> booking_model { get; set; }
        public Dictionary<string, string> product_model { get; set; }
        public string booking_mst_status { get; set; }
        public int voucher_deadline { get; set; }
        public string create_user { get; set; }
        public DateTime create_datetime { get; set; }
        public string modify_user { get; set; }
        public DateTime modify_datetime { get; set; }
    }
}

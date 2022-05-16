using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class BookingMstModel
    {
        public int booking_mst_xid { get; set; }                      // 主檔xid
        public string order_mid { get; set; }                         // 母單order_mid
        public int order_oid { get; set; }                            // 母單order_oid
        public int prod_oid { get; set; }                             // 母單prod_oid （其餘在product_model)
        public Dictionary<string, string> booking_model { get; set; } // java call ocbt的原始json
        public Dictionary<string, string> product_model { get; set; } // 從product取得的原始json
        public string booking_mst_status { get; set; }                // 狀態
        public int voucher_deadline { get; set; }                     // 憑證最長等待的時間 （從 product_model 取出）
        public string create_user { get; set; }
        public DateTime create_datetime { get; set; }
        public string modify_user { get; set; }
        public DateTime modify_datetime { get; set; }
    }
}

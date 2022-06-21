using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class BookingDtlModel
    {
        public int booking_dtl_xid { get; set; }        // 明細檔xid
        public int booking_mst_xid { get; set; }        // 主檔xid
        public int prod_oid { get; set; }               // 子單prod_oid
        public int package_oid { get; set; }            // 子單package_oid
        public int item_oid { get; set; }               // 子單item_oid
        public SkuOid sku_oid { get; set; }             // 子單sku_oid
        public int booking_qty { get; set; }            // 規則上欲訂購的數量
        public int real_booking_qty { get; set; }       // 實際訂購的數量
        public string booking_dtl_order_status { get; set; }    // 訂單狀態
        public string booking_dtl_voucher_status { get; set; }  // 憑證狀態
        public int order_master_oid { get; set; }       // 購物車order_master_oid
        public string order_master_mid { get; set; }    // 購物車order_master_mid
        public int order_oid { get; set; }              // 子單order_oid
        public string order_mid { get; set; }           // 子單order_mid
        public Dictionary<string, string> voucher_file_info { get; set; } // voucher 存放資訊
        public string create_user { get; set; }
        public DateTime create_datetime { get; set; }
        public string modify_user { get; set; }
        public DateTime? modify_datetime { get; set; }
    }
    public class SkuOid
    {
        public string[] sku_oids { get; set; }
    }
    
}

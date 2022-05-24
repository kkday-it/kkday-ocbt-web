using System;
namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class BookingLogModel
    {
        public int booking_log_xid { get; set; }        // log檔xid
        public int booking_mst_xid { get; set; }        // 主檔xid
        public int booking_dtl_xid { get; set; }        // 明細檔xid
        public string booking_source { get; set; }      // 定義是主檔還是明細檔的log  (MST/DTL)
        public string status_type { get; set; }         // ORDER / VOUCHER
        public string status { get; set; }              // 狀態
        public string previous_status { get; set; }     // 記錄之前的status
        public string create_user { get; set; }
        public DateTime create_datetime { get; set; }
        public string modify_user { get; set; }
        public DateTime? modify_datetime { get; set; }
    }
}

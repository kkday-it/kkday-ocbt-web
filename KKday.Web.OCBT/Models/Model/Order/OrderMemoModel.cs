using System;
using KKday.Web.OCBT.Models.Model.DataModel;

namespace KKday.Web.OCBT.Models.Model.Order
{
    public class OrderMemoRequstModel
    {
        public string requestUuid { get; set; }
        public string parentOrderMid { get; set; }
        public string orderMid { get; set; }
        public string @event { get;set; } //BE2_CANCEL & PART_REFUND
        public string modifyReasonDesc { get; set; }  //"取消三位＆其他費用支出"
        public string modifyReasonCodeTransEn { get; set; }  //supplierCostModify.reason.modifyResonCode -> 找對應English翻譯
        public string modifyReasonCodeTransTW { get; set; }  //訂單變動：數量變更（客人） //(nullable) supplierCostModify.reason.modifyResonCode -> 找對應zh-tw翻譯
    }

    public class OrderMemoResponseModel
    {
        public ResponseMetaModel metadata { get; set; }
    }
}

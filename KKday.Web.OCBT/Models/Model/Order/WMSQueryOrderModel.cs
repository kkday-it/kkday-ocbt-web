using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Order
{
    #region RQ Model From WMS
    public class QueryOrderModel
    {
        public Int64 company_xid { set; get; }
        public Int64 channel_oid { set; get; }
        public string locale_lang { get; set; }
        public string current_currency { get; set; }
        public string state { get; set; }
        public bool isKKday { get; set; }
        public Option option { get; set; }
        public string account_type { get; set; }
        public int account_xid { get; set; }
        public string ip_address { get; set; }
    }
    public class Option
    {
        //狀態篩選條件
        //GO處理中 GO_OK已處理  CX_ING取消中 CX已取消
        public string order_status { get; set; }
        public int page_size { get; set; }
        public int current_page { get; set; }
        public string time_zone { get; set; }
        public string prod_Sdate { get; set; }
        public string prod_Edate { get; set; }
        public string order_Sdate { get; set; }
        public string order_Edate { get; set; }
        public string[] kkday_orders { get; set; }
        public List<string> orders { get; set; }
        public string partner_orders { get; set; }
        public int prod_no { get; set; }
        public string partner_order_no { get; set; }
        public string company_country { get; set; }
    }
    #endregion RQ Model From WMS

    #region RS Model From WMS
    public class OrderListModel
    {
        public string result { get; set; }
        public string result_msg { get; set; }
        public int order_qty { get; set; }
        public int current_page { get; set; }
        public string companyName { get; set; }
        public List<Order> order { get; set; }
    }
    public class Order
    {
        public string orderNo { get; set; }
        public string orderOid { get; set; }
        public string orderMid { get; set; }
        public string crtDt { get; set; }
        public string userCrtDt { get; set; }
        public string userCrtDtGMTNm { get; set; }
        public long company_xid { get; set; }
        public string comp_name { get; set; }
        public string opStatus { get; set; }//取得訂單後的狀態 TAKE_STATUS  CANCEL_STATUS
        public string takeStatus { get; set; }//取得訂單後的步驟 1 2 3
        public string orderStatus { get; set; }//訂單本身狀態 GO CX
        public string orderStatusTxt { get; set; }
        public string lstStatus { get; set; }
        public int channelOid { get; set; }
        public string cashflowStatus { get; set; }
        public string cancelStatus { get; set; }
        public string cancelTxt { get; set; }
        public string cancelTxtSup { get; set; }
        public string cancelTxtMem { get; set; }
        public string cancelCode { get; set; }
        public string productOid { get; set; }
        public string productName { get; set; }
        public string packageOid { get; set; }
        public string packageName { get; set; }
        public int qtyTotal { get; set; }
        public string begLstGoDt { get; set; }
        public string begLstGoDtGMT { get; set; }
        public string begLstGoDtGMTNm { get; set; }
        public string endLstBackDt { get; set; }
        public double priceRefund { get; set; }
        public double priceTotal { get; set; }
        public double currRate { get; set; }
        public double currPriceRefund { get; set; }
        public double currFeeCancel { get; set; }
        public string pmgwCurrCd { get; set; }
        public double? pmgwCurrPriceTotal { get; set; }//增加問號避免null值
        public bool isNew { get; set; }
        public List<skuInfos> skuInfoList { get; set; }//新增sku的資訊
    }
    public class skuInfos
    {
        public string skuOid { get; set; }
        public int? qty { get; set; }
        public double? price { get; set; }
    }
    #endregion RS Model From WMS
}

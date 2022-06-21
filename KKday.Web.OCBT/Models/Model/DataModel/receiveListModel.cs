using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class ReceiveListModel
    {
       public List<AcctDocReceiveModel> data { get; set; }
       public Product.metaDataProdModel metadata { get; set; }
    }
    public class AcctDocReceiveModel
    {
        public int? receive_master_oid { get; set; }
        public string order_master_oid { get; set; }
        public string receive_method { get; set; }
        public double? receive_total { get; set; }
        public string receive_status { get; set; }
        public string currency { get; set; }
        public double? currency_rate { get; set; }
        public double? currency_receive_total { get; set; }
        public int? channel_oid { get; set; }
        public int? company_no { get; set; }
        public double? fa_vouch_price { get; set; }//收款入帳金額
        public string fa_vouch_currency { get; set; }//收款入帳幣別
        public string fa_vouch_currency_rate { get; set; }//收款入帳匯率

    }
}

using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.CartBooking
{

    public class ConfirmBookingValidModel
    {
        public List<ConfirmProdInfoModel> cartorder { get; set; }
        public string token { get; set; }
        public double master_price_total { get; set; }
        public bool adjprice_flag { get; set; }//是否能作變價處理
        public string adjprice_type { get; set; }//變價type 01:接受>0任意金額 02:接受最低至cost成本的金額 03:接受最低至net price的金額
    }
    public class ConfirmProdInfoModel
    {
        public string ori_guidkey { get; set; }
        public string price_token { get; set; }
        public string prod_oid { get; set; }
        public string currency { get; set; }
        public string prod_name { get; set; }
        public string prod_image_url { get; set; }
        public string begin_date { get; set; }//yyyy-MM-dd
        public string end_date { get; set; }//yyyy-MM-dd
        public string begin_date_gmt { get; set; }//yyyy-MM-dd(gmt+8)
        public string end_date_gmt { get; set; }//yyyy-MM-dd(gmt+8)
        public bool? has_event { get; set; }
        public string event_time { get; set; }
        public List<ConfirmSku> skus { get; set; }
        public double? total_price { get; set; }
        public double? ota_price { get; set; }
        public double? sale_rate { get; set; }//usd rate

        public string pmgw_currency { get; set; }
        public double? pmgw_totalPrice { get; set; }

        public string prod_version { get; set; }//產品編號
        public string packege_oid { get; set; }//套餐編號
        public string package_name { get; set; }//套餐名稱
        public string item_oid { get; set; }//商品編號
        public string time_zone { get; set; }//時區
        public string prod_type { get; set; }//產品類型
        public string country_code { get; set; }//國家代碼
        public bool? is_backup { get; set; }//是否需要備用場次
        public string locale { get; set; }//語系
        public string item_unit { get; set; }

        public int unit_max { get; set; }//unit最大值
        public bool adjprice_flag { get; set; }//是否能作變價處理
        public string adjprice_type { get; set; }//變價type 01:接受>0任意金額 02:接受最低至cost成本的金額 03:接受最低至net price的金額

        public string comp_xid { get; set; }//因應sale_channel

        //---------取得paymentList所需參數--------------//
        public string is_tourism_product { get; set; }//是否為旅遊商品
        public string sale_state { get; set; }//銷售市場
        public string forbidden_marketing { get; set; }//禁賣市場
        public string paymentInvoiceType { get; set; }
        public List<string> kkday_product_country_code_list { get; set; }//payment 商品所在地
        public string go_date_type { get; set; }//01為單一日 02為多日 03為無日期 04為無日期

    }
    public class ConfirmSku
    {
        public string sku_id { get; set; }
        public Dictionary<string, string> spec { get; set; }//語系spec
        public int? qty { get; set; }
        public double? price { get; set; }//精準度以currency的小數點為主
        public double usd_price { get; set; }
        public double gross_rate { get; set; }//利潤以美金
        public int? age_max { get; set; }//新增sku最大年齡
        public int? age_min { get; set; }//新增sku最小年齡
    }

    public class BookingValidModel
    {
        public string result { get; set; }
        public string result_msg { get; set; }
    }
}

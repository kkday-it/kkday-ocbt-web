using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Product
{
    public class ComboInfoModel
    {
        public string prod_oid { get; set; }
        public string pkg_oid { get; set; }
        public string item_oid { get; set; }
        public int? voucher_max_time { get; set; }
        public List<ComboInfoSkusModel> skus { get; set; }
        public List<ComboProdModel> combo_prod { get; set; }
        
    }
    public class ComboInfoSkusModel:ComboSkusModel
    {
        public List<ComboProdModel> combo_prod { get; set; }
    }
    public class ComboProdModel
    {
        public string prod_oid { get; set; }
        public string pkg_oid { get; set; }
        public string item_oid { get; set; }
        public string supplier_oid { get; set; }
        public List<ComboSkusModel> skus { get; set; }

    }
    public class ComboSkusModel
    {
        public string sku_oid { get; set; }
        public int? qty { get; set; }
    }
    public class ComboDataModel
    {
        public string status { get; set; }
        public ComboInfoModel combo_info { get; set; }
        public string error_code { get; set; }
        public string error_desc { get; set; }
    }
    public class ComboReturnModel
    {
        public List<ComboDataModel> data { get; set; }
        public metaDataProdModel meta { get; set; }
        public int voucher_max_time { get; set; }
    }
    public class metaDataProdModel
    {
        public string status { get; set; }
        public string desc { get; set; }
    }
    public class ComboRequestModel
    {
        public List<ComboCartItemsModel> cart_items{get;set;}
    }
    public class ComboCartItemsModel
    {
        public string prod_oid { get; set; }
        public string pkg_oid { get; set; }
        public string go_date { get; set; }
        public string back_date { get; set; }
        public string event_time { get; set; }
        public List<ComboSkusModel> skus { get; set; }
    }
    
}

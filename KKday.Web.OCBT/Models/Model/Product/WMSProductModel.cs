using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Product
{
    public class WMSProductModel
    {
        public string version { get; set; }
        public List<int> pkgs { get; set; }

    }
    public class WMSPackageModel
    {
        public string result { get; set; }
        public string result_msg { get; set; }
        public string guid { get; set; }
        public int pkg_no { get; set; }
        public string pkg_name { get; set; }

    }
    public class WMSItemModel
    {
        public int item_no { get; set; }
        public string unit { get; set; }
        public string unit_desc { get; set; }
        public List<Spec> specs { get; set; }
        public List<Sku> skus { get; set; }

    }
    public class SpecItem
    {
        public string spec_item_oid { get; set; }//規格項目ID(spec_item_oid)
        public string name { get; set; }//規格項目名稱
    }
    public class Spec
    {//品項規格
        public string spec_oid { get; set; }//spec_oid 
        public string spec_title { get; set; }//規格名稱
        public bool is_custom { get; set; }//是否是自訂規格 0:公用規格, 1:自訂規格
        public List<SpecItem> spec_items { get; set; }//規格項目列表
    }
    public class Sku
    {
        public string sku_oid { get; set; }
        public Dictionary<string, string> spec { get; set; }
        public Dictionary<string, PriceModel> calendar_detail { get; set; }
    }
    public class PriceModel
    {
        public Dictionary<string, double?> price { get; set; }
        public Dictionary<string, double?> cost { get; set; }
        public Dictionary<string, int?> remain_qty { get; set; }
        public Dictionary<string, double> b2d_price { get; set; }
    }
}

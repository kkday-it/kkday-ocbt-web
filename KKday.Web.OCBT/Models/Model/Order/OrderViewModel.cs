using System;
using System.Collections.Generic;
using KKday.Web.OCBT.Models.Model.DataModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KKday.Web.OCBT.Models.Model.Order
{
    public class OrderViewModel
    {
    }
    public class OrderSearch
    {
        public string main_prod_oid { get; set; }
        public string main_pkg_oid { get; set; }
        public string main_order_oid { get; set; }
        public string sub_prod_oid { get; set; }
        public string sub_pkg_oid { get; set; }
        public string sub_order_oid { get; set; }
        public string main_status { get; set; }
        public List<SelectListItem> SelectedItems { get; set; }
    }

    public class OrderRsModel : RsModel
    {
        public List<OrderMstModel> order_mst_list { get; set; }
        public List<OrderDtlModel> order_dtl_list { get; set; }
    }
    public class OrderMstModel : BookingMstModel
    {
        public int main_master_oid { get; set; }
        public string prod_name { get; set; }
        public int package_oid { get; set; }
        public string package_name { get; set; }
    }
    public class OrderDtlModel : BookingDtlModel
    {
        public int sub_master_oid { get; set; }
    }
}
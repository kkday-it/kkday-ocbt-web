using System;
using System.Collections.Generic;
namespace KKday.Web.OCBT.Models.Model.Order
{
    public class OrderApiRqModel
    {
        public string apiKey { get; set; }
        public string locale { get; set; }
        public string userOid { get; set; }
        public string ver { get; set; }
        public string ipaddress { get; set; }
        public Dictionary<string,object> json { get; set; }
    }


    public class relastionMappingResModel
    {
        public relastionMappingContentResModel content { get; set; }
    }


    public class relastionMappingContentResModel
    {
        public string result { get; set; }
        public string msg { get; set; }
        public string parentOrderMid { get; set; }
        public string parentOrderStatus { get; set; }
        public List<relastionMappingOrderListResModel> orderList { get; set; }
    }

    public class relastionMappingOrderListResModel
    {
        public string orderMid { get; set; }
        public string orderStatus { get; set; }
        public Boolean parent { get; set; }
    }
}

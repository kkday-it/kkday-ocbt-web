using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Product
{
    public class QueryWMSProductModel
    {
        public string locale { get; set; }
        public string prod_no { get; set; }
        public string state { get; set; }
        public string currency { get; set; }
        public List<string> block { get; set; }
        public string pkg_no { get; set; }
        public string begin_date { get; set; }
        public string end_date { get; set; }
        public string prod_version { get; set; }

        public string account_xid { get; set; }//取固定的帳號
        public string company_xid { get; set; }//取固定的分銷商
    }
    
}

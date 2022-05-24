using System;
using System.Collections.Generic;
namespace KKday.Web.OCBT.Models.Model.DataModel
{

    public class ComboSupRequestModel
    {
        public string request_uuid { get; set; }
        public string source_id { get; set; }
    }


    public class ComboSupResponseModel : ResponseJavaModel
    {
        public ComboSupResponseDataModel data { get; set; }   
    }


    public class ComboSupResponseDataModel
    {
        public List<int> supplier_oid { get; set; }
    }
     
}

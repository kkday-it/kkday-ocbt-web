using System;
using System.Collections.Generic;
namespace KKday.Web.OCBT.Models.Model.DataModel
{

    public class ComboSupRequestModel
    {
        public string request_uuid { get; set; }
        public string source_id { get; set; }
    }


    public class ComboSupResponseModel 
    {
        public ComboSupResponseDataModel data { get; set; }
        public ResponseMetaModel metadata { get; set; }
    }


    public class ComboSupResponseDataModel
    {
        public List<int> supplier_oid { get; set; }
    }

    public class ChkCancelRequestModel : ComboSupRequestModel
    {
        public string order_mid { get; set; }
    }

    public class ChkCancelResponseModel : ResponseJson
    {
    }

}

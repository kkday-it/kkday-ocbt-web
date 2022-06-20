using System;
using System.Collections.Generic;
namespace KKday.Web.OCBT.Models.Model.DataModel
{

    public class ComboSupRequestModel
    {
        public string requestUuid { get; set; }
        public string sourceId { get; set; }
    }


    public class ComboSupResponseModel 
    {
        public ComboSupResponseDataModel data { get; set; }
        public ResponseMetaModel metadata { get; set; }
    }


    public class ComboSupResponseDataModel
    {
        public List<int> supplierOid { get; set; }
    }

    public class ChkCancelRequestModel : ComboSupRequestModel
    {
        public string orderMid { get; set; }
    }

    public class ChkCancelResponseModel : ResponseJson
    {
    }

}

using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class ResponseJavaModel
    {
        public string apiKey { get; set; }
        public string userOid { get; set; }
        public string ver { get; set; }
        public string locale { get; set; }
        public string ipaddress { get; set; }
        public ResponseJson json { get; set; }

        
    }
    public class ResponseJson
    {
        public string orderMid { get; set; }
        public string request_uuid { get; set; }
        public ResponseDataModel data { get; set; }
        public ResponseMetaModel metadata { get; set; }
    }
    public class ResponseMetaModel
    {
        public string status { get; set; }//狀態Code
        public string description { get; set; }//回傳描述
    }
    public class ResponseDataModel
    {
       public List<ResponseOrderInfoModel> orderinfo { get; set; }
    }
    public class ResponseOrderInfoModel
    {
        public string kkOrderNo { get; set; }
        public List<string> ticket { get; set; }
        public string type { get; set; }
        public string fileExtention { get; set; }
        public string result { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class RequestJavaModel
    {
        public string apiKey { get; set; }
        public string userOid { get; set; }
        public string ver { get; set; }
        public string locale { get; set; }
        public string ipaddress { get; set; }
        public RequestJson json { get; set; }
    }
    public class RequestJson
    {
        public string orderMid { get; set; }
        public string request_uuid { get; set; }
        public RequestDataModel data { get; set; }
        public RequesteMetaModel metadata { get; set; }
        //orderInfo需要參數
        public string memberUuid { get; set; }
        public string deviceId { get; set; }
        public string tokenKey { get; set; }
    }
    public class RequesteMetaModel
    {
        public string status { get; set; }//狀態Code
        public string description { get; set; }//回傳描述
    }
    public class RequestDataModel
    {
        public List<RequestOrderInfoModel> orderinfo { get; set; }
        public string base64str { get; set; }
    }
    public class RequestOrderInfoModel
    {
        public string kkOrderNo { get; set; }
        public List<string> ticket { get; set; }
        public string type { get; set; }
        public string fileExtention { get; set; }
        public string result { get; set; }
    }
    public class ConvertBase64Rq
    {
        public string requestUuid { get; set; }
        public string fileUrl { get; set; } // ConvertBase64用
    }
}

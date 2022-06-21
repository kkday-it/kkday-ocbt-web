using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    
    public class ResponseJson
    {
        public ResponseMetaModel metadata { get; set; }
    }
    public class ResponseMetaModel
    {
        public string status { get; set; }//狀態Code
        public string description { get; set; }//回傳描述
    }

    /// <summary>
    /// Convert to Base64 Response
    /// </summary>
    public class ConvertBase64Rs : ResponseJson
    {
        public ResponseDataModel data { get; set; }
    }
    public class ResponseDataModel
    {
        public string base64str { get; set; }
    }
}

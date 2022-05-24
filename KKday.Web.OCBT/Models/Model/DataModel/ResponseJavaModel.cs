using System;
namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class ResponseJavaModel
    {
        public ResponseMetaModel metadata { get; set; }   
    }
    public class ResponseMetaModel
    {
        public string status { get; set; }//狀態Code
        public string description { get; set; }//回傳描述
    }
}

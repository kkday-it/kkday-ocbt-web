using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class CurrencyModels
    {
        public string result { get; set; }
        public string result_msg { get; set; }
        public List<CurrencyModel> currencies { get; set; }
    }
    public class CurrencyModel
    {
        public string code { get; set; }//幣別金額
        public double rate { get; set; }//匯率
        public int precision { get; set; }//精准度
        public string roundingMethod { get; set; }//依據此欄位做四捨五入
    }
}

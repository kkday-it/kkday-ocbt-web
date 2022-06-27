using System;
namespace KKday.Web.OCBT.Models.Model.Order
{
    public class CheckOrderFromB2dRqModel
    {
        public string result_type { get; set; }
        public OrderFromB2d order { get; set; }
    }
    public class OrderFromB2d
    {
        public string order_no { get; set; }
        public string status { get; set; }
        public Vouch voucher { get; set; }
    }
    public class Vouch
    {
        public bool isGenerate { get; set; }
    }
}

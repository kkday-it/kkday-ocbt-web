using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.CartBooking
{
    public class CartBookingRsModel: BookingValidModel
    {
        public string  orde_master_mid{get;set;}
        public List<orderSimpleModel> orders { get; set; }
    }
    public class orderSimpleModel
    {
        public string order_no { get; set; }
        public string order_oid { get; set; }
        public string order_mid { get; set; }
    }
}

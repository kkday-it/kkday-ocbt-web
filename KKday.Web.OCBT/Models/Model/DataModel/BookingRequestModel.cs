using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.DataModel
{
    public class BookingRequestModel
    {
        public string requestUuid { get; set; }
        public string source { get; set; }
        public JavaOrderModel order { get; set; }
    }
    public class JavaOrderModel
    {
        public int packageOid { get; set; }
        public string pkgName { get; set; }
        public string orderMid { get; set; }
        public string orderMasterMid { get; set; }
        public int orderMasterOid { get; set; }
        public int orderOid { get; set; }//新增orderOid
        public string contactCountryCd { get; set; }
        public string contactEmail { get; set; }
        public string contactTelCd { get; set; }
        public string telCountryCd { get; set; }
        public string contactTel { get; set; }
        public string contactFirstname { get; set; }
        public string contactLastname { get; set; }
        public string crtDt { get; set; }
        public string begLstGoDt { get; set; }
        public string endLstBackDt { get; set; }
        public string eventTime { get; set; }
        public string prodCurrCd { get; set; }
        //public string eventBackupData { get; set; }
        //public List<EventBackupData> eventBackupData { get; set; }
        public List<OrderCusList> orderCusList { get; set; }
        public string memberLang { get; set; }

        public string voucherType { get; set; }
        public string externalItemOid { get; set; }
        public List<PriceList> lstPrice { get; set; }

        //發送slack救單用
        public string prodName { get; set; }
        public long? prodOid { get; set; }
    }
    public class EventBackupData
    {
        public int? eventSort { get; set; }
        public string eventDate { get; set; }
        public string eventTime { get; set; }
    }

    public class PriceList
    {
        public int qty { get; set; }
        public double price { get; set; }
        public double priceNetOrg { get; set; }
        public string specTicket { get; set; }
        public string skuOid { get; set; }
        public List<SpecInfo> specInfo { get; set; }

    }

    public class SpecInfo
    {
        public string externalTitle { get; set; }
        public string externalValue { get; set; }

    }
    public class OrderCusList
    {
        public string cusLastname { get; set; }
        public string cusFirstname { get; set; }
        public string cusLocalFirstname { get; set; }
        public string cusLocalLastname { get; set; }
        public string cusGender { get; set; }
        public object passportId { get; set; }
        public object cusBirthday { get; set; }
        public object countryCd { get; set; }
        public object cusField { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static KKday.Web.OCBT.Models.Model.Order.OmdlConverter;

namespace KKday.Web.OCBT.Models.Model.Order
{
    public class JavaOrderInfoModel
    {
        public ContentData content { get; set; }
    }
    public class ContentData
    {
        public List<OrderModuleDataModel> orderModuleDataList { get; set; }
        public Order order { get; set; }
    }
    public class OrderModuleDataModel
    {
        public string moduleType { get; set; }
        [JsonConverter(typeof(ModuleDataConverter))]
        public List<ModuleModel> moduleData { get; set; }

    }
    public class ModuleModel
    {
        #region OMDL_CUST_DATA
        public EnglishName englishName { get; set; }
        public string gender { get; set; }
        public string birthday { get; set; }
        public Nationality nationality { get; set; }
        public PassportInfo passport { get; set; }
        public LocalName localName { get; set; }
        public Height height { get; set; }
        public ShoeSize shoeSize { get; set; }
        public Meal meal { get; set; }
        public double? glassDiopter { get; set; }
        #endregion
        #region OMDL_SHUTTLE
        public string shuttleDate { get; set; }
        public DesignatedLocation designatedLocation { get; set; }
        public DesignatedByCustomer designatedByCustomer { get; set; }
        public CharterRoute charterRoute { get; set; }
        #endregion
        #region OMDL_CONTACT
        public ContactName contactName { get; set; }
        public ContactTel contactTel { get; set; }
        public ContactApp contactApp { get; set; }
        #endregion
        #region OMDL_PSGR_DATA
        public int? qtyAdult { get; set; }
        public int? qtyChild { get; set; }
        public int? qtyInfant { get; set; }
        public int? qtyCarryLuggage { get; set; }
        public int? qtyCheckedLuggage { get; set; }
        public QtyChildSeat qtyChildSeat { get; set; }
        public QtyInfantSeat QtyInfantSeat { get; set; }
        #endregion
        #region OMDL_FLIGHT_INFO
        public Arrival arrival { get; set; }
        public Departure departure { get; set; }
        #endregion
        #region OMDL_RENT_CAR
        public PickUp pickUp { get; set; }
        public DropOff dropOff { get; set; }
        public bool? isNeedFreeGPS { get; set; }
        public bool? isNeedFreeWiFi { get; set; }
        #endregion
        #region OMDL_OTHER_DATA
        public string activationDate { get; set; }
        public string mobileIMEI { get; set; }
        public string mobileModelNumber{ get; set; }
        public string exchangeLocationID { get; set; }
        public OrderProdSetting orderProdSetting { get; set; }
        #endregion
        #region OMDL_SEND_DATA
        public ReceiverName receiverName { get; set; }
        public ReceiverTel receiverTel { get; set; }
        public SendToCountry sendToCountry { get; set; }
        public SendToHotel sendToHotel { get; set; }
        public ShipInfo shipInfo { get; set; }
        #endregion
    }
    #region OMDLCUSTDATA
    public class EnglishName : CommonName
    {

    }
    public class Nationality
    {
        public string nationalityCode { get; set; }
        public string HKMOIdentityNumber { get; set; }
        public string TWIdentityNumber { get; set; }
        public string MTPNumber { get; set; }
    }
    public class PassportInfo
    {
        public string passportNo { get; set; }
        public string passportExpDate { get; set; }
    }
    public class LocalName : CommonName
    {

    }
    public class Height : CommonUnit
    {
    }
    public class ShoeSize : CommonUnit
    {
    }
    public class Meal
    {
        public MealType mealType { get; set; }
        public List<string> excludeFoodType { get; set; }
        public FoodAllergy foodAllergy { get; set; }

    }
    public class MealType
    {
        public string typeName { get; set; }
    }
    public class FoodAllergy
    {
        public bool? isFoodAllergy { get; set; }
        public string allergenList { get; set; }
    }
    #endregion
    #region OMDLSHUTTLE
    public class DesignatedLocation
    {
        public OrderProdSetting orderProdSetting { get; set; }
    }
    public class OrderProdSetting: CommonTimeSlot
    {
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public CommonTimerange timeRange { get; set; }
        //omdlrentcar
        public string routeLocal { get; set; }
        public string officeName { get; set; }
        public string addressEng { get; set; }
        public string addressLocal { get; set; }
        //omdlotherdata
        public string name { get; set; }
        public string address { get; set; }
        public string note { get; set; }
        public string businessHours { get; set; }



    }
    public class DesignatedByCustomer
    {
        public PickUp pickUp { get; set; }
        public DropOff dropOff { get; set; }
    }
    public class PickUp
    {
        public string location { get; set; }
        public Time time { get; set; }
        public OrderProdSetting orderProdSetting { get; set; }
        public CommonTimeSlot datetime { get; set; }

    }
    public class DropOff
    {
        public string location { get; set; }
        public OrderProdSetting orderProdSetting { get; set; }
        public CommonTimeSlot datetime { get; set; }
    }
    public class CharterRoute
    {
        public bool? isCustom { get; set; }
        public List<string> customRoutes { get; set; }
        public string routesID { get; set; }
        public OrderProdSetting orderProdSetting { get; set; }

    }
    public class Time:CommonTimeSlot
    {
        public bool? isCustom { get; set; }
        public OrderProdSetting orderProdSetting { get; set; }
    }

    #endregion
    #region OMDLCONTACTDATA
    public class ContactName : CommonName
    { }
    public class ContactTel: CommonTel
    {
        public bool? haveTel { get; set; }
        
    }
    public class ContactApp
    {
        public bool? haveApp { get; set; }
        public string appAccount { get; set; }
        public AppType appType { get; set; }
    }
    public class AppType
    {
        public string typeName { get; set; }
    }
    #endregion
    #region OMDLPSGRDATA
    public class QtyChildSeat:CommonSeat
    {
        
    }
    public class QtyInfantSeat : CommonSeat
    {
    }

    #endregion
    #region OMDLFLIGHTINFO
    public class Arrival:CommonFlight
    {
        public CommonTimeSlot arrivalDatetime { get; set; }
        
        public bool? isNeedToApplyVisa { get; set; }
    }
    
    public class Departure:CommonFlight
    {
        public CommonTimeSlot departureDatetime { get; set; }
        public bool? haveBeenInCountry { get; set; }
    }
    #endregion
    #region OMDLRENTCAR

    #endregion
    #region OMDLSENDDATA
    public class ReceiverName : CommonName { }
    public class ReceiverTel: CommonTel
    {

    }
    public class SendToCountry
    {
        public ReceiveAddress receiveAddress { get; set; }
    }
    public class ReceiveAddress
    {
        public string address { get; set; }
        public string cityName { get; set; }
        public string zipCode { get; set; }
        public string countryName { get; set; }
    }
    public class SendToHotel
    {
        public BuyerPassportEnglishName buyerPassportEnglishName { get; set; }
        public BuyerLocalName buyerLocalName { get; set; }
        public string bookingOrderNo { get; set; }
        public string bookingWebsite { get; set; }
        public string checkInDate { get; set; }
        public string checkOutDate { get; set; }
        public string hotelName { get; set; }
        public string hotelAddress { get; set; }
        public string hotelTel { get; set; }
    }
    public class BuyerPassportEnglishName:CommonName
    { }
    public class BuyerLocalName:CommonName
    {
    }
    public class ShipInfo
    {
        public string shipDate { get; set; }
        public string trackingNumber { get; set; }
    }
    #endregion

    public class CommonUnit
    {
        public string unit { get; set; }
        public string value { get; set; }
    }
    public class CommonName
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
    public class CommonTimerange
    {
        public CommonTimeSlot from { get; set; }
        public CommonTimeSlot to { get; set; }
    }
    public class CommonTimeSlot
    {
        public string date { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
    }
    public class CommonSeat
    {
        public int? supplierProvided { get; set; }
        public int? selfProvided { get; set; }
    }
    public class CommonFlight
    {
        public string flightNo { get; set; }
        public string flightType { get; set; }
        public string airline { get; set; }
        public string airport { get; set; }
        public string terminalNo { get; set; }
    }
    public class CommonTel
    {
        public string telCountryCode { get; set; }
        public string telNumber { get; set; }
    }

    


    
}
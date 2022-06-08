using System;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Product
{
    public class WMSProductModuleModel
    {
        public string result { get; set; }//(跟Product共用)
        public string result_msg { get; set; }//(跟Product共用)
        public List<string> module_type { get; set; }//此商品有套用的module類型

        public CusData module_cust_data { get; set; }//旅客資料
        public ContactData module_contact_data { get; set; }//旅遊期間聯絡人
        public SimWifi module_sim_wifi { get; set; }//其他資料
        public CarPasgr module_car_pasgr { get; set; }//乘客資料
        public FlightInfo module_flight_info { get; set; }//抵達班機資訊+離開班機資訊  pkg_flightInfoType=03 ,抵達arr班機資訊 flightInfoType=02, 離開dep班機資訊=flightInfoType=01
        public SendData module_send_data { get; set; }//寄送資料
        public RentCar module_rent_car { get; set; }//租車資料
        public VenueInfo module_venue_info { get; set; }//接送資料 venueType=03,04
        public PkgmdlSetting module_pkgmdl_setting { get; set; } //套餐設定

        //[booking]OMDL_OTHER_DATA.exchangeLocationID
        public List<Location> module_exchange_location_list { get; set; }//其他資料 >>領取櫃台 exchangeType=05 (跟Product共用)
        //[booking]guideLang
        public List<GuideLanguage> module_guide_lang_list { get; set; } //導覽語言 (跟Product共用)

        //[booking]eventOid & eventBackupData
        public List<Event> module_event_list { get; set; }
    }
    public class Event
    {
        public string day { get; set; }
        public string event_times { get; set; }
    }


    #region CusData Model Type = {PMDL_CUST_DATA,orderCusList}
    public class CusData
    {
        public bool is_require { get; set; }//cust data request
        public string cus_type { get; set; } //01:旅客代表人 02:每位旅客資料

        public EnglisName englis_name { get; set; }
        public LocalName local_name { get; set; }
        public Gender gender { get; set; }
        public Nationality nationality { get; set; }
        public Birthday birthday { get; set; }
        public Passport passport { get; set; }
        public High high { get; set; }
        public Weight weight { get; set; }
        public ShoeSize shoe_size { get; set; }
        public Meal meal { get; set; }
        public GlassDiopter glass_degree { get; set; }

    }

    public class EnglisName //[booking]FirstName + LastName
    {
        public bool is_require_FirstName { get; set; }
        public bool is_require_LastName { get; set; }
    }

    public class LocalName //[booking]FirstName + LastName
    {
        public bool is_require_FirstName { get; set; }
        public bool is_require_LastName { get; set; }
    }

    public class Gender
    {
        public bool is_require { get; set; }
        public List<GenderType> gender_list { get; set; }
    }

    public class GenderType
    {
        public string type { get; set; }
        public string type_name { get; set; }
    }


    public class Nationality
    {
        public bool is_require { get; set; }  //[booking] nationalityCode
        public List<NationInfo> nation_list { get; set; }
        public NationalityID nationality_id { get; set; }
    }
    public class NationalityID
    {
        public bool is_require_TW { get; set; }//身分證
        public bool is_require_MTP { get; set; }//台胞證
        public bool is_require_HKMO { get; set; }//港澳身分證字號
    }
    public class NationInfo
    {
        public string country_local_name { get; set; }

        public string country_code { get; set; }
        public string country_tel_code { get; set; }
        public string country_tel_info { get; set; }
    }

    public class Birthday
    {
        public bool is_require { get; set; }
    }

    public class Passport
    {
        public bool is_require_PassprotNo { get; set; }
        public bool is_require_PassprotExpDate { get; set; }
    }

    public class High
    {
        public bool is_require { get; set; }
        public List<Unit> unit_list { get; set; }//01公分 02英尺 
    }

    public class Weight
    {
        public bool is_require { get; set; }//01公斤 02英鎊
        public List<Unit> unit_list { get; set; }
    }

    public class Unit
    {
        public string unit_code { get; set; } //01公斤 02英鎊
        public string unit_name { get; set; }

        public string size_range_start { get; set; }
        public string size_range_end { get; set; }
    }

    public class ShoeSize // [booking] type(M/W/C) + unit + value
    {
        public bool is_require { get; set; }
        public Man man { get; set; }
        public Woman woman { get; set; }
        public Child child { get; set; }
    }
    public class Man
    {
        public bool is_provided { get; set; }
        public List<Unit> unit_list { get; set; }
    }
    public class Woman
    {
        public bool is_provided { get; set; }
        public List<Unit> unit_list { get; set; }
    }
    public class Child
    {
        public bool is_provided { get; set; }
        public List<Unit> unit_list { get; set; }
    }

    public class Meal //[booking] mealType + excludeFoodType[] + foodAllergy(allergenList+isFoodAllergy)
    {
        public bool is_require { get; set; }

        public List<MealType> meal_list { get; set; }
        public ExcludeFood exclude_food { get; set; }
    }
    public class MealType
    {
        public bool is_provided { get; set; }
        public string meal_type { get; set; }
        public string meal_type_name { get; set; }
    }
    public class ExcludeFood
    {
        public bool is_exclude { get; set; } //是否可排除哪些食物
        public List<Food> food_list { get; set; }//客人不要哪些食物
        public AllergyFood allergy_food { get; set; }
    }
    public class Food
    {
        public bool can_exclude { get; set; }
        public string food_type { get; set; }
        public string food_type_name { get; set; }
    }
    public class AllergyFood
    {
        public bool is_require_FoodAllergy { get; set; } //過敏   
    }

    public class GlassDiopter
    {
        public bool is_require { get; set; }
        public string degree_range_start { get; set; }
        public string degree_range_end { get; set; }
    }

    #endregion

    #region ContactData Model Type = {PMDL_CONTACT_DATA,OMDL_CONTACT_DATA}
    public class ContactData
    {
        public bool is_require { get; set; }
        public ContactName contact_name { get; set; }
        public ContactTel contact_tel { get; set; }
        public ContactApp contact_app { get; set; }

    }
    public class ContactName //[booking]firstName+lastName
    {
        public bool is_require_FirstName { get; set; }
        public bool is_require_LastName { get; set; }
    }

    public class ContactTel //[booking] "YesNo"+ telCountryCode+ telNumber
    {
        public bool is_require_TelNumber { get; set; }
        public bool is_require_TelCountryCode { get; set; }
        public List<NationInfo> tel_code_list { get; set; }
    }

    public class ContactApp //[booking] "YesNo" + appType + appAccount
    {
        public bool is_require { get; set; } //"YesNo"
        public List<App> app_type_list { get; set; }
        public bool is_require_AppAccount { get; set; }
    }
    public class App
    {
        public bool is_supported { get; set; }
        public string app_type { get; set; }
        public string app_name { get; set; }
    }

    //public class TelCode
    //{
    //    public bool is_supported { get; set; }
    //    public string app_type { get; set; }
    //    public string app_name { get; set; }
    //}
    #endregion

    #region SimWifi Model type = {PMDL_SIM_WIFI,OMDL_OTHER_DATA}  
    public class SimWifi
    {
        public bool is_require { get; set; }
        public MobileModelNumber mobile_model_no { get; set; }
        public MobileIMEI mobile_IMEI { get; set; }
        public ActivationDate activation_date { get; set; }
    }
    public class MobileModelNumber
    {
        public bool is_require { get; set; }//手機型號
    }

    public class MobileIMEI
    {
        public bool is_require { get; set; }//IMEI
    }

    public class ActivationDate
    {
        public bool is_require { get; set; } //開通日期
    }

    #endregion

    #region CarPasgr Model type= {PMDL_CAR_PSGR,OMDL_PSGR_DATA}
    public class CarPasgr
    {
        public bool is_require { get; set; }
        public AdultQty adul_qty { get; set; }
        public ChildQty child_qty { get; set; }
        public InfantQty infant_qty { get; set; }
        public ChildSeat child_safety_seat { get; set; }
        public InfantSeat infant_safety_seat { get; set; }
        public CarryLuggageQty carry_luggage_qty { get; set; }//手提行李
        public CheckedLuggageQty checked_luggage_qty { get; set; }//托運行李
    }
    public class AdultQty
    {
        public bool is_require { get; set; }
        public int? age_range_start { get; set; }
        public int? age_range_end { get; set; }
    }
    public class ChildQty
    {
        public bool is_require { get; set; }
        public int? age_range_start { get; set; }
        public int? age_range_end { get; set; }
    }
    public class InfantQty
    {
        public bool is_require { get; set; }
        public int? age_range_start { get; set; }
        public int? age_range_end { get; set; }
    }
    public class ChildSeat //[booking]supplierProvided + selfProvided
    {
        public bool is_require_supplierProvided { get; set; }
        public bool is_require_selfProvided { get; set; }
        public int? age_range_start { get; set; }
        public int? age_range_end { get; set; }
    }
    public class InfantSeat //[booking] supplierProvided + selfProvided
    {
        public bool is_require_supplierProvided { get; set; }
        public bool is_require_selfProvided { get; set; }
        public int? age_range_start { get; set; }
        public int? age_range_end { get; set; }
    }
    public class CarryLuggageQty
    {
        public bool is_require { get; set; }
    }

    public class CheckedLuggageQty
    {
        public bool is_require { get; set; }
    }

    #endregion

    #region FlightInfo Model 有改架構 type=PMDL_FLIGHT_INFO
    public class FlightInfo
    {
        public bool is_require { get; set; }
        public Arrival arrival { get; set; }
        public Departure departure { get; set; }
    }
    public class Arrival
    {
        public bool is_require_FlightType { get; set; }
        public List<FlightType> flight_type_list { get; set; }
        public bool is_require_Date { get; set; } //[booking]date + hour + minute
        public bool is_require_Hour { get; set; }
        public bool is_require_Minute { get; set; }
        public bool is_require_Airport { get; set; }
        public List<Airport> airport_list { get; set; }
        public bool is_require_Airline { get; set; }
        public bool is_require_FlightNo { get; set; }
        public bool is_require_TerminalNo { get; set; }
        public bool is_need_ApplyVisa { get; set; }//[booking] "YesNo" 是否需要辦理落地簽證
    }
    public class Departure
    {
        public bool is_require_FlightType { get; set; }
        public List<FlightType> flight_type_list { get; set; }
        public bool is_require_Date { get; set; } //date + hour + minute
        public bool is_require_Hour { get; set; }
        public bool is_require_Minute { get; set; }
        public bool is_require_Airport { get; set; }
        public List<Airport> airport_list { get; set; }
        public bool is_require_Airline { get; set; }
        public bool is_require_FlightNo { get; set; }
        public bool is_require_TerminalNo { get; set; }
        public bool is_require_HaveBeenInCountry { get; set; } //已在商品所在國家

    }
    public class FlightType
    {
        public string type { get; set; }
        public string type_name { get; set; }
    }
    public class Airport
    {
        public string airport_code { get; set; }
        public string airport_name { get; set; }
        public string area_code { get; set; }
    }

    #endregion

    #region SendData Model  有改架構  type={PMDL_SEND_DATA,OMDL_SEND_DATA}
    public class SendData
    {
        public bool is_require { get; set; }
        public ReceiverName receiver_name { get; set; } //firstName+lastName
        public ReceiverTel receiver_tel { get; set; } //telCountryCode + telNumber
        public ReceiverAddress receiver_address { get; set; }
        public SendToHotel send_to_hotel { get; set; }

    }
    public class ReceiverName
    {
        public bool is_require_FirstName { get; set; }
        public bool is_require_LastName { get; set; }
    }
    public class ReceiverTel
    {
        public bool is_require_TelNumber { get; set; }
        public bool is_require_TelCountryCode { get; set; }
        public List<NationInfo> tel_code_list { get; set; }
    }
    public class ReceiverAddress //收件國家 countryCode +countryName /城市 cityCode+cityName /郵遞區號 zipCode /地址 address
    {
        public bool is_require_Country { get; set; }
        public bool is_require_City { get; set; }
        public bool is_require_ZipCode { get; set; }
        public bool is_require_Address { get; set; }
        public List<Country> country_list { get; set; }
    }

    public class SendToHotel
    {
        public bool is_provided { get; set; }
        public SendToHotelInfo send_to_hotel_info { get; set; }
    }
    public class SendToHotelInfo
    {
        public bool is_require_HotelName { get; set; }
        public bool is_require_HotelAddress { get; set; }
        public bool is_require_HotelTel { get; set; }
        public bool is_require_BuyerPassportEnglishFirstName { get; set; } //[booking]firstName + lastName
        public bool is_require_BuyerPassportEnglishLastName { get; set; }
        public bool is_require_BuyerLocalFirstName { get; set; } //[booking]firstName + lastName
        public bool is_require_BuyerLocalLastName { get; set; }
        public bool is_require_BookingOrderNo { get; set; }
        public bool is_require_BookingWebsite { get; set; }
        public bool is_require_CheckOutDate { get; set; }
        public bool is_require_CheckInDate { get; set; }

    }
    #endregion

    #region RentCar Model {PMDL_RENT_CAR,OMDL_RENT_CAR,OMDL_SHUTTLE}
    /// <summary>
    /// rent_type =02
    /// OMDL_RENT_CAR
    /// 
    /// rent_type =03
    /// OMDL_SHUTTLE.charterRoute
    /// 
    /// </summary>

    public class RentCar
    {
        public bool is_require { get; set; }
        public string rent_type { get; set; }//01{pickUp=true,dropOff=false},02{pickUp=true,dropOff=true}
        public bool is_require_SDate { get; set; } //取車日期
        public bool is_require_EDate { get; set; } //還車日期
        public RentOffice rent_office { get; set; } //01,02
        public DriverShuttle driver_shuttle { get; set; } //03
    }

    public class RentOffice
    {
        public bool is_require_SLocation { get; set; } //取車地點
        public bool is_require_ELocation { get; set; } //還車地點
        public List<Office> office_list { get; set; }
        public bool is_ProvidedFreeWiFi { get; set; } //YesNo
        public bool is_ProvidedFreeGPS { get; set; } //YesNo
    }
    public class Office
    {
        public int sort { get; set; }
        public string id { get; set; }
        public string area_code { get; set; }
        public string office_name { get; set; }
        public string address_eng { get; set; }
        public string address_local { get; set; }
        public int drop_off_interval { get; set; }
        public BusinessHour business_hour { get; set; }//與 product公用
    }


    public class DriverShuttle
    {
        public CharterRoute charterRoute { get; set; }
    }

    public class CharterRoute //包車路線
    {
        public bool is_require { get; set; }
        public List<Routes> route_list { get; set; }
        public RouteCustomized route_custom { get; set; }
    }

    public class Routes
    {
        public int sort { get; set; }
        public string id { get; set; }
        public string routeEng { get; set; } //路線說明(英文）
        public string routeLocal { get; set; } //路線說明(在地語系)

    }
    public class RouteCustomized
    {
        public bool is_require { get; set; }
        public bool is_require_Location { get; set; }
        public int? route_limit { get; set; }

    }


    #endregion

    #region VenueInfo Model {PMDL_VENUE,OMDL_SHUTTLE}
    /// <summary>
    /// venue_type = 03 
    /// OMDL_SHUTTLE.designatedLocation
    /// 
    /// venue_type = 04
    /// OMDL_SHUTTLE.designatedByCustomer
    /// 
    /// </summary>


    public class VenueInfo
    {
        public bool is_require { get; set; }
        public string venue_type { get; set; }
        public bool is_require_Date { get; set; } //接送日期

        public List<DesignatedLocation> designated_location_list { get; set; }
        public DesignatedByCustomer designated_by_customer { get; set; }

    }
    public class DesignatedLocation //venueType= 03 地點指定 
    {
        public int sort { get; set; }
        public string id { get; set; }
        public string location_name { get; set; }
        public string location_address { get; set; }
        public string image_url { get; set; }
        public string time_range_start { get; set; }
        public string time_range_end { get; set; }

    }

    public class DesignatedByCustomer //venueType=04 旅客指定
    {
        public PipickUp pick_up { get; set; }
        public DropOff drop_off { get; set; }
    }
    public class DropOff
    {
        public bool is_require_Location { get; set; }
    }
    public class PipickUp
    {
        public bool is_require_Location { get; set; }
        public PickUpTime time { get; set; }
    }

    public class PickUpTime
    {
        public bool is_require { get; set; }
        public TimeCustomized custom { get; set; }
        public List<Time> time_list { get; set; }

    }
    public class Time
    {
        public string id { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
    }
    public class TimeCustomized
    {
        public bool is_allow { get; set; }
        public string time_range_start { get; set; } //VenueInfo
        public string time_range_end { get; set; } //VenueInfo

    }
    public class Location
    {
        public string id { get; set; }
        public List<BusinessHour> businessHours { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string note { get; set; }

    }
    public class From
    {
        public int? hour { get; set; }
        public int? minute { get; set; }
    }

    public class To
    {
        public int? hour { get; set; }
        public int? minute { get; set; }
    }

    public class BusinessHour
    {
        public string weekDays { get; set; }
        public From from { get; set; }
        public To to { get; set; }
    }
    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<City> cities { get; set; }
    }


    #endregion

    #region PKGMDL_SETTING

    public class PkgmdlSetting
    {
        public List<ModuleSetting> moduleSetting { get; set; }
    }

    public class ModuleSetting
    {
        public string pkgOid { get; set; }
        public ModuleSetting2 moduleSetting { get; set; }
    }

    public class ModuleSetting2
    {
        public SendInfoType sendInfoType { get; set; }
        public FlightInfoType flightInfoType { get; set; }
    }

    public class SendInfoType
    {
        public string value { get; set; }
        public string countryCode { get; set; }
    }

    public class FlightInfoType
    {
        public string value { get; set; }
    }
    public class GuideLanguage
    {
        public string lang_code { get; set; }
        public string lang_name { get; set; }

    }
    public class City
    {
        public string city_code { get; set; }
        public string country_code { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    #endregion

}

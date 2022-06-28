using System;
using System.Collections.Generic;
using System.Linq;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.CartBooking;
using KKday.Web.OCBT.Models.Model.DataModel;
using KKday.Web.OCBT.Models.Model.Order;
using KKday.Web.OCBT.Models.Model.Product;
using KKday.Web.OCBT.Proxy;
using Newtonsoft.Json;

namespace KKday.Web.OCBT.Models.Repository
{
    public class BookingRepository
    {
        public CartBookingModel SetBookingModel(WMSProductModuleModel module, JavaOrderModel order)
        {
            try
            {
                #region 取母單的訂單資訊
                var masterInfo =JsonConvert.DeserializeObject<JavaOrderInfoModel>(GetMasterOrderInfo(order.orderMid));
                #endregion
                CartBookingModel rs = new CartBookingModel()
                {
                    crt_user = Website.Instance.Configuration["WMS_API:CompanyData:AccountXid"],
                    booking_type = "api",
                    locale=order.memberLang,
                    company_xid=Website.Instance.Configuration["WMS_API:CompanyData:CompanyXid"],
                    contactFirstname=order.contactFirstname,
                    contactLastname=order.contactLastname,
                    contactEmail=order.contactEmail,
                    telCountryCd=order.telCountryCd,
                    contactTel=order.contactTel,
                    contactCountryCd=order.contactCountryCd,
                    lstGoDt=order.begLstGoDt,
                    lstBackDt=order.endLstBackDt,
                    guideLang=order.memberLang,
                    note="",
                    crtDevice="API",
                    crtBrowser="N",
                    crtBrowserVersion="1",
                    multiPricePlatform="03",
                    sourceCode="B2D",
                    modules =new modulesData()
                    
                };
                if (module.result == "0000")
                {
                    #region 取得custData
                    if (module.module_cust_data != null && module.module_cust_data.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_CUST_DATA").Count() > 0)
                        {
                            rs.travelerData = GetCusModel(module.module_cust_data, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_CUST_DATA").Select(x => x.moduleData).First());
                        }
                    }
                    else
                    {
                        rs.travelerData = new List<CusDataInfo>();
                    }


                    #endregion
                    #region 取得contactData
                    if (module.module_contact_data != null && module.module_contact_data.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_CONTACT_DATA").Count() > 0)
                        {
                            rs.modules.contactData = GetContModel(module.module_contact_data, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_CONTACT_DATA").Select(x => x.moduleData).First());
                        }
                        
                    }
                    else
                    {
                        rs.modules.contactData = new contactDataM
                        {
                            moduleType = "OMDL_CONTACT_DATA",
                            moduleData = new moduleData_contactData()
                            {
                                contactName=new contactNameInfo(),
                                contactApp=new contactAppInfo() ,
                                contactTel=new contactTelInfo()
                            }
                        };
                    }
                    #endregion
                    #region 取得sendData
                    if (module.module_send_data != null && module.module_send_data.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_SEND_DATA").Count() > 0)
                        {
                            rs.modules.sendData = GetSendModel(module.module_send_data, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_SEND_DATA").Select(x => x.moduleData).First());
                        }
                    }
                    else
                    {
                        rs.modules.sendData = new sendDataM
                        {
                            moduleType = "OMDL_SEND_DATA",
                            moduleData = new moduleData_sendData()
                            {
                                receiverName=new receiverNameInfo(),
                                receiverTel=new receiverTelInfo(),
                                shipInfo=new shipInfoInfo(),
                                sendToCountry=new sendToCountryInfo() { receiveAddress=new receiveAddressInfo() { } },
                                sendToHotel=new sendToHotelInfo() { buyerLocalName=new buyerLocalNameInfo(),buyerPassportEnglishName=new buyerPassportEnglishNameInfo()}
                            }
                        };
                    }
                    #endregion
                    #region 取得FlyData
                    if (module.module_flight_info != null && module.module_flight_info.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_FLIGHT_INFO").Count() > 0)
                        {
                            rs.modules.flightInfoData = GetFlightInfoModel(module.module_flight_info, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_FLIGHT_INFO").Select(x => x.moduleData).First());
                        }
                    }
                    else
                    {
                        rs.modules.flightInfoData = new flightInfoDataM
                        {
                            moduleType = "OMDL_FLIGHT_INFO",
                            moduleData = new moduleData_FlightInfo()
                            {
                                arrival = new arrivalInfo() {
                                    arrivalDatetime=new arrivalDatetimeInfo(),
                                },
                                departure=new departureInfo()
                                {
                                    departureDatetime=new departureDatetimeInfo()
                                }

                            }
                        };
                    }
                    #endregion
                    #region 取得PsgData
                    if (module.module_car_pasgr != null && module.module_car_pasgr.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_PSGR_DATA").Count() > 0)
                        {
                            rs.modules.passengerData = GetPassengerModel(module.module_car_pasgr, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_PSGR_DATA").Select(x => x.moduleData).First());
                        }
                    }
                    else
                    {
                        rs.modules.passengerData = new passengerDataM
                        {
                            moduleType = "OMDL_PSGR_DATA",
                            moduleData = new moduleData_passenger()
                            {
                                qtyChildSeat=new qtyChildSeatInfo(),
                                qtyInfantSeat=new qtyInfantSeatInfo()
                            }
                        };
                    }
                    #endregion
                    #region 取得OtherData
                    if (module.module_sim_wifi != null && module.module_sim_wifi.is_require)
                    {
                        if (masterInfo?.content?.orderModuleDataList != null && masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_OTHER_DATA").Count() > 0)
                        {
                            rs.modules.otherData = GetOtherModel(module.module_sim_wifi, masterInfo.content.orderModuleDataList.Where(m => m.moduleType == "OMDL_OTHER_DATA").Select(x => x.moduleData).First());
                        }
                    }
                    else
                    {
                        rs.modules.otherData = new otherDataM
                        {
                            moduleType = "OMDL_OTHER_DATA",
                            moduleData = new moduleData_otherData()
                            {
                                
                            }
                        };
                    }
                    #endregion

                    rs.modules.carRentingData = new carRentingDataM
                    {
                        moduleType = "OMDL_RENT_CAR",
                        moduleData = new moduleData_CarRent()
                        {
                            dropOff=new dropOffInfo_forCar()
                            {
                                datetime=new dateTimeInfo()
                                
                            },
                            pickUp=new pickUpInfo_forCar()
                            {
                                datetime=new dateTimeInfo()
                            }
                        }
                    };//目前不支援rentCar模組
                    rs.modules.shuttleData = new shuttleDataM
                    {
                        moduleType = "OMDL_SHUTTLE",
                        moduleData = new moduleData_Shuttle()
                        {
                            charterRoute=new charterRouteInfo() { },
                            designatedByCustomer=new designatedByCustomerInfo() { dropOff=new dropOffInfo() { },pickUp=new pickUpInfo() { time=new timeInfo() { } } },
                            designatedLocation=new designatedLocationInfo() { },
                            
                        }
                    };//目前不支援pickup模組

                    return rs;

                }
                else
                {
                    Website.Instance.logger.Info($"SetBookingModel ModuleData={JsonConvert.SerializeObject(module)}");
                    throw new Exception("ModuleData Error.");
                }
            }
            catch (Exception ex)
            {

                Website.Instance.logger.Info($"SetBookingModel Error ModuleData={JsonConvert.SerializeObject(module)},JavaOrder={JsonConvert.SerializeObject(order)}, message:{ex.Message},stacktrace:{ex.StackTrace}");
                throw ex;
            }
            
        }

        public List<CusDataInfo> GetCusModel(CusData cusModel, List<ModuleModel> orderMasterModule ,int qty=1)
        {
            try
            {
                List<CusDataInfo> list = new List<CusDataInfo>();
                if (cusModel.cus_type == "01")
                {
                    CusDataInfo insData = JsonConvert.DeserializeObject<CusDataInfo>(JsonConvert.SerializeObject(orderMasterModule.First()));
                    list.Add(insData);
                }
                else if (cusModel.cus_type == "02")
                {
                    foreach (var cusMaster in orderMasterModule)
                    {
                        if (orderMasterModule.IndexOf(cusMaster) < qty)
                        {
                            CusDataInfo insData = JsonConvert.DeserializeObject<CusDataInfo>(JsonConvert.SerializeObject(cusMaster));
                            list.Add(insData);
                        }
                    }
                }
                
                return list;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetCusModel Error message:{ex.Message},stacktrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public contactDataM GetContModel(ContactData contactModel, List<ModuleModel> orderMasterModule)
        {
            try
            {
                contactDataM rs = new contactDataM
                {
                    moduleType = "OMDL_CONTACT_DATA",
                    moduleData =JsonConvert.DeserializeObject<moduleData_contactData>(JsonConvert.SerializeObject(orderMasterModule))  
                };
                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetContModel Error message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public sendDataM GetSendModel(SendData sendModel, List<ModuleModel> orderMasterModule)
        {
            try
            {
                sendDataM rs = new sendDataM
                {
                    moduleType = "OMDL_SEND_DATA",
                    moduleData= JsonConvert.DeserializeObject<moduleData_sendData>(JsonConvert.SerializeObject(orderMasterModule))
                };
                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetSendModel Error message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public flightInfoDataM GetFlightInfoModel(FlightInfo flightModel, List<ModuleModel> orderMasterModule)
        {
            try
            {
                flightInfoDataM rs = new flightInfoDataM
                {
                    moduleType = "OMDL_FLIGHT_INFO",
                    moduleData = JsonConvert.DeserializeObject<moduleData_FlightInfo>(JsonConvert.SerializeObject(orderMasterModule))
                };
                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetFlightInfoModel Error message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public passengerDataM GetPassengerModel(CarPasgr pasgrModel, List<ModuleModel> orderMasterModule)
        {
            try
            {
                passengerDataM rs = new passengerDataM
                {
                    moduleType = "OMDL_PSGR_DATA",
                    moduleData = JsonConvert.DeserializeObject<moduleData_passenger>(JsonConvert.SerializeObject(orderMasterModule))
                };
                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetPassengerModel Error message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public otherDataM GetOtherModel(SimWifi simWifiModel, List<ModuleModel> orderMasterModule)
        {
            try
            {
                otherDataM rs = new otherDataM
                {
                    moduleType = "OMDL_OTHER_DATA",
                    moduleData = JsonConvert.DeserializeObject<moduleData_otherData>(JsonConvert.SerializeObject(orderMasterModule))
                };
                return rs;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetOtherModel Error message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public string GetMasterOrderInfo(string order_master_oid)
        {
            try
            {
                string deviceId = Guid.NewGuid().ToString();
                RequestJavaModel callbackData = new RequestJavaModel
                {
                    apiKey = Website.Instance.Configuration["KKdayAPI:Body:ApiKey"],
                    userOid = Website.Instance.Configuration["KKdayAPI:Body:UserOid"],
                    locale = "zh-tw",
                    ver = Website.Instance.Configuration["KKdayAPI:Body:Ver"],
                    ipaddress = "127.0.0.1",
                    json = new RequestJson
                    {
                        memberUuid = Website.Instance.Configuration["KKdayAPI:Body:MemberUuid"],
                        deviceId = deviceId,
                        tokenKey = MD5Tool.GetMD5(Website.Instance.Configuration["KKdayAPI:Body:MemberUuid"] + deviceId +
                                                          Website.Instance.Configuration["KKdayAPI:Body:Token"])
                    }

                };
                string url = $"{Website.Instance.Configuration["COMBO_SETTING:JAVA"]}api/v2/order/info/" + order_master_oid;
                string result = CommonProxy.Post(url, JsonConvert.SerializeObject(callbackData,new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore}));
                return result;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetMasterOrderInfo {order_master_oid} error ,ex:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
            
        }

        public BookingValidModel confirmBooking(ConfirmBookingValidModel rq)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Booking/SetPriceCartbookingValid";
                var result= CommonProxy.Post(url, JsonConvert.SerializeObject(rq));
                return JsonConvert.DeserializeObject<BookingValidModel>(result);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetconfirmBookingModel Error. Message={ex.Message},stackTrace={ex.StackTrace}");
                throw ex;

            }
        }

        public CartBookingRsModel CartBooking(List<CartBookingModel> rq,string parent_order_mid)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Booking/CartBookingAR/{parent_order_mid}";
                var result = CommonProxy.Post(url, JsonConvert.SerializeObject(rq));
                return JsonConvert.DeserializeObject<CartBookingRsModel>(result);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"CartBooking Error. Message={ex.Message},stackTrace={ex.StackTrace}");
                throw ex;

            }
        }

    }
}

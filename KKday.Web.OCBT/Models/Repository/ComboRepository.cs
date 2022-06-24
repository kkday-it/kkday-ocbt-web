using System;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Npgsql;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using KKday.Web.OCBT.Proxy;
using System.Collections.Generic;
using KKday.Web.OCBT.Models.Model.Product;
using Microsoft.Extensions.DependencyInjection;

namespace KKday.Web.OCBT.Models.Repository
{
    public class ComboBookingRepository
    {
        private readonly IServiceProvider _services;
        private readonly IRedisHelper _redis;
        private readonly SlackHelper _slack;
        public ComboBookingRepository(IServiceProvider services,IRedisHelper redis, SlackHelper slack)
        {
            _services = services;
            _redis = redis;
            _slack = slack;
        }
        public string ComboBooking()
        {
            return "";
        }
        #region 處理Booking_mst
        public string FilterMstData(BookingMstModel rq)
        {
            string filter = "";
            if (rq.booking_mst_xid != 0)
            {
                filter += $" AND mst.booking_mst_xid={rq.booking_mst_xid}";
            }
            if (!string.IsNullOrEmpty(rq.order_mid))
            {
                filter += $" AND mst.order_mid='{rq.order_mid}'";
            }
            if (!string.IsNullOrEmpty(rq.booking_mst_order_status))
            {
                filter += $" AND mst.booking_mst_order_status='{rq.booking_mst_order_status}'";
            }
            if (!string.IsNullOrEmpty(rq.booking_mst_voucher_status))
            {
                filter += $" AND mst.booking_mst_voucher_status='{rq.booking_mst_voucher_status}'";
            }

            return filter;
        }
        public BookingMstModel GetBookingMstData(BookingMstModel rq)
        {
            try
            {
                string sql = @"SELECT booking_mst_xid,order_mid,order_oid,prod_oid,go_date,
(case booking_model::text when '{}' then null else booking_model end) as booking_model ,
(case combo_model::text when '{}' then null else combo_model end)as combo_model ,booking_mst_order_status,
booking_mst_voucher_status,voucher_deadline,is_callback,is_back,is_need_back,monitor_start_datetime,create_user,create_datetime,modify_user,modify_datetime
FROM BOOKING_MST mst WHERE 1=1 ";
                sql += FilterMstData(rq);
                SqlMapper.AddTypeHandler(typeof(Dictionary<string, object>), new ObjectJsonMapper());
                SqlMapper.AddTypeHandler(typeof(Dictionary<string,object>), new ObjectJsonMapper());
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.Query<BookingMstModel>(sql).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetBookingMstData order_mid {rq.order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        public int InsertBookingMst(BookingMstModel rq)
        {
            try
            {
                string sql = @"INSERT INTO public.booking_mst(order_mid,order_oid,prod_oid,go_date,booking_model,booking_mst_order_status,booking_mst_voucher_status,
is_callback,is_back,create_user,create_datetime)
VALUES(:order_mid,:order_oid,:prod_oid,:go_date,:booking_model::jsonb,:booking_mst_order_status,:booking_mst_voucher_status,:is_callback,:is_back,:create_user,now())
RETURNING booking_mst_xid";
                SqlMapper.AddTypeHandler(typeof(BookingRequestModel), new ObjectJsonMapper());
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.QuerySingle<int>(sql, rq);
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"InsBookingMst order_mid {rq.order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }

        public int UpdateCallBack(bool is_callBack,string order_mid,string modify_user)
        {
            try
            {
                string sql = @"UPDATE booking_mst SET is_callback=:is_callBack,modify_user=:modify_user,modify_datetime=now() where order_mid=:order_mid";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.QuerySingle<int>(sql, new { order_mid, is_callBack, modify_user });
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateCallBack order_mid {order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        public int UpdateMstStatus(string order_status, string order_mid, string modify_user)
        {
            try
            {
                string sql = @"UPDATE booking_mst SET booking_mst_order_status=:order_status,modify_user=:modify_user,modify_datetime=now() where order_mid=:order_mid";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.Execute(sql, new { order_mid, order_status, modify_user });
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateCallBack order_mid {order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        public int UpdateMstComboModel(string order_mid,string modify_user,ComboReturnModel comboData,int voucher_max_time)
        {
            try
            {
                string sql = @"UPDATE booking_mst SET combo_model=:comboData::jsonb,voucher_deadline=:voucher_max_time,modify_user=:modify_user,modify_datetime=now() where order_mid=:order_mid";
                SqlMapper.AddTypeHandler(typeof(ComboReturnModel), new ObjectJsonMapper());
                
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.Execute(sql, new { order_mid,modify_user,comboData,voucher_max_time });
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateCallBack order_mid {order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        #endregion
        #region 處理Booking_dtl
        public string FilterDtlData(BookingDtlModel rq)
        {
            string filter = "";
            if (rq.booking_dtl_xid != 0)
            {
                filter += $" AND booking_dtl_xid={rq.booking_dtl_xid} ";
            }
            if (rq.booking_mst_xid != 0)
            {
                filter += $" AND booking_mst_xid={rq.booking_mst_xid} ";
            }
            if (rq.prod_oid != 0)
            {
                filter += $" AND prod_oid={rq.prod_oid}";
            }

            return filter;
        }
        public List<BookingDtlModel> GetBookingDtlData(BookingDtlModel rq)
        {
            try
            {
                string sql = @"SELECT booking_dtl_xid,booking_mst_xid,prod_oid,package_oid,item_oid,sku_oid,
real_booking_qty,booking_dtl_order_status,booking_dtl_voucher_status
FROM BOOKING_DTL dtl WHERE 1=1 ";


                sql += FilterDtlData(rq);
                SqlMapper.AddTypeHandler(typeof(SkuOid), new ObjectJsonMapper());
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.Query<BookingDtlModel>(sql).ToList();
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetBookingMstData order_mid {rq.order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        public List<int> InsertBookingDtl(List<BookingDtlModel> data)
        {
            try
            {
                string sql = @"INSERT INTO public.booking_dtl(booking_mst_xid,prod_oid,package_oid,item_oid,sku_oid,real_booking_qty,booking_dtl_order_status,
booking_dtl_voucher_status,order_master_oid,order_master_mid,create_datetime,create_user)
VALUES(@booking_mst_xid,@prod_oid,@package_oid,@item_oid,@sku_oid::jsonb,@real_booking_qty,@booking_dtl_order_status,
@booking_dtl_voucher_status,@order_master_oid,@order_master_mid,now(),@create_user);";

                SqlMapper.AddTypeHandler(typeof(SkuOid), new ObjectJsonMapper());
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    conn.Execute(sql, data);
                    var getDtl = GetBookingDtlData(new BookingDtlModel() { booking_mst_xid = data.First().booking_mst_xid });
                    return getDtl.Select(x => x.booking_dtl_xid).ToList();
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateOrderDtl error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        public int UpdateDtlStatus(BookingDtlModel data)
        {
            try
            {
                string sql = @"UPDATE BOOKING_DTL dtl SET booking_dtl_order_status=@booking_dtl_order_status,
order_master_oid=@order_master_oid,order_master_mid=@order_master_mid,order_mid=@order_mid,order_oid=@order_oid,modify_datetime=now()
where booking_dtl_xid=@booking_dtl_xid";
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.Execute(sql,data);
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateOrderDtl error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        #endregion
        public void CallBackJava(RequestJson jsonData,string order_mid="")
        {
            try
            {
                bool isCallBack = true;
                if (!string.IsNullOrEmpty(order_mid))
                {
                    BookingMstModel rq = new BookingMstModel
                    {
                        order_mid = order_mid
                    };
                    var bookingMstData = GetBookingMstData(rq);
                    isCallBack = bookingMstData != null ? bookingMstData.is_callback : true;
                }
                if (!isCallBack)//只有沒有is_callback過的才能打java
                {
                    RequestJavaModel callbackData = new RequestJavaModel
                    {
                        apiKey = Website.Instance.Configuration["KKdayAPI:Body:ApiKey"],
                        userOid = Website.Instance.Configuration["KKdayAPI:Body:UserOid"],
                        locale = "zh-tw",
                        ipaddress = GetLocalIPAddress(),
                        json = jsonData

                    };
                    string url = "";
                    string result = CommonProxy.Post(url, JsonConvert.SerializeObject(callbackData));
                    Website.Instance.logger.Info($"CallBackJava result message: {result}");

                    var rs = JObject.Parse(result);
                    if (rs["content"]["result"]?.ToString() != "0000")
                    {
                        //警示
                        _slack.SlackPost(Guid.NewGuid().ToString("N"), "CallBackJava", "ComboRepository/CallBackJava", $"order_mid:{order_mid},CallBackJava回覆失敗,請協助確認！", $"Result ={ result}");
                    }
                    UpdateCallBack(true, order_mid,"SYSTEM");
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"CallBackJava Error message:{ex.Message},stacktrace:{ex.StackTrace}");
                RequestJavaModel callbackData = new RequestJavaModel
                {
                    apiKey = Website.Instance.Configuration["KKdayAPI:Body:ApiKey"],
                    userOid = Website.Instance.Configuration["KKdayAPI:Body:UserOid"],
                    locale = "zh-tw",
                    ipaddress = GetLocalIPAddress(),
                    json = new RequestJson
                    {
                        metadata=new RequesteMetaModel
                        {
                            status="9999",
                            description="系統異常"
                        }
                    }

                };
                string url = "";
                string result = CommonProxy.Post(url, JsonConvert.SerializeObject(callbackData));

            }
        }
        public void ComboBookingFlow(string queue)
        {
            try
            {
                Website.Instance.logger.Info($"ComboBookingFlow queue={queue}");
                if (string.IsNullOrEmpty(queue))
                {
                    throw new Exception("ComboBookingFlow error");
                }
                //將queue轉成dataModel
                var queueModel = JsonConvert.DeserializeObject<BookingRequestModel>(queue);
                #region 查找母單資訊
                var getMstModel = GetBookingMstData(new BookingMstModel
                {
                    order_mid = queueModel.order.orderMid
                });
                #endregion
                #region 判斷母單資訊
                if (getMstModel != null && getMstModel?.booking_mst_order_status == "GL" && getMstModel?.booking_mst_voucher_status == "GL")//確認不為null與狀態都為GL
                {
                    RequestJson jsonData = new RequestJson
                    {
                        orderMid = getMstModel?.order_mid,
                        metadata = new RequesteMetaModel
                        {
                            status = "2008",
                            description = "母子訂單已曾順利完成作業"
                        }
                    };
                    CallBackJava(jsonData, queueModel.order.orderMid);
                    return;//跳出
                }
                else if (getMstModel != null)//母訂單模組不為空值，取DTL值
                {
                    var getDtlModel = GetBookingDtlData(new BookingDtlModel
                    {
                        booking_mst_xid = getMstModel.booking_mst_xid
                    });
                    if (getDtlModel?.Count > 0 && getMstModel.booking_mst_voucher_status != "GL")
                    {
                        #region call JAVA API 確認子訂單內容是否一致
                        #endregion
                    }
                }
                else if (getMstModel == null)//沒有母單資料 ins進母單
                {
                    getMstModel = new BookingMstModel
                    {
                        order_mid = queueModel.order?.orderMid,
                        order_oid = queueModel.order.orderOid,
                        prod_oid = (int)queueModel.order.prodOid,
                        go_date = Convert.ToDateTime(queueModel.order?.begLstGoDt).ToString("yyyyMMdd"),
                        booking_model=JsonConvert.DeserializeObject<Dictionary<string,object>>(JsonConvert.SerializeObject(queueModel)),
                        booking_mst_order_status = "NW",
                        booking_mst_voucher_status = "NW",
                        create_user = "SYSTEM",
                        is_callback = false,
                        is_back = false
                    };
                    getMstModel.booking_mst_xid = InsertBookingMst(getMstModel);//將插入的mst_xid回傳給mstModel

                }
                #region 取得product的comboBooking
                ComboCartItemsModel cartItem = new ComboCartItemsModel
                {
                    prod_oid = queueModel?.order?.prodOid?.ToString(),
                    pkg_oid = queueModel?.order?.packageOid.ToString(),
                    go_date = queueModel?.order?.begLstGoDt,
                    back_date = queueModel?.order?.endLstBackDt,
                    event_time = queueModel?.order?.eventTime,
                    skus = new List<ComboSkusModel>()
                };

                foreach (var sku in queueModel?.order?.lstPrice)
                {
                    cartItem.skus.Add(new ComboSkusModel
                    {
                        sku_oid = sku.skuOid,
                        qty = sku.qty
                    });
                }

                ComboRequestModel CartItemModelrs = new ComboRequestModel
                {
                    cart_items = new List<ComboCartItemsModel>()

                };
                CartItemModelrs.cart_items.Add(cartItem);
                var ComboData = GetComboProd(CartItemModelrs);//取得ComboData的資料
                UpdateMstComboModel(queueModel.order?.orderMid, "SYSTEM", ComboData, ComboData.voucher_max_time);//將comboBooking等資訊押入至資料庫
                #endregion
                if (ComboData.meta.status != "100000")//取得combo產品失敗
                {
                    Website.Instance.logger.Info($"ComboBookingFlow GetComboProd error. request={JsonConvert.SerializeObject(CartItemModelrs)},return={JsonConvert.SerializeObject(ComboData)}");
                    CallBackJava(new RequestJson
                    {
                        metadata = new RequesteMetaModel
                        {
                            status = "2003",
                            description = "OCBT查詢子商品對應明細失敗"
                        }
                    }, queueModel.order.orderMid);
                    throw new Exception("ComboBookingFlow GetComboProd error.");
                }
                var FAData = GetReceiveMaster(queueModel.order.orderOid);
                if (FAData.metadata.status != "AD00")
                {
                    Website.Instance.logger.Info($"ComboBookingFlow GetReceiveMaster error. orderOid={queueModel.order.orderOid},return={JsonConvert.SerializeObject(FAData)}");
                    CallBackJava(new RequestJson
                    {
                        metadata = new RequesteMetaModel
                        {
                            status = "2010",
                            description = "取得母訂單的分帳公司失敗，無法訂購"
                        }
                    }, queueModel.order.orderMid);
                    throw new Exception("ComboBookingFlow GetReceiveMaster error.");

                }
                List<KKday.Web.OCBT.Models.Model.CartBooking.CartBookingModel> cartBooking = new List<KKday.Web.OCBT.Models.Model.CartBooking.CartBookingModel>();
                KKday.Web.OCBT.Models.Model.CartBooking.ConfirmBookingValidModel cartBookingValid = new Model.CartBooking.ConfirmBookingValidModel()
                {
                    cartorder = new List<Model.CartBooking.ConfirmProdInfoModel>()
                };
                var _bookingRepos = _services.GetService<BookingRepository>();
                List<BookingDtlModel> insertDtlData = new List<BookingDtlModel>();
                foreach (var skuOid in queueModel.order.lstPrice)//母單中的sku
                {
                    foreach (var prodDtl in ComboData.data)//取得ComboData.Data中的資訊
                    {
                        foreach (var prod in prodDtl.combo_info.skus.Where(x => x.sku_oid == skuOid.skuOid).Select(y => y.combo_prod).First())
                        {
                            var ProdModel = QueryProduct(prod.prod_oid,
                        FAData.data.FirstOrDefault().fa_vouch_currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品資訊與產品版本
                            var PkgModel = QueryPackage(prod.prod_oid, prod.pkg_oid,
                        FAData.data.FirstOrDefault().fa_vouch_currency, queueModel.order.memberLang, queueModel.order.contactCountryCd, queueModel.order.begLstGoDt, queueModel.order.endLstBackDt);
                            if (PkgModel.result != "0000")
                            {
                                Website.Instance.logger.Info($"ComboBookingFlow QueryPackage error response={JsonConvert.SerializeObject(PkgModel)}");
                                CallBackJava(new RequestJson
                                {
                                    metadata = new RequesteMetaModel
                                    {
                                        status = "2004",
                                        description = "OCBT查詢子商品失敗 （下架或其他原因）"
                                    }
                                }, queueModel.order.orderMid);
                                throw new Exception("ComboBookingFlow GetReceiveMaster error.");
                            }
                            else
                            {

                                var ProdModuleModel = QueryModule(prod.prod_oid,
                                FAData.data.FirstOrDefault().currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品模組
                                var insertdata = new BookingDtlModel()
                                {
                                    booking_mst_xid = getMstModel.booking_mst_xid,
                                    prod_oid = Convert.ToInt32(prod.prod_oid),
                                    package_oid = Convert.ToInt32(prod.pkg_oid),
                                    item_oid = Convert.ToInt32(prod.item_oid),
                                    sku_oid = new SkuOid() { sku_oids = prod.skus.Select(x => x.sku_oid).ToArray() },
                                    booking_qty = skuOid.qty,//母單sku下的數量
                                    real_booking_qty = 0,//在外層統計的時候再計算實際數量
                                    booking_dtl_order_status = "NW",
                                    booking_dtl_voucher_status = "NW",
                                    order_master_mid = "",
                                    order_master_oid = 0

                                };//插入DtlData
                                var bookingModel = _bookingRepos.SetBookingModel(ProdModuleModel, queueModel.order);//取得訂購的模組
                                #region 補上訂購所有資訊
                                bookingModel.productOid = prod.prod_oid;
                                bookingModel.packageOid = prod.pkg_oid;
                                bookingModel.itemOid = prod.item_oid;
                                bookingModel.prodVersion = ProdModel.version;
                                bookingModel.channelOid = Website.Instance.Configuration["WMS_API:CompanyData:ChannelOid"];
                                bookingModel.ipaddress = GetLocalIPAddress();
                                bookingModel.currency = FAData.data.First().fa_vouch_currency;
                                bookingModel.skus = new List<Model.CartBooking.SkuModel>();
                                bookingModel.checkoutDate= DateTime.Now.AddMonths(1).ToString("yyyy-MM-25 HH:mm:ss");
                                #endregion
                                //補上bookingInfo
                                bookingModel.bookingInfo = new Model.CartBooking.bookingInfoModel
                                {
                                    time_zone = ProdModel.timezone,
                                    prod_name = ProdModel.prod_name,
                                    pkg_name = PkgModel.pkg_name,
                                    prod_oid = Convert.ToInt64(prod.prod_oid),
                                    pkg_oid = Convert.ToInt64(prod.pkg_oid),
                                    skus = new List<Model.CartBooking.BookingInfoConfirmSku>(),
                                    is_open_date = (ProdModel.go_date_setting.type == "03" || ProdModel.go_date_setting.type == "04") ? true : false,
                                    pay_type = "arType",
                                    kk_company_no= FAData.data.First().company_no.ToString()
                                };
                                if (ProdModel.go_date_setting.type == "03" || ProdModel.go_date_setting.type == "04")
                                {
                                    bookingModel.lstGoDt = null;
                                    bookingModel.lstBackDt = null;
                                }
                                else
                                {
                                    bookingModel.bookingInfo.go_date = bookingModel.lstGoDt;
                                    bookingModel.bookingInfo.back_date = bookingModel.lstBackDt;    
                                }
                                
                                Model.CartBooking.ConfirmProdInfoModel confirmOrder = new Model.CartBooking.ConfirmProdInfoModel()
                                {
                                    prod_version = ProdModel.version,
                                    time_zone = ProdModel.timezone,
                                    has_event = (bool?)PkgModel.item.First().has_event,
                                    event_time=queueModel.order.eventTime,
                                    begin_date = queueModel.order.begLstGoDt,
                                    end_date = queueModel.order.endLstBackDt,
                                    currency = FAData.data.FirstOrDefault().fa_vouch_currency,
                                    ori_guidkey = PkgModel.guid,//價錢的快取
                                    price_token = Guid.NewGuid().ToString(),//存取至給Java驗證的Price
                                    prod_oid = prod.prod_oid,
                                    packege_oid = prod.pkg_oid,
                                    item_oid = prod.item_oid,
                                    skus = new List<Model.CartBooking.ConfirmSku>(),
                                    locale = queueModel.order.memberLang,
                                    go_date_type = ProdModel.go_date_setting.type,
                                    adjprice_flag=true,
                                    adjprice_type="01"
                                };
                                bookingModel.priceToken = confirmOrder.price_token;//將訂單的token逐筆給予
                                if (confirmOrder?.has_event == false)
                                {
                                    confirmOrder.event_time = "";
                                    bookingModel.eventTime = "";
                                }
                                else//有場次lstBackDt需要為空
                                {
                                    bookingModel.lstBackDt = null;
                                }
                                
                                double tempTotalPrice = 0;
                                int? dtlTotalQty = 0;
                                foreach (var skusData in prod.skus)//將對應到的sku放進一筆訂單中的skus
                                {
                                    confirmOrder.skus.Add(new Model.CartBooking.ConfirmSku
                                    {
                                        price = 1,
                                        qty = skusData.qty,
                                        sku_id = skusData.sku_oid
                                    });
                                    bookingModel.skus.Add(new Model.CartBooking.SkuModel
                                    {
                                        sku_oid=skusData.sku_oid,
                                        qty=(int)skusData.qty
                                    });

                                    tempTotalPrice += (double)skusData.qty * 1;
                                    dtlTotalQty += skusData.qty;
                                }
                                insertdata.real_booking_qty = (int)dtlTotalQty;
                                insertDtlData.Add(insertdata);
                                confirmOrder.total_price = tempTotalPrice;
                                cartBookingValid.cartorder.Add(confirmOrder);
                                bookingModel.currPriceTotal = tempTotalPrice;
                                cartBooking.Add(bookingModel);
                            }
                        }
                    }
                }
                var dtlcount = InsertBookingDtl(insertDtlData);
                cartBookingValid.adjprice_flag = true;
                cartBookingValid.adjprice_type = "01";//允許任意金額
                cartBookingValid.token = Guid.NewGuid().ToString();//母單的金額
                cartBooking.ForEach(x => x.masterPriceToken = cartBookingValid.token);
                var bookingConfirm = _bookingRepos.confirmBooking(cartBookingValid);
                if (bookingConfirm.result != "0000")
                {
                    Website.Instance.logger.Info($"ComboBookingFlow confirmBooking error. request={JsonConvert.SerializeObject(cartBookingValid)},response={JsonConvert.SerializeObject(bookingConfirm)}");
                    CallBackJava(new RequestJson
                    {
                        metadata = new RequesteMetaModel
                        {
                            status = "2002",
                            description = "OCBT成立訂單失敗"
                        }
                    }, queueModel.order.orderMid);
                    throw new Exception("ComboBookingFlow CartBooking error.");
                }

                var cartbookingRs = _bookingRepos.CartBooking(cartBooking);
                if (cartbookingRs.result != "0000")
                {
                    UpdateMstStatus("BOOKING_FAIL", queueModel.order.orderMid, "SYSTEM");
                    var bookingDtl = GetBookingDtlData(new BookingDtlModel { booking_mst_xid = getMstModel.booking_mst_xid });
                    bookingDtl.ForEach(x =>
                    {
                        x.booking_dtl_order_status = "BOOKING_FAIL";
                        x.modify_user = "SYSTEM";
                        UpdateDtlStatus(x);
                    });
                    Website.Instance.logger.Info($"ComboBookingFlow CartBooking error.request={JsonConvert.SerializeObject(cartBooking)}, response={JsonConvert.SerializeObject(cartbookingRs)}");
                    CallBackJava(new RequestJson
                    {
                        metadata = new RequesteMetaModel
                        {
                            status = "2002",
                            description = "OCBT成立訂單失敗"
                        }
                    }, queueModel.order.orderMid);
                    throw new Exception("ComboBookingFlow CartBooking error.");
                }
                //更新主訂單的狀態
                UpdateMstStatus("GL", queueModel.order.orderMid, "SYSTEM");
                var bookingDtlList = GetBookingDtlData(new BookingDtlModel { booking_mst_xid = getMstModel.booking_mst_xid });
                int countwithorder = 0;
                foreach (var bookingrq in cartBooking)
                {
                    var DtlList = bookingDtlList.Where(x => x.sku_oid.sku_oids.Except(bookingrq.skus.Select(x => x.sku_oid)).Count()==0).First();//判斷Sku_oid[]中是不是完全一樣
                    DtlList.order_master_mid = cartbookingRs.orde_master_mid;
                    DtlList.order_mid = cartbookingRs.orders[countwithorder].order_mid;
                    DtlList.order_oid = Convert.ToInt32(cartbookingRs.orders[countwithorder].order_oid);
                    DtlList.booking_dtl_order_status = "GL";
                    UpdateDtlStatus(DtlList);
                }
                var pushData = new
                {
                    master_order_mid = queueModel.order.orderMid
                };
                _redis.Push("ComboBookingVoucher", JsonConvert.SerializeObject(pushData));//將Java資料傳入redisQueue
                #endregion

            }
            catch (Exception ex)//跳出錯誤
            {
                Website.Instance.logger.Info($"ComboBookingFlow error {queue} error:{ex.Message},{ex.StackTrace}");
                CallBackJava(new RequestJson
                {
                    metadata = new RequesteMetaModel
                    {
                        status = "9999",
                        description = "系統異常"
                    }
                });
            }
            #region 查找母單

            #endregion
        }

        public WMSProductModel QueryProduct(string prod_oid,string currency,string locale,string state)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Product/QueryProd";
                QueryWMSProductModel queryProduct = new QueryWMSProductModel
                {
                    prod_no=prod_oid,
                    locale=locale,
                    state=state,
                    currency=currency,
                    block=new List<string> { "PRODUCT", "PACKAGE", "PRODUCT-MARKETING" },
                    begin_date=DateTime.Now.ToString("yyyy-MM-dd"),
                    end_date=DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd"),
                    account_xid=Website.Instance.Configuration["WMS_API:CompanyData:AccountXid"]

                };
                var ProductData = CommonProxy.Post(url, JsonConvert.SerializeObject(queryProduct));
                return JsonConvert.DeserializeObject<WMSProductModel>(ProductData);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"QueryProduct Error message:{ex.Message},stacktrace:{ex.StackTrace}");
                throw ex;
            }
        }
        public WMSPackageModel QueryPackage(string prod_oid, string pkg_oid, string currency, string locale, string state, string s_date, string e_date)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Product/QueryItemsPrice";
                QueryWMSProductModel queryProduct = new QueryWMSProductModel
                {
                    prod_no = prod_oid,
                    pkg_no = pkg_oid,
                    locale = locale,
                    state = state,
                    currency = currency,
                    block = new List<string> { "PRODUCT", "PACKAGE", "PRODUCT-MARKETING" },
                    begin_date = s_date,
                    end_date = e_date,
                    account_xid = Website.Instance.Configuration["WMS_API:CompanyData:AccountXid"],
                    company_xid = Website.Instance.Configuration["WMS_API:CompanyData:CompanyXid"]

                };
                var ProductData = CommonProxy.Post(url, JsonConvert.SerializeObject(queryProduct));
                return JsonConvert.DeserializeObject<WMSPackageModel>(ProductData);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"QueryPackage Error message:{ex.Message},stacktrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public WMSProductModuleModel QueryModule(string prod_oid, string currency, string locale, string state)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Product/QueryModule";
                QueryWMSProductModel queryProduct = new QueryWMSProductModel
                {
                    prod_no = prod_oid,
                    locale = locale,
                    currency = currency,
                    state=state

                };
                var ProductData = CommonProxy.Post(url, JsonConvert.SerializeObject(queryProduct));
                return JsonConvert.DeserializeObject<WMSProductModuleModel>(ProductData);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"QueryModule Error message:{ex.Message},stacktrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public ComboReturnModel GetComboProd(ComboRequestModel rs)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["COMBO_SETTING:Prod"]}/GetComboInfo";
                var responseMsg = CommonProxy.Post(url, JsonConvert.SerializeObject(rs));
                ComboReturnModel response = new ComboReturnModel
                {
                    meta = new metaDataProdModel
                    {
                        status = "100000"
                    },
                    data = new List<ComboDataModel>(),
                    voucher_max_time=10
                };
                response.data.Add(new ComboDataModel
                {
                    combo_info = new ComboInfoModel()
                    {
                        prod_oid = "112381",
                        pkg_oid = "301357",
                        item_oid = "46684",
                        voucher_max_time = 10,
                        skus = new List<ComboInfoSkusModel>()
                        {
                            new ComboInfoSkusModel()
                            {
                                sku_oid="0a1ddc70318f48bb8ee93bdc94d10efb",
                                qty=1,
                                combo_prod=new List<ComboProdModel>()
                                {
                                    new ComboProdModel()
                                    {
                                        prod_oid="112404",
                                        pkg_oid="334714",
                                        item_oid="80491",
                                        skus=new List<ComboSkusModel>()
                                        {
                                            new ComboSkusModel
                                            {
                                                sku_oid="851f03fac941e008dcee324862ec28e0",
                                                qty=1
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });

                //return JsonConvert.DeserializeObject<ComboReturnModel>(responseMsg);
                return response;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetComboProd error. message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }
        }

        public ReceiveListModel GetReceiveMaster(int order_master_oid)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["COMBO_SETTING:FA"]}api/v2/receive-master/list/by-order-master/{order_master_oid}";
                var responseMsg = CommonProxy.Get(Guid.NewGuid().ToString(), url);
                return JsonConvert.DeserializeObject<ReceiveListModel>(responseMsg);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetReceiveMaster error.Message:{ex.Message},stackTrace:{ex.StackTrace}");
                throw ex;
            }

        }



        public string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
            catch (Exception ex)
            {
                return "127.0.0.1";
            }
        }

    }
}

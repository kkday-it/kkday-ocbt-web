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
                filter += $" AND mst.order_mid={rq.order_mid}";
            }
            if (!string.IsNullOrEmpty(rq.booking_mst_order_status))
            {
                filter += $" AND mst.booking_mst_order_status={rq.booking_mst_order_status}";
            }
            if (!string.IsNullOrEmpty(rq.booking_mst_voucher_status))
            {
                filter += $" AND mst.booking_mst_voucher_status={rq.booking_mst_voucher_status}";
            }

            return filter;
        }
        public BookingMstModel GetBookingMstData(BookingMstModel rq)
        {
            try
            {
                string sql = @"SELECT * FROM BOOKING_MST mst WHERE 1=1 ";
                sql += FilterMstData(rq);
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
is_callback,is_back,create_user,create_date)
VALUES(:order_mid,:order_oid,:prod_oid,:booking_model,:booking_mst_order_status,:booking_mst_voucher_status,:is_callback,:is_back,:create_user,now())
RETURNING booking_mst_xid";

                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.QuerySingle<int>(sql,rq);
                }

            }
            catch(Exception ex)
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
                    return conn.QuerySingle<int>(sql, new { order_mid, order_status, modify_user });
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
                string sql = @"SELECT * FROM BOOKING_DTL dtl WHERE 1=1 ";
                sql += FilterDtlData(rq);
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
                string sql = @"INSERT INTO public.BOOKING_DTL(booking_mst_xid,prod_oid,package_oid,item_oid,sku_oid,booking_qty,real_booking_qty,booking_dtl_order_status,
booking_dtl_voucher_status,order_master_oid,order_master_mid,create_datetime,create_user)
VALUES(@booking_mst_xid,@prod_oid,@package_oid,@item_oid,@sku_oid,@booking_qty,@real_booking_qty,@booking_dtl_order_status,
@booking_dtl_voucher_status,@order_master_oid,@order_master_mid,now(),@create_user);
RETURNING booking_dtl_xid";


                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.Query<int>(sql).ToList();
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
                string sql = @"UPDATE BOOKING_DTL dtl SET booking_dtl_order_satus=@booking_dtl_order_status,
order_master_oid=@order_master_oid,order_master_mid=@order_master_mid,order_mid=@order_mid,order_oid=@order_oid,modify_datetime=now()
where booking_dtl_xid=@booking_dtl_xid";
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.QuerySingle<int>(sql,data);
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"UpdateOrderDtl error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        #endregion
        public void CallBackJava(ResponseJson jsonData,string order_mid="")
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
                    ResponseJavaModel callbackData = new ResponseJavaModel
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
                ResponseJavaModel callbackData = new ResponseJavaModel
                {
                    apiKey = Website.Instance.Configuration["KKAPI_INPUT:API_KEY"],
                    userOid = Website.Instance.Configuration["KKAPI_INPUT:USER_OID"],
                    locale = "zh-tw",
                    ipaddress = GetLocalIPAddress(),
                    json = new ResponseJson
                    {
                        metadata=new ResponseMetaModel
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
                    ResponseJson jsonData = new ResponseJson
                    {
                        orderMid = getMstModel?.order_mid,
                        metadata = new ResponseMetaModel
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
                        go_date = queueModel.order?.begLstGoDt,
                        //booking_model
                        //combo_model
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
                    skus= new List<ComboSkusModel>()
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
                
                #endregion
                if (ComboData.meta.status != "100000")//取得combo產品失敗
                {
                    Website.Instance.logger.Info($"ComboBookingFlow GetComboProd error. request={JsonConvert.SerializeObject(CartItemModelrs)},return={JsonConvert.SerializeObject(ComboData)}");
                    CallBackJava(new ResponseJson
                    {
                        metadata = new ResponseMetaModel
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
                    CallBackJava(new ResponseJson
                    {
                        metadata = new ResponseMetaModel
                        {
                            status = "2010",
                            description = "取得母訂單的分帳公司失敗，無法訂購"
                        }
                    },queueModel.order.orderMid);
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
                        foreach (var prod in prodDtl.combo_info.skus.Where(x => x.sku_oid ==skuOid.skuOid).Select(y=>y.combo_prod).First())
                        {
                            var ProdModel = QueryProduct(prod.prod_oid,
                        FAData.data.fa_vouch_currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品資訊與產品版本
                            var PkgModel = QueryPackage(prod.prod_oid,prod.pkg_oid,
                        FAData.data.currency, queueModel.order.memberLang, queueModel.order.contactCountryCd, queueModel.order.begLstGoDt, queueModel.order.endLstBackDt);
                            if (PkgModel.result != "0000")
                            {
                                Website.Instance.logger.Info($"ComboBookingFlow QueryPackage error response={JsonConvert.SerializeObject(PkgModel)}");
                                CallBackJava(new ResponseJson
                                {
                                    metadata = new ResponseMetaModel
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
                                FAData.data.currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品模組
                                var insertdata= new BookingDtlModel()
                                {
                                    booking_mst_xid = getMstModel.booking_mst_xid,
                                    prod_oid = Convert.ToInt32(prod.prod_oid),
                                    package_oid = Convert.ToInt32(prod.pkg_oid),
                                    item_oid = Convert.ToInt32(prod.item_oid),
                                    sku_oid = new SkuOid() { sku_oid = prod.skus.Select(x => x.sku_oid).ToArray() },
                                    booking_qty = skuOid.qty,//母單sku下的數量
                                    real_booking_qty=0,//在外層統計的時候再計算實際數量
                                    booking_dtl_order_status="NW",
                                    booking_dtl_voucher_status="NW",
                                    order_master_mid="",
                                    order_master_oid=0

                                };//插入DtlData
                                var bookingModel = _bookingRepos.SetBookingModel(ProdModuleModel, queueModel.order);//取得訂購的模組
                                cartBooking.Add(bookingModel);
                                Model.CartBooking.ConfirmProdInfoModel confirmOrder = new Model.CartBooking.ConfirmProdInfoModel()
                                {
                                    prod_version = ProdModel.version,
                                    time_zone = ProdModel.timezone,
                                    has_event = (bool?)PkgModel.item.First().has_event,
                                    begin_date = queueModel.order.begLstGoDt,
                                    end_date = queueModel.order.endLstBackDt,
                                    currency = FAData.data.currency,
                                    ori_guidkey = PkgModel.guid,//價錢的快取
                                    price_token = Guid.NewGuid().ToString(),//存取至給Java驗證的Price
                                    prod_oid = prod.prod_oid,
                                    packege_oid = prod.pkg_oid,
                                    item_oid = prod.item_oid,
                                    skus = new List<Model.CartBooking.ConfirmSku>(),
                                    locale = queueModel.order.memberLang,
                                    //go_date_type 需要補
                                };
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
                                    tempTotalPrice += (double)skusData.qty * 1;
                                    dtlTotalQty += skusData.qty;
                                }
                                insertdata.real_booking_qty = (int)dtlTotalQty;
                                insertDtlData.Add(insertdata);
                                confirmOrder.total_price = tempTotalPrice;
                                cartBookingValid.cartorder.Add(confirmOrder);

                            }
                        }
                    }
                }


                //foreach (var prod in ComboData.data)
                //{
                //    var ProdModel = QueryProduct(prod.combo_info.prod_oid,
                //        FAData.data.fa_vouch_currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品資訊與產品版本

                //    var PkgModel = QueryPackage(prod.combo_info.prod_oid,prod.combo_info.pkg_oid,
                //        FAData.data.currency, queueModel.order.memberLang, queueModel.order.contactCountryCd, queueModel.order.begLstGoDt, queueModel.order.endLstBackDt);
                //    if (PkgModel.result != "0000")
                //    {
                //        Website.Instance.logger.Info($"ComboBookingFlow QueryPackage error response={JsonConvert.SerializeObject(PkgModel)}");
                //        CallBackJava(new ResponseJson
                //        {
                //            metadata = new ResponseMetaModel
                //            {
                //                status = "2004",
                //                description = "OCBT查詢子商品失敗 （下架或其他原因）"
                //            }
                //        }, queueModel.order.orderMid);
                //        throw new Exception("ComboBookingFlow GetReceiveMaster error.");
                //    }
                //    else
                //    {

                //        var ProdModuleModel = QueryModule(prod.combo_info.prod_oid,
                //        FAData.data.currency, queueModel.order.memberLang, queueModel.order.contactCountryCd);//取得產品模組
                //        insertDtlData.Add(new BookingDtlModel
                //        {
                //            booking_mst_xid= getMstModel.booking_mst_xid,
                //            prod_oid=Convert.ToInt32(prod.combo_info.prod_oid),
                //            package_oid=Convert.ToInt32(prod.combo_info.pkg_oid),
                //            item_oid=Convert.ToInt32(prod.combo_info.item_oid),
                //            sku_oid=new SkuOid() { sku_oid=prod.combo_info.skus.Select(x=>x.sku_oid).ToArray()}

                //        });
                //        var bookingModel = _bookingRepos.SetBookingModel(ProdModuleModel, queueModel.order);//取得訂購的模組
                //        cartBooking.Add(bookingModel);
                //        Model.CartBooking.ConfirmProdInfoModel confirmOrder = new Model.CartBooking.ConfirmProdInfoModel()
                //        {
                //            prod_version=ProdModel.version,
                //            time_zone=ProdModel.timezone,
                //            has_event=(bool?)PkgModel.item.First().has_event,
                //            begin_date=queueModel.order.begLstGoDt,
                //            end_date=queueModel.order.endLstBackDt,
                //            currency=FAData.data.currency,
                //            ori_guidkey=PkgModel.guid,//價錢的快取
                //            price_token=Guid.NewGuid().ToString(),//存取至給Java驗證的Price
                //            prod_oid= prod.combo_info.prod_oid,
                //            packege_oid=prod.combo_info.pkg_oid,
                //            item_oid=prod.combo_info.item_oid,
                //            skus=new List<Model.CartBooking.ConfirmSku>(),
                //            locale=queueModel.order.memberLang,
                //            //go_date_type 需要補
                //        };
                //        double tempTotalPrice = 0;
                //        foreach (var skusData in prod.combo_info.skus)//將對應到的sku放進一筆訂單中的skus
                //        {
                //            confirmOrder.skus.Add(new Model.CartBooking.ConfirmSku {
                //                price=1,
                //                qty=skusData.qty,
                //                sku_id=skusData.sku_oid
                //            });
                //            tempTotalPrice += (double)skusData.qty * 1;

                //        }
                //        confirmOrder.total_price = tempTotalPrice;
                //        cartBookingValid.cartorder.Add(confirmOrder);

                //    }

                //}
                //先將明細插入至db，如果數量大於十，代表無法做購物車
                var dtlcount = InsertBookingDtl(insertDtlData);
                cartBookingValid.adjprice_flag = true;
                cartBookingValid.adjprice_type="01";//允許任意金額
                cartBookingValid.token = Guid.NewGuid().ToString();//母單的金額
                var bookingConfirm = _bookingRepos.confirmBooking(cartBookingValid);
                if (bookingConfirm.result != "0000")
                {
                    Website.Instance.logger.Info($"ComboBookingFlow confirmBooking error. request={JsonConvert.SerializeObject(cartBookingValid)},response={JsonConvert.SerializeObject(bookingConfirm)}");
                    CallBackJava(new ResponseJson
                    {
                        metadata = new ResponseMetaModel
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
                    CallBackJava(new ResponseJson
                    {
                        metadata = new ResponseMetaModel
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
                    var DtlList = bookingDtlList.Where(x => x.sku_oid.sku_oid == bookingrq.bookingInfo.skus.Select(x => x.sku_id).ToArray()).First();
                    DtlList.order_master_mid = cartbookingRs.orde_master_mid;
                    DtlList.order_mid = cartbookingRs.orders[countwithorder].order_mid;
                    DtlList.order_oid= Convert.ToInt32(cartbookingRs.orders[countwithorder].order_oid);
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
                CallBackJava(new ResponseJson
                {
                    metadata = new ResponseMetaModel
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
        public WMSPackageModel QueryPackage(string prod_oid,string pkg_oid,string currency, string locale, string state,string s_date,string e_date)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["WMS_API:URL"]}v2/Product/QueryProd";
                QueryWMSProductModel queryProduct = new QueryWMSProductModel
                {
                    prod_no = prod_oid,
                    pkg_no=pkg_oid,
                    locale = locale,
                    state=state,
                    currency = currency,
                    block = new List<string> { "PRODUCT", "PACKAGE", "PRODUCT-MARKETING" },
                    begin_date = s_date,
                    end_date = e_date,
                    account_xid = Website.Instance.Configuration["WMS_API:CompanyData:AccountXid"]

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
                var responseMsg = CommonProxy.Post(url,JsonConvert.SerializeObject(rs));
                return JsonConvert.DeserializeObject<ComboReturnModel>(responseMsg);
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
                string url = $"{Website.Instance.Configuration[""]}api/v2/receive-master/list/by-order-master/{order_master_oid}";
                var responseMsg = CommonProxy.Get(Guid.NewGuid().ToString(),url);
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

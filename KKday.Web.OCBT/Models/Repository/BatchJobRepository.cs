using System;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using KKday.Web.OCBT.Models.Model.Order;
using KKday.Web.OCBT.Models.Repository;
using KKday.Web.OCBT.Proxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KKday.Web.OCBT.Models.Repository
{
    public class BatchJobRepository
    {

        private readonly ComboBookingRepository _comboBookingRepos;
        private readonly OrderProxy _orderProxy;
        private readonly SlackHelper _slack;
        public BatchJobRepository(ComboBookingRepository comboBookingRepos, OrderProxy orderProxy,SlackHelper slack)
        {
            _comboBookingRepos = comboBookingRepos;
            _orderProxy = orderProxy;
            _slack = slack;
        }


        public void SetParentBack(string guidKey)
        {
            //1.查詢所有的母單  (1).只要過出發日 (2)不管有沒有正常完成  ，都要由OCBT檢查
            try
            {
                List<ParentOrderModel> orderLst = new  List<ParentOrderModel>();
                SqlMapper.AddTypeHandler(typeof(List<ChildOrderModel>), new ObjectJsonMapper());

                //取comboSupList
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    //SQL含open date
                    string sqlStmt = @"select a.booking_mst_xid,a.order_mid,a.go_date,a.booking_mst_order_status,a.booking_mst_voucher_status,a.order_oid,a.is_back,a.is_need_back,
jsonb_agg(json_build_object('booking_dtl_xid',b.booking_dtl_xid,'order_mid',b.order_mid,'order_oid',b.order_oid,'booking_dtl_order_status',b.booking_dtl_order_status,'booking_dtl_voucher_status',b.booking_dtl_voucher_status )) as child_order,
'N' as is_open_date
from booking_mst a
left join booking_dtl b on a.booking_mst_xid =b.booking_mst_xid
where a.is_back=false and a.is_need_back=true
and to_date(a.go_date,'yyyyMMdd') <now() and COALESCE(a.go_date,'')!=''
group by a.booking_mst_xid,a.order_mid,a.go_date,a.booking_mst_order_status,a.booking_mst_voucher_status,a.order_oid,a.is_back,a.is_need_back
union all 
select a.booking_mst_xid,a.order_mid,a.go_date,a.booking_mst_order_status,a.booking_mst_voucher_status,a.order_oid,a.is_back,a.is_need_back,
jsonb_agg(json_build_object('booking_dtl_xid',b.booking_dtl_xid,'order_mid',b.order_mid,'order_oid',b.order_oid,'booking_dtl_order_status',b.booking_dtl_order_status,'booking_dtl_voucher_status',b.booking_dtl_voucher_status )) as child_order,
'Y' as is_open_date
from booking_mst a
left join booking_dtl b on a.booking_mst_xid =b.booking_mst_xid
where a.is_back=false and a.is_need_back=true
and COALESCE(a.go_date,'')=''
group by a.booking_mst_xid,a.order_mid,a.go_date,a.booking_mst_order_status,a.booking_mst_voucher_status,a.order_oid,a.is_back,a.is_need_back";

                    orderLst = conn.Query<ParentOrderModel>(sqlStmt).ToList();
                }

                foreach (ParentOrderModel parent in orderLst)
                {
                    relastionMappingResModel mapping = _comboBookingRepos.GetMappingOrderList(parent.order_mid);
                    if (mapping.content.result == "0000")
                    {
                        List<string> orderMid = new List<string>(); orderMid.Add(parent.order_mid);
                     
                        relastionMappingOrderListResModel mappingParent = mapping.content.orderList.Where(x => x.parent == true).Select(x=>x).ToList().FirstOrDefault();

                        //如果母單CX & BACK , 直接壓 is_need_back =false // 表示不再確認，並留註記
                        //如果母單GO 要判斷是否子單相同，都以子單的為主，
                        //(1)子單只要是 CX + 其他BACK 就回壓BACK , is_back=true 並留註記
                        //(2)全CX 就直接壓 is_need_back =false // 表示不再確認，並留註記
                        if (mappingParent.orderStatus == "BACK" || mappingParent.orderStatus == "CX")
                        {
                            this.UdpIsNeedBack(guidKey, parent.booking_mst_xid, false);
                        }
                        else if (mappingParent.orderStatus == "GO")
                        {
                            if (mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "GO")?.Count() > 1)
                            {
                                //有GO不做事
                            }
                            else if (mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "CX")?.Count() == mapping.content.orderList.Count - 1)
                            {
                                //子單全CX
                                this.UdpIsNeedBack(guidKey, parent.booking_mst_xid, false);
                            }
                            else if (mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "BACK")?.Count() == mapping.content.orderList.Count - 1)
                            {
                                //CALL JAVA SET BACK
                                if (this.ParentStatusBack(guidKey, orderMid))
                                {
                                    this.UdpIsBack(guidKey, parent.booking_mst_xid, true);
                                }
                            }
                            else if (mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "BACK")?.Count() > 1)
                            {
                                //CX +BACK ==TOTAL
                                var cxCount = mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "CX")?.Count();
                                var backCount = mapping.content.orderList.Where(x => x.parent == false && x.orderStatus == "BACK")?.Count();
                                if ((cxCount + backCount) == mapping.content.orderList.Count - 1)
                                {
                                    //CALL JAVA SET BACK
                                    if (this.ParentStatusBack(guidKey, orderMid))
                                    {
                                        this.UdpIsBack(guidKey, parent.booking_mst_xid, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.UdpIsNeedBack(guidKey, parent.booking_mst_xid, false);
                        //通知可能非母單
                        _slack.SlackPost(guidKey, "ParentStatusBack", "BatchJobRepository/ParentStatusBack", $"order_mid:{parent.order_mid}, mapping失敗，可能非母單！", $"Result ={ JsonConvert.SerializeObject(mapping)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"BatchJobRepository_SetParentBack_exception:GuidKey={guidKey}, Message={ex.Message}, StackTrace={ex.StackTrace}");

            }
        }


        private Boolean UdpIsNeedBack(string guidKey,Int64 bookingMstXid,Boolean isNeedBack)
        {
            try
            {
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    string sqlStmt = @"update booking_mst set is_need_back=:is_need_back,modify_user='SYSTEM',modify_datetime=now() where booking_mst_xid =:booking_mst_xid";
                    conn.Execute(sqlStmt,new { is_need_back= isNeedBack, booking_mst_xid=bookingMstXid });
                }

                Website.Instance.logger.Info($"BatchJobRepository_UdpIsNeedBack:GuidKey={guidKey},booking_mst_xid:{bookingMstXid} 已為BACK or CX");
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"BatchJobRepository_UdpIsNeedBack_exception:GuidKey={guidKey}, Message={ex.Message}, StackTrace={ex.StackTrace}");

            }
            return true;
        }

        private Boolean UdpIsBack(string guidKey, Int64 bookingMstXid, Boolean isBack)
        {
            try
            {
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    string sqlStmt = @"update booking_mst set is_back=:is_back,modify_user='SYSTEM',modify_datetime=now() where booking_mst_xid =:booking_mst_xid";
                    conn.Execute(sqlStmt, new { is_back = isBack, booking_mst_xid=bookingMstXid });
                }
                Website.Instance.logger.Info($"BatchJobRepository_UdpIsBack:GuidKey={guidKey},booking_mst_xid:{bookingMstXid} 壓為BACKs");
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"BatchJobRepository_UdpIsBack_exception:GuidKey={guidKey}, Message={ex.Message}, StackTrace={ex.StackTrace}");
            }
            return true;
        }


        private Boolean ParentStatusBack(string guidKey, List<string> order_mid)
        {
            try
            {
                string url = $"{Website.Instance.Configuration["COMBO_SETTING:JAVA"]}api/v3/order/statusback";
                Dictionary<string, object> json = new Dictionary<string, object>();
                json.Add("orderMidList", order_mid);

                //呼叫java母子狀態
                string deviceId = Guid.NewGuid().ToString();
                OrderApiRqModel JavaApi = new OrderApiRqModel()
                {
                    apiKey = Website.Instance.Configuration["KKdayAPI:Body:ApiKey"],
                    userOid = Website.Instance.Configuration["KKdayAPI:Body:OcbtUserOid"],
                    locale = "zh-tw",
                    ver = Website.Instance.Configuration["KKdayAPI:Body:Ver"],
                    ipaddress = _comboBookingRepos.GetLocalIPAddress(),
                    json=json
                };
                string result = CommonProxy.Post(url, JsonConvert.SerializeObject(JavaApi, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));


                var rs = JObject.Parse(result);
                if (rs["content"]["result"]?.ToString() != "0000")
                {
                    //警示
                    _slack.SlackPost(guidKey, "ParentStatusBack", "BatchJobRepository/ParentStatusBack", $"order_mid:{JsonConvert.SerializeObject(order_mid)}, SetParentBack回覆失敗", $"Result ={ result}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"ParentStatusBack error.order_mid={JsonConvert.SerializeObject(order_mid)} ,Message={ex.Message},stackTrace={ex.StackTrace}");
                return false;
            }
        }
    }
}

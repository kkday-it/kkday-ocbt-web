using System;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Npgsql;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using KKday.Web.OCBT.Proxy;

namespace KKday.Web.OCBT.Models.Repository
{
    public class ComboBookingRepository
    {
        public string ComboBooking()
        {
            return "";
        }
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
        public long InsertBookingMst(BookingMstModel rq)
        {
            try
            {
                string sql = @"INSERT INTO public.booking_mst(order_mid,order_oid,prod_oid,go_date,booking_model,booking_mst_order_status,booking_mst_voucher_status,
is_callback,is_back,create_user,create_date)
VALUES(:order_mid,:order_oid,:prod_oid,:booking_model,:booking_mst_order_status,:booking_mst_voucher_status,:is_callback,:is_back,:create_user,now());";
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    return conn.QuerySingle<long>(sql,rq);
                }

            }
            catch(Exception ex)
            {
                Website.Instance.logger.Info($"InsBookingMst order_mid {rq.order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }
        
        public void CallBackJava(ResponseJson jsonData)
        {
            try
            {
                ResponseJavaModel callbackData = new ResponseJavaModel
                {
                    apiKey = Website.Instance.Configuration["KKAPI_INPUT:API_KEY"],
                    userOid = Website.Instance.Configuration["KKAPI_INPUT:USER_OID"],
                    locale = "zh-tw",
                    ipaddress = GetLocalIPAddress(),
                    json = jsonData

                };
                string url = "";
                string result= CommonProxy.Post(url, JsonConvert.SerializeObject(callbackData));
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

                #endregion
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"ComboBookingFlow errpr {queue} error:{ex.Message},{ex.StackTrace}");
            }
            #region 查找母單

            #endregion
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

using System;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Npgsql;
using System.Linq;
namespace KKday.Web.OCBT.Models.Repository
{
    public class ComboBookingRepository
    {
        public string ComboBooking()
        {
            return "";
        }
        public BookingMstModel GetBookingMstData(string order_mid,string filter)
        {
            try
            {
                string sql = @"SELECT * FROM BOOKING_MST WHERE 1=1 ";
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    conn.Open();
                    return conn.Query<BookingMstModel>(sql).First();
                }

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Info($"GetBookingMstData order_mid {order_mid} error:{ex.Message},{ex.StackTrace}");
                throw ex;
            }
        }

    }
}

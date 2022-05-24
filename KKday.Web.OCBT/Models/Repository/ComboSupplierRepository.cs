using System;
using Dapper;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Npgsql;
using System.Linq;
using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Repository
{
    public class ComboSupplierRepository
    {
        public ComboSupResponseModel getComboSupLst(ComboSupRequestModel rq)
        {
            ComboSupResponseDataModel res = new ComboSupResponseDataModel();
            List<int> supOidLst = new List<int>();

            try
            {
                //取comboSupList
                using (var conn = new NpgsqlConnection(Website.Instance.OCBT_DB))
                {
                    string sqlStmt = @"select string_agg(supplier_oid::text,',') as sup_oid from combo_supplier 
where status = 'NW'";

                    string result = conn.QuerySingle<string>(sqlStmt);
                    
                    if( !string.IsNullOrEmpty(result))supOidLst = result.Split(',').Select(Int32.Parse).ToList();
                }

                return new ComboSupResponseModel
                {
                    metadata = new ResponseMetaModel
                    {
                        status = "1000",
                        description = ""
                    },
                    data = new ComboSupResponseDataModel
                    {
                        supplier_oid = supOidLst
                    }
                };
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboSupplierRepository_getComboSupLst_exception:GuidKey ={rq?.request_uuid}, Message={ex.Message}, StackTrace={ex.StackTrace}");

                return new ComboSupResponseModel
                {
                    metadata = new ResponseMetaModel
                    {
                        status = "9999",
                        description = "異常:" + ex.Message.ToString()
                    },
                    data =new ComboSupResponseDataModel {
                        supplier_oid = supOidLst
                    }
                };
            }
        }
    }

    
}

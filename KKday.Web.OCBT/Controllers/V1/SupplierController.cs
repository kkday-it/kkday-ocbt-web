using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using KKday.Web.OCBT.Models.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.V1
{
    [Route("api/v1/[controller]")]
    public class SupplierController : Controller
    {
        private IRedisHelper _redisHelper;
        private readonly ComboSupplierRepository _comboSupRepos;
        public SupplierController(IRedisHelper redisHelper, ComboSupplierRepository comboSupRepos)
        {
            _redisHelper = redisHelper;
            _comboSupRepos = comboSupRepos;
        }
        
        [HttpPost("GetComboSupplierList")]
        public ComboSupResponseModel GetComboSupplierList([FromBody] ComboSupRequestModel rq)
        {
            try
            {
                Website.Instance.logger.Info($"ComboBooking_GetComboSupplierList_quest:{JsonConvert.SerializeObject(rq)}");

                if (rq?.sourceId != "BE2" && rq?.sourceId != "JAVA" && rq?.sourceId != "MKT" && rq?.sourceId != "PROD") throw new Exception("非指定使用者！");

                return _comboSupRepos.getComboSupLst(rq);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"ComboBooking_GetComboSupplierList_exception:GuidKey ={rq?.requestUuid}, Message={ex.Message}, StackTrace={ex.StackTrace}");

                return new ComboSupResponseModel
                {
                    metadata = new ResponseMetaModel
                    {
                        status = "9999",
                        description = "異常:" + ex.Message.ToString()
                    }
                };
            }
        }
    }
}

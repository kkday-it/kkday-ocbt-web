using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "KKdayOnly")]
    public class OrderController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Query Order List
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IActionResult FetchOrderData(string filter, string sort, string order, int offset, int limit)
        {
            var guidKey = User.FindFirst("GuidKey").Value;
            Dictionary<string, object> jsonData = new Dictionary<string, object>();

            try
            {

            }
            catch (Exception ex)
            {
                Website.Instance.logger.Fatal($"Order_FetchOrderData_Exception:GuidKey={guidKey}, Message={ex.Message}, StackTrace={ex.StackTrace}");
                jsonData.Add("total", 0);
                jsonData.Add("totalNotFiltered", 0);
                jsonData.Add("rows", new string[] { });
            }

            return Json(jsonData);
        }
    }
}

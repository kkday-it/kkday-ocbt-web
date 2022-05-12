using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.Controllers
{
    [AllowAnonymous]
    public class LocaleController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string id)
        {
            // 設定 Culture
            HttpContext.Response.Cookies.Append(
                      CookieRequestCultureProvider.DefaultCookieName,
                      CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(id ?? CultureInfo.CurrentCulture.ToString()))
                        );

            string referer = Request.Headers["Referer"].ToString();
            if (referer == "")
            {
                return Redirect("~/");
            }
            else
            {
                return Redirect(referer);
            }
        }
    }
}

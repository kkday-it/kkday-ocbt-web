using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.Login;
using KKday.Web.OCBT.Proxy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KKday.Web.OCBT.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "KKdayOnly")]
    public class LoginController : Controller
    {
        private readonly OAuthProxy _oAuthProxy;
        public LoginController(OAuthProxy oAuthProxy)
        {
            _oAuthProxy = oAuthProxy;
        }

        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Claim取使用者類型
            var userType = User.FindFirst("Account")?.Value;
            if (userType != null) return Redirect("~/");

            return View();
        }
        /// <summary>
        /// 使用者登入認證
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Authen(LoginRqModel rq)
        {
            rq.guid_key = Guid.NewGuid().ToString();
            Dictionary<string, string> jsonData = new Dictionary<string, string>() { { "status", "ERROR" } };

            try
            {
                // Call OAuth 驗證 KKday E-mail
                var account = _oAuthProxy.UserAuth(rq);
                if (account.result == "0000")
                {
                    var claims = new List<Claim>
                    {
                        new Claim("Account", account.userInfo.email),
                        new Claim("UUID", account.userInfo.userUuid),
                        new Claim("GuidKey", rq.guid_key),
                        new Claim("IdentityType", "KKDAY"),
                        new Claim("Ver", Website.Instance.PrincipleVersion), // 帶入當前ClaimPriciple版號
                    };
                    var userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties()
                        {
                            ExpiresUtc = DateTime.UtcNow.AddDays(10), // 預設 Cookie 有效時間
                            IsPersistent = rq.rememberme,
                            AllowRefresh = true
                        });
                    jsonData["status"] = "OK";
                    jsonData.Add("GuidKey", rq.guid_key);
                    jsonData.Add("url", Url.Content("~/"));
                }
                else
                {
                    jsonData.Add("msg", account.result_message);
                }

                #region 登入後重新設定使用者的 Cookie Culture

                // 設定 Culture
                HttpContext.Response.Cookies.Append(
                      CookieRequestCultureProvider.DefaultCookieName,
                      CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureInfo.CurrentCulture.ToString()))
                   );

                #endregion 登入後重新設定使用者的 Cookie Culture
            }
            catch (Exception ex)
            {
                Website.Instance.logger.FatalFormat($"GuidKey : {rq.guid_key}; OCBT Login Authen Error: Msg={ex.Message}, StackTrace={ex.StackTrace}");
                jsonData.Add("msg", ex.Message);
            }
            return Json(jsonData);
        }
        /// <summary>
        /// 使用者登出 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                var account = User.FindFirstValue("Account")?.ToString();
                //Website.Instance.logger.Info($"Logout_AuthenAsync_Account:{JsonConvert.SerializeObject(account)}");
                await HttpContext.SignOutAsync();
            }
            catch (Exception ex)
            {
                await HttpContext.SignOutAsync();
            }

            return Redirect("~/");
        }
    }
}

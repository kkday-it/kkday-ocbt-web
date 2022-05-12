using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Proxy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KKday.Web.OCBT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region ASP.NET Core多語系挖字

            //JSON多語系挖字
            services.AddJsonLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("zh-TW"),
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    // 依序進行判斷文化特性，以上都沒有才會以 DefaultRequestCulture 決定語系。
                    new CookieRequestCultureProvider(), // (1) 透過 cookie 的值判斷要求的文化特性資訊。
                    new QueryStringRequestCultureProvider(), // (2) 透過查詢字串中的值，決定要求的文化特性資訊。
                    new AcceptLanguageHeaderRequestCultureProvider(), // (3) 透過 Accept-Language 標頭的值，判斷要求的文化特性資訊。 
                };
            });

            #endregion ASP.NET Core多語系挖字

            #region Cookie 驗證服務
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".OCBT.SharedCookie";
                    options.LoginPath = "/Login/";
                    options.SlidingExpiration = true;
                    // options.Cookie.Domain = "kkday.com";

                    options.Events.OnValidatePrincipal = (context) =>
                    {
                        int failCount = 0;
                        var identity = (ClaimsIdentity)context.Principal.Identity;
                        var versionKey = identity.FindFirst("Ver");
                        if (versionKey == null)
                        {
                            failCount++;
                        }
                        else
                        {
                            var serverVersion = new Version(Website.Instance.PrincipleVersion); // 與 ~/Login/AuthenAsync的 Claim
                            var localeVersion = new Version(versionKey.Value);
                            // Principal Version => 0:同版本, 1:不同版本
                            if (serverVersion.CompareTo(localeVersion) == 1)
                            {
                                failCount++;
                            }
                        }
                        var guidKey = identity.FindFirst("GuidKey");
                        if (guidKey == null)
                        {
                            identity.AddClaims(new[] {
                                new Claim("GuidKey", System.Guid.NewGuid().ToString("N"))
                            });
                            context.ShouldRenew = true;
                            // identity.AddClaims(new[] { new Claim("GuidKey", System.Guid.NewGuid().ToString("N")) });
                            // context.ShouldRenew = true;
                            failCount++;
                        }
                        if (failCount > 0)
                        {
                            context.RejectPrincipal();
                            context.Response.Redirect("/Login/LogOutAsync");
                        }
                        // 驗證cookie內IdentityType是否存在
                        if (identity.FindFirst("IdentityType") == null)
                        {
                            // 不存在則否認此cookie
                            context.RejectPrincipal();
                        }
                        return Task.CompletedTask;
                    };
                });
            #endregion Cookie 驗證服務

            // 指定Cookie授權政策區分不同身分者
            services.AddAuthorization(options =>
            {
                options.AddPolicy("KKdayOnly", policy => policy.RequireClaim("IdentityType", "KKDAY"));
            });

            #region Dependency Injection Regisgter --begin
            services.AddSingleton<OAuthProxy>();
            #endregion Dependency Injection Regisgter -- end

            services.AddSession();
            services.AddControllersWithViews().AddViewLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // 初始化-網站主控台
            Website.Instance.Init(this.Configuration);

            // 多語系挖字初始設定
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

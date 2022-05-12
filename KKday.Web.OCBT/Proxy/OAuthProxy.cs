using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using KKday.Web.OCBT.AppCode;
using KKday.Web.OCBT.Models.Model.Login;
using Newtonsoft.Json;

namespace KKday.Web.OCBT.Proxy
{
    public class OAuthProxy
    {
        /// <summary>
        /// Call OAuth Authentication KKday User
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        public LoginRsModel UserAuth(LoginRqModel rq)
        {
            LoginRsModel rs = new LoginRsModel();
            try
            {
                // 第一層:Authentication Email
                var auth = OAuthAuthentication(rq);
                if (auth.metadata.status == "AU0000")
                {
                    // 第二層:Get Profile
                    var _profile = OAuthGetProfile(rq);
                    if (_profile.metadata.status == "AU0000")
                    {
                        rs.result = "0000";
                        rs.result_message = "OK";
                        rs.userInfo = new UserInfoEx
                        {
                            email = rq.email,
                            userName = _profile.data.subUsers.Where(x => x.platformOid == "1").Select(s => s.userDetail.userName).FirstOrDefault() ?? string.Empty,
                            userUuid = _profile.data.subUsers.Where(x => x.platformOid == "1").Select(s => s.userDetail.userUuid).FirstOrDefault() ?? string.Empty
                        };
                    }
                    else
                    {
                        rs.result = _profile.metadata.status;
                        rs.result_message = $"OAuth Get Profile Error: {_profile.metadata.desc}";
                    }
                }
                else
                {
                    rs.result = auth.metadata.status;
                    rs.result_message = $"OAuth Authentication Email Error: {auth.metadata.desc}";
                }
            }
            catch (Exception ex)
            {
                Website.Instance.logger.FatalFormat($"GuidKey : {rq.guid_key}; OCBT OAuthProxy UserAuth Error: Msg={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
            return rs;
        }
        /// <summary>
        /// 使用者登入驗證
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        private OAuthLoginRsModel OAuthAuthentication(LoginRqModel rq)
        {
            try
            {
                Dictionary<string, string> parms = new Dictionary<string, string>
                {
                    { "account", rq.email},
                    { "password", rq.password}
                };

                string url = $"{Website.Instance.Configuration["OAuth:Url"]}/api/v1/auth/be2/login";

                var result = CommonProxy.Post(url: url, parms: JsonConvert.SerializeObject(parms));

                return JsonConvert.DeserializeObject<OAuthLoginRsModel>(result);
            }
            catch (Exception ex)
            {
                Website.Instance.logger.FatalFormat($"GuidKey : {rq.guid_key}; OCBT OAuthProxy OAuthAuthentication Error: Msg={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        /// <summary>
        /// 取得auth_user+sub_users資料by AuthKey 
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        private OAuthProfileRsModel OAuthGetProfile(LoginRqModel rq)
        {
            try
            {
                string result = "";
                using (var handler = new HttpClientHandler())
                {
                    // Ignore Certificate Error!!
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        string url = $"{Website.Instance.Configuration["OAuth:Url"]}/api/v1/user?authKey={rq.email}";

                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            request.Headers.Add("Authorization", Website.Instance.Configuration["OAuth:Token"]);

                            var response = client.SendAsync(request).Result;

                            result = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    return JsonConvert.DeserializeObject<OAuthProfileRsModel>(result);
                }
            }
            catch(Exception ex)
            {
                Website.Instance.logger.FatalFormat($"GuidKey : {rq.guid_key}; OCBT OAuthProxy OAuthGetProfile Error: Msg={ex.Message}, StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
    }
}

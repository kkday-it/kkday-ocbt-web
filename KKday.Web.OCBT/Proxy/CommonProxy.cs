using System;
using System.Net.Http;
using System.Net.Http.Headers;
using KKday.Web.OCBT.AppCode;

namespace KKday.Web.OCBT.Proxy
{
    public class CommonProxy
    {
        /// <summary>
        /// 通用的 Post Method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms"></param>
        /// <param name="guidKey"></param>
        /// <returns></returns>
        public static string Post(string url, string parms, string guidKey = null)
        {
            try
            {
                string result = "";
                using (var handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) =>
                        {
                            return true;
                        };

                    using (var client = new HttpClient(handler))
                    {
                        using (HttpContent content = new StringContent(parms))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            var response = client.PostAsync(url, content).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            //Website.Instance.logger.Info($"guidKey : {guidKey}; KKday API UserAuth URL:{url},data:{json_data},result:{result},URL Response StatusCode:{response.StatusCode}");
                        }
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                Website.Instance.logger.Fatal($"CommonProxy_PostProxy_Exception:GuidKey={guidKey}, Message={ex.Message},StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        /// <summary>
        /// 通用的 Get Method
        /// </summary>
        /// <param name="guidKey"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string Get(string guidKey, string uri)
        {
            try
            {
                string jsonResult;

                //建立連線到WMS API
                using (var handler = new HttpClientHandler())
                {
                    // Ignore Certificate Error!!
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
                        {
                            //Get使用
                            //request.Headers.Add("x-auth-key", Website.Instance.Configuration["KEY:AvailableArea_API"]); ;
                            //request.Headers.Add("{0}.{1}", "x-auth-key", "kkdaysearchapi_Rfd_fsg+x+TcJy");
                            var response = client.SendAsync(request).Result;
                            jsonResult = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }

                return jsonResult;
            }
            catch (Exception ex)
            {
                //紀錄CountryProxy_Get的Log
                Website.Instance.logger.Fatal($"CreditProxy_Get_Exception:GuidKey={guidKey}, Message={ex.Message},StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using KKday.Web.OCBT.AppCode;

namespace KKday.Web.OCBT.Proxy
{
    public class OrderProxy
    {
        /// <summary>
        /// 通用的 Post Method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms"></param>
        /// <param name="guidKey"></param>
        /// <returns></returns>
        public  string Post(string url,string parms,string guidKey = null)
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
                Website.Instance.logger.Fatal($"OrderProxy_Post_Exception:GuidKey={guidKey}, Message={ex.Message},StackTrace={ex.StackTrace}");
                throw ex;
            }
        }
        
    }
}

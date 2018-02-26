using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InvisibleCaptchaTest.Captcha
{
    public class CaptchaServer
    {
        private const string jsonEncoding = "application/json";

        private readonly string secretKey;
        private readonly bool useProxy;
        private readonly string proxyIp;
        private readonly int proxyPort;

        private string apiVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        public CaptchaServer(string secretKey, bool useProxy, string proxyIp, int proxyPort)
        {
            this.secretKey = secretKey;
            this.useProxy = useProxy;
            this.proxyIp = proxyIp;
            this.proxyPort = proxyPort;
        }

        public async Task<bool> Verify(String token)
        {
            HttpClient httpClient = getHttpClient();

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("secret", secretKey));
            postData.Add(new KeyValuePair<string, string>("response", token));
            HttpContent httpContent = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = await httpClient.PostAsync(apiVerifyUrl, httpContent);

            string serializedResponse = await response.Content.ReadAsStringAsync();

            VerifyCaptchaResponse verifyResponse = JsonConvert.DeserializeObject<VerifyCaptchaResponse>(serializedResponse);

            return verifyResponse.Success;
        }

        private HttpClient getHttpClient()
        {
            // http://drumcoder.co.uk/blog/2017/aug/25/http-client-dot-net-core/

            WebProxy webProxy = null;
            if (useProxy)
            {
                // NetworkCredential proxyCredential = new NetworkCredential("myUsername", "myPassword");

                webProxy = new WebProxy($"{proxyIp}:{proxyPort}");

                // { Credentials = proxyCredential };
            }

            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = webProxy,
                UseProxy = useProxy,
                PreAuthenticate = true,
                UseDefaultCredentials = true,
            };

            HttpClient httpClient = new HttpClient(httpClientHandler);

            return httpClient;
        }
    }
}

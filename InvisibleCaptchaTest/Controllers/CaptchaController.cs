using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InvisibleCaptchaTest.Captcha;
using InvisibleCaptchaTest.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InvisibleCaptchaTest.Controllers
{
    public class CaptchaController : Controller
    {
        public IActionResult Index()
        {
            var config = new Config().GetConfiguration();

            ViewBag.CaptchaSiteKey = config["Captcha:SiteKey"];

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostFormDataAction(IFormCollection form)
        {
            string field = form["field"];
            string token = form["g-recaptcha-response"];

            var config = new Config().GetConfiguration();

            bool captchaEnabled = bool.Parse(config["Captcha:Enabled"]);
            bool useProxy = bool.Parse(config["Captcha:UseProxy"]);
            string proxyIp = config["Captcha:ProxyIp"];
            int proxyPort = int.Parse(config["Captcha:ProxyPort"]);
            string secretKey = config["Captcha:SecretKey"];

            var captchaSever = new CaptchaServer(secretKey, useProxy, proxyIp, proxyPort);

            bool isVerified = await captchaSever.Verify(token);

            return Content($"Posted '{field}' and verified? {isVerified}");
        }
    }
}
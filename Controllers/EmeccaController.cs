using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using com.emecca.service;
using System.Text.Json.Nodes;
using System.Net;
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Text;
using Newtonsoft.Json.Linq;

namespace com.emecca.controller
{
    [ApiController]
    [Route("[controller]")]
    public class EmeccaController : ControllerBase
    {
        private readonly ILogger<EmeccaController> _logger;
        private readonly EmeccaDotNetContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public EmeccaController(EmeccaDotNetContext context, ILogger<EmeccaController> logger, IConfiguration config, IHttpClientFactory clientFactory)
        {
            _context = context;
            _logger = logger;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback()
        {
            if (!this.Request.Query.TryGetValue("code", out var code))
                return this.StatusCode(400);
            var redirectUrl = @"http://localhost:3000" + "?code=" + code;
            return this.Redirect(redirectUrl);
        }

        [HttpGet("/Authentication/GetToken")]
        public async Task<string> ExchangeAccessToken(string code)
        {
            return "ertyuyui";
            var client = _clientFactory.CreateClient();
            var oauth_host = _config["OauthHost"];
            var auth_host_id = _config["TokenAuthId"];
            var auth_host_serect = _config["TokenAuthPass"];
            var request = new HttpRequestMessage(HttpMethod.Post, @"https://" + oauth_host + @"/oauth2ServerToken.do");
            var callback = @"http://localhost:3000";

            request.Content = new FormUrlEncodedContent(
               new Dictionary<string, string>
               {
                   ["grant_type"] = "authorization_code",
                   ["code"] = code,
                   ["redirect_uri"] = callback,
                   ["client_id"] = auth_host_id,
                   ["client_secret"] = auth_host_serect
               });

            var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return (null);
            }
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonNode.Parse(content);
            var accessToken = result["access_token"].GetValue<string>();

            return accessToken;
        }

        [HttpGet("/Authentication/GetLoginUser")]
        public async Task<ActionResult<EmeUserBasVO>> GetLoginUser()
        {
            string token="";
            Request.Headers.TryGetValue("Authorization", out var headerValue);
            if (headerValue.ToString() != "")
            {
                token = headerValue.ToString().Split(' ')[1];
                var result = await _context.emeuser_vo.FirstOrDefaultAsync(c => c.UserName == "RU");
                return result;
            }

            using var trans = _context.Database.BeginTransaction();
            try
            {
                using (WebClient client2 = new WebClient())
                {
                    var oauth_host = _config["OauthHost"];
                    HttpWebRequest bearerReq = WebRequest.Create("https://" + oauth_host + @"/oauth2ServerInfo.do") as HttpWebRequest;
                    bearerReq.Accept = "application/json";
                    bearerReq.Method = "POST";
                    bearerReq.ContentType = "application/x-www-form-urlencoded";
                    bearerReq.KeepAlive = false;
                    bearerReq.Headers.Add("Authorization", "Bearer " + token);
                    WebResponse bearerResp = bearerReq.GetResponse();
                    StreamReader reader = new StreamReader(bearerResp.GetResponseStream(), Encoding.UTF8);
                    var req_user_info = reader.ReadToEnd();
                    if (req_user_info != null)
                    {
                        var user_info = JsonNode.Parse(req_user_info);
                        var use_no = user_info["cn"].GetValue<string>();
                        var result = await _context.emeuser_vo.FirstOrDefaultAsync(c => c.UserName == use_no && c.Status == "Y");   //.Take(10)
                        trans.Commit();
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                this._logger.LogError("ERROR", e);
                return StatusCode(500);
                //return null;
            }

        }
    }
}

using System.Net;
using System.Text;

namespace EmeccaRestfulApi.Middleware
{
    public class OAuth2Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public OAuth2Middleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/EmeccaAuto/Authentication") || context.Request.Path.ToString().Contains("/GetEmeccaObjectId"))
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var oauth_host = _config["OauthHost"];
                Boolean token_check = false;
                using (WebClient client2 = new WebClient())
                {
                    HttpWebRequest bearerReq = WebRequest.Create("https://" + oauth_host + @"/oauth2ServerInfo.do") as HttpWebRequest;
                    bearerReq.Method = "POST";
                    bearerReq.Accept = "application/json";
                    bearerReq.ContentType = "application/x-www-form-urlencoded";
                    bearerReq.KeepAlive = false;
                    bearerReq.Headers.Add("Authorization", authHeader);
                    WebResponse bearerResp = bearerReq.GetResponse();
                    StreamReader reader = new StreamReader(bearerResp.GetResponseStream(), Encoding.UTF8);
                    var req_user_info = reader.ReadToEnd();
                    if (req_user_info == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                    else
                    {
                        await _next(context);
                        return;
                    }
                }

            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            await _next(context);
        }
    }
}

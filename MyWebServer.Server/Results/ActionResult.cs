using MyWebServer.Server.Http;
using MyWebServer.Server.Http.Collections;

namespace MyWebServer.Server.Results
{
    public class ActionResult : HttpResponse
    {
        public ActionResult(HttpResponse response) 
            : base(response.StatusCode)
        {
            this.Content = response.Content;
            this.PrepareHeaders(response.Headers);
            this.PrepareCookies(response.Cookies);
        }


        private void PrepareHeaders(HeaderCollection headers)
        {
            foreach (var header in headers)
            {
                this.Headers.Add(header.Name, header.Value);
            }
        }

        private void PrepareCookies(CookieCollection cookies)
        {
            foreach (var cookie in cookies)
            {
                this.Cookies.Add(cookie.Name, cookie.Value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server.Http;

namespace MyWebServer.Server.Results
{
    public class RedirectResult : ActionResult
    {
        public RedirectResult(HttpResponse response, string location) : base(response)
        {
            this.StatusCode = HttpStatusCode.Found;

            this.Headers.Add(HttpHeader.Location, location);
        }
    }
}

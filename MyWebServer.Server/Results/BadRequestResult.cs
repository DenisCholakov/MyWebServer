using MyWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Results
{
    public class BadRequestResult : ActionResult
    {
        public BadRequestResult(HttpResponse response)
            : base(response)
            => this.StatusCode = HttpStatusCode.BadRequest;
    }
}

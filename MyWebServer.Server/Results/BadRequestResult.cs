using MyWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Results
{
    public class BadRequestResult : HttpResponse
    {
        public BadRequestResult() 
            : base(HttpStatusCode.BadRequest)
        {
        }
    }
}

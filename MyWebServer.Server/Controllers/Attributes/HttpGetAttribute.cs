using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server.Http;

namespace MyWebServer.Server.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute() 
            : base(HttpMethod.GET)
        {
        }
    }
}

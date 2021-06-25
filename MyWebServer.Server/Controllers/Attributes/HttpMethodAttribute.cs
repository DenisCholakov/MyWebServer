using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server.Http;

namespace MyWebServer.Server.Controllers.Attributes
{
    public abstract class HttpMethodAttribute : Attribute
    {
        protected HttpMethodAttribute(HttpMethod httpMethod)
            => this.HttpMethod = httpMethod;
        public HttpMethod HttpMethod { get; }
    }
}

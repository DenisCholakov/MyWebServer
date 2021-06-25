using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.App.Data;
using MyWebServer.Server.Controllers;
using MyWebServer.Server.Controllers.Attributes;
using MyWebServer.Server.Http;

namespace MyWebServer.App.Controllers
{
    public class CatsController : Controller
    {
        private readonly IData data;

        public CatsController(IData data)
            => this.data = data;

        public HttpResponse All()
        {
            var cats = this.data.Cats;

            return View(cats);
        }

        [HttpGet]
        public HttpResponse Create() => View();

        [HttpPost]
        public HttpResponse Save(string name, int age)
        {
            return Text($"{name} - {age}");
        }
    }
}

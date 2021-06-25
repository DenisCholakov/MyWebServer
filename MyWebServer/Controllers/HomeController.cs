using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server.Controllers;
using MyWebServer.Server.Http;
using MyWebServer.Server.Results;

namespace MyWebServer.App.Controllers
{
    public class HomeController : Controller
    {
        public HttpResponse Index()
            => Text("Hello from Denis!");

        public HttpResponse ToCats() => Redirect("/Animals/Cats");

        public HttpResponse ToSoftUni() => Redirect("https://softuni.bg");

        public HttpResponse StaticFiles() => View();

        public HttpResponse Error() => throw new InvalidOperationException("Exception was thrown");
    }
}

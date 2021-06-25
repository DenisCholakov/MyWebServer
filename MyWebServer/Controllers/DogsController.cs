using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWebServer.App.Models.Animals;
using MyWebServer.Server.Controllers;
using MyWebServer.Server.Controllers.Attributes;
using MyWebServer.Server.Http;

namespace MyWebServer.App.Controllers
{
    public class DogsController : Controller
    {
        [HttpGet]
        public HttpResponse Create() => View();

        [HttpPost]
        public HttpResponse Create(DogFormModel model)
            => Text($"Dog: {model.Name} - {model.Age} - {model.Breed}");
    }
}

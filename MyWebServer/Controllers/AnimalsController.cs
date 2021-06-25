using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.App.Models.Animals;
using MyWebServer.Server.Controllers;
using MyWebServer.Server.Http;

namespace MyWebServer.App.Controllers
{
    public class AnimalsController : Controller
    {
        public HttpResponse Cats()
        {
            const string nameKey = "Name";
            const string ageKey = "Age";

            var query = this.Request.Query;
            var catName = query.Contains(nameKey) ? query[nameKey] : "the cats";
            var catAge = query.Contains(ageKey) ? int.Parse(query[ageKey]) : 0;

            var viewModel = new CatViewModel
            { 
                Name = catName, 
                Age = catAge };

            return View(viewModel);
        }

        public HttpResponse Dogs() => View(new DogViewModel
        { 
            Name = "Rex",
            Age = 3,
            Breed = "Street perfect"
        });

        public HttpResponse Bunnies() => View("Bunnies");

        public HttpResponse Turtles() => View("Animals/Wild/Turtles");
    }
}

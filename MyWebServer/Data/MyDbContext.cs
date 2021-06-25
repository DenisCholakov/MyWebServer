using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.App.Data.Models;

namespace MyWebServer.App.Data
{
    public class MyDbContext : IData
    {
        public MyDbContext()
            => this.Cats = new List<Cat>
            {
                new Cat { Id = 1, Name = "Sharo", Age = 5},
                new Cat { Id = 2, Name = "Lady", Age = 13}
            };

        public IEnumerable<Cat> Cats { get; set; }
    }
}

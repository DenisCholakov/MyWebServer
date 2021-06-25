using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Controllers
{
    public static class ControllerHelpers
    {
        public static string GetControllerName(this Type type)
            => type.Name.Replace(nameof(Controller), String.Empty);
    }
}
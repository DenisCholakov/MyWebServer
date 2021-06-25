using System.Threading.Tasks;

using MyWebServer.App.Controllers;
using MyWebServer.App.Data;
using MyWebServer.Server;
using MyWebServer.Server.Controllers;
using MyWebServer.Server.Results.Views;

namespace MyWebServer.App
{
    public class StartUp
    {
        public static async Task Main(string[] args)
        {
            await HttpServer
                .WithRoutes(routes => routes
                    .MapStaticFiles()
                    .MapControllers()
                    .MapGet<HomeController>("/toCats", c => c.ToCats()))
                .WithServices(services => services
                    .AddService<IViewEngine, CompilationViewEngine>()
                    .AddService<IData, MyDbContext>())
                .Start();
        }
    }
}

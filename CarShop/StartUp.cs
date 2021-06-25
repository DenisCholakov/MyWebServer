using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using MyWebServer.Server;
using MyWebServer.Server.Controllers;
using MyWebServer.Server.Results.Views;

using CarShop.Data;
using CarShop.Services;

namespace MyWebServer.App
{
    public class StartUp
    {
        public static async Task Main(string[] args)
        {
            await HttpServer
                .WithRoutes(routes => routes
                    .MapStaticFiles()
                    .MapControllers())
                .WithServices(services => services
                    .AddService<IViewEngine, CompilationViewEngine>()
                    .AddService<CarShopDbContext>()
                    .AddService<IValidator, Validator>()
                    .AddService<IPasswordHasher, PasswordHasher>()
                    .AddService<IUserService, UserService>())
                .WithConfiguration<CarShopDbContext>(context => context.Database.Migrate())
                .Start();
        }
    }
}

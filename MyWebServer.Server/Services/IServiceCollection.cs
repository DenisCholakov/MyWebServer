using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Services
{
    public interface IServiceCollection
    {
        IServiceCollection AddService<TService, TImplementation>()
            where TService : class
            where TImplementation : TService;

        IServiceCollection AddService<TService>()
            where TService : class;


        TService Get<TService>()
            where TService : class;

        object CreateInstance(Type type);
    }
}

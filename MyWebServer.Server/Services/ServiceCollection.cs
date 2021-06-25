using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Services
{
    public class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, Type> services;

        public ServiceCollection() 
            => this.services = new();

        public IServiceCollection AddService<TService, TImplementation>()
            where TService : class
            where TImplementation : TService
        {
            this.services[typeof(TService)] = typeof(TImplementation);

            return this;
        }

        public IServiceCollection AddService<TService>()
            where TService : class
        {
            this.services[typeof(TService)] = typeof(TService);

            return this;
        }

        public TService Get<TService>() 
            where TService : class
        {
            if (!this.services.ContainsKey(typeof(TService)))
            {
                return null;
            }

            return (TService)this.CreateInstance(typeof(TService));
        }

        public object CreateInstance(Type type)
        {
            if (this.services.ContainsKey(type))
            {
                type = this.services[type];
            }

            var constructors = type.GetConstructors();

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException("Multiple constructors are not supported in the service resolver");
            }

            var constructor = constructors.First();
            var parameters = constructor.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterValue = this.CreateInstance(parameters[i].ParameterType);
                parameterValues[i] = parameterValue;
            }

            return constructor.Invoke(parameterValues);
        }
    }
}

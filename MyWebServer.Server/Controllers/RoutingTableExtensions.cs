using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MyWebServer.Server.Controllers.Attributes;
using MyWebServer.Server.Http;
using MyWebServer.Server.Routing;

namespace MyWebServer.Server.Controllers
{
    public static class RoutingTableExtensions
    {
        private const string defaultActionName = "Index";
        private const string defaultControllerName = "Home";

        private static Type httpResponseType = typeof(HttpResponse);
        private static Type stringType = typeof(string);

        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapGet(path, GetRequestResponseFunction(controllerFunction));

        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, HttpResponse> controllerFunction)
            where TController : Controller
            => routingTable.MapPost(path, GetRequestResponseFunction(controllerFunction));

        public static IRoutingTable MapControllers(this IRoutingTable routingTable)
        {
            var controllerActions = GetControllerActions();

            foreach (var controllerAction in controllerActions)
            {
                var controllerType = controllerAction.DeclaringType;
                var controllerName = controllerType.GetControllerName();
                var actionName = controllerAction.Name;
                var path = $"/{controllerName}/{actionName}";
                Func<HttpRequest, HttpResponse> responseFunction = GetResponseFunction(controllerAction, controllerType, path);

                var httpMethod = HttpMethod.GET;

                var httpMethodAttribute = controllerAction
                    .GetCustomAttribute<HttpMethodAttribute>();

                if (httpMethodAttribute != null)
                {
                    httpMethod = httpMethodAttribute.HttpMethod;
                }

                routingTable.Map(httpMethod, path, responseFunction);

                MapDefaultRoutes(routingTable, httpMethod, controllerName, actionName, responseFunction);

            }

            return routingTable;
        }

        private static IEnumerable<MethodInfo> GetControllerActions()
            => Assembly.GetEntryAssembly()
                .GetExportedTypes()
                .Where(t => t.IsAssignableTo(typeof(Controller))
                    && t.Name.EndsWith(nameof(Controller)))
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => m.ReturnType.IsAssignableTo(httpResponseType)))
                .ToList();

        private static Func<HttpRequest, HttpResponse> GetRequestResponseFunction<TController>(Func<TController, HttpResponse> controllerFunction)
            => request =>
            {
                var controller = (TController)CreateController(typeof(TController), request);

                return controllerFunction(controller);
            };

        private static object CreateController(Type controllerType, HttpRequest request)
        {
            var controller = (Controller)request.Services.CreateInstance(controllerType);
            controllerType
                .GetProperty("Request", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(controller, request);

            return controller;
        }

        private static Func<HttpRequest, HttpResponse> GetResponseFunction(MethodInfo controllerAction, Type controllerType, string path)
        {
            return request =>
            {
                if (!UserIsAuthorized(controllerAction, request.Session))
                {
                    return new HttpResponse(HttpStatusCode.Unauthorized);
                }

                var controllerInstance = CreateController(controllerType, request);

                if (controllerAction.ReturnType != httpResponseType)
                {
                    throw new InvalidOperationException($"Controller action '{path}' does not return HttpResponse.");
                }

                var parameterValues = GetParameterValues(controllerAction, request);

                return (HttpResponse)controllerAction.Invoke(controllerInstance, parameterValues);
            };
        }

        private static bool UserIsAuthorized(MethodInfo controllerAction,
            HttpSession session)
        {
            var authorization = controllerAction
                    .DeclaringType
                    .GetCustomAttribute<AuthorizeAttribute>()
                    ?? controllerAction.GetCustomAttribute<AuthorizeAttribute>();

            if (authorization != null)
            {
                var userIsAuthorized = session.Contains(Controller.UserSessionKey)
                    && session[Controller.UserSessionKey] != null;

                if (!userIsAuthorized)
                {
                    return false;
                }
            }

            return true;
        }
        private static object[] GetParameterValues(MethodInfo controllerAction, HttpRequest request)
        {
            var actionParameters = controllerAction
                            .GetParameters()
                            .Select(p => new
                            {
                                p.Name,
                                Type = p.ParameterType
                            })
                            .ToArray();

            var values = new object[actionParameters.Length];

            for (int i = 0; i < actionParameters.Length; i++)
            {
                var parameter = actionParameters[i];
                var parameterName = parameter.Name;
                var parameterType = parameter.Type;

                if(parameterType.IsPrimitive || parameterType == stringType)
                {
                    var parameterValue = request.GetValue(parameterName);
                    values[i] = Convert.ChangeType(parameterValue, parameterType);
                }
                else
                {
                    var parameterValue = Activator.CreateInstance(parameterType);
                    var parameterProperties = parameterType.GetProperties();

                    foreach (var prop in parameterProperties)
                    {
                        var propertyValue = Convert.ChangeType(request.GetValue(prop.Name), prop.PropertyType);
                        prop.SetValue(parameterValue, propertyValue);
                    }

                    values[i] = parameterValue;
                }
            }

            return values;
        }

        private static void MapDefaultRoutes(
            IRoutingTable routingTable,
            HttpMethod httpMethod,
            string controllerName, 
            string actionName, 
            Func<HttpRequest, HttpResponse> responseFunction)
        {
            if (actionName == defaultActionName)
            {
                routingTable.Map(httpMethod, $"/{controllerName}", responseFunction);

                if (controllerName == defaultControllerName)
                {
                    routingTable.Map(httpMethod, "/", responseFunction);
                }
            }
        }

        private static string GetValue(this HttpRequest request, string name)
            => request.Query.GetValueOrDefault(name) ?? request.Form.GetValueOrDefault(name);
    }
}

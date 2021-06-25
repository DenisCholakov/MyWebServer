using System;
using System.Collections.Generic;
using System.IO;
using MyWebServer.Server.Common;
using MyWebServer.Server.Http;

namespace MyWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<HttpMethod, Dictionary<string, Func<HttpRequest, HttpResponse>>> routes;

        public RoutingTable() => this.routes = new()
        {
            [HttpMethod.GET] = new(),
            [HttpMethod.POST] = new(),
            [HttpMethod.PUT] = new(),
            [HttpMethod.DELETE] = new()
        };

        public IRoutingTable Map(HttpMethod method, string path, HttpResponse response)
        {
            Guard.AgainstNull(response, nameof(response));

            return this.Map(method, path, request => response);
        }

        public IRoutingTable Map(HttpMethod method, string path, Func<HttpRequest, HttpResponse> responseFunction)
        {
            Guard.AgainstNull(path, nameof(path));
            Guard.AgainstNull(responseFunction, nameof(responseFunction));

            this.routes[method][path.ToLower()] = responseFunction;

            return this;
        }

        public IRoutingTable MapGet(string path, HttpResponse response)
            => MapGet(path, request => response);

        public IRoutingTable MapGet(string path, Func<HttpRequest, HttpResponse> responseFunction)
            => Map(HttpMethod.GET, path, responseFunction);

        public IRoutingTable MapPost(string path, HttpResponse response)
            => MapPost(path, request => response);

        public IRoutingTable MapPost(string path, Func<HttpRequest, HttpResponse> responseFunction)
            => Map(HttpMethod.POST, path, responseFunction);

        public HttpResponse ExecuteRequest(HttpRequest request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Path.ToLower();

            if (!this.routes.ContainsKey(requestMethod)
                    || !this.routes[requestMethod].ContainsKey(requestUrl))
            {
                return new HttpResponse(HttpStatusCode.NotFound);
            }

            var responseFunction = this.routes[requestMethod][requestUrl];

            return responseFunction(request);
        }

        public IRoutingTable MapStaticFiles(string folder = GlobalConstants.StaticFilesRootFolder)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var staticFilesFolder = Path.Combine(currentDirectory, folder);

            if (!Directory.Exists(staticFilesFolder))
            {
                return this;
            }

            var staticFiles = Directory.GetFiles(
                staticFilesFolder,
                "*.*",
                SearchOption.AllDirectories);

            foreach (var file in staticFiles)
            {
                var relativePath = Path.GetRelativePath(staticFilesFolder, file);
                var urlPath = "/" + relativePath.Replace("\\", "/");

                this.MapGet(urlPath, request =>
                {
                    var content = File.ReadAllBytes(file);
                    var fileExtension = Path.GetExtension(file).Trim('.');
                    var contentType = HttpContentType.GetByFileExtension(fileExtension);

                    return new HttpResponse(HttpStatusCode.Ok).SetContent(content, contentType);
                });
            }

            return this;
        }
    }
}

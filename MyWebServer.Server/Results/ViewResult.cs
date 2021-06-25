using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


using MyWebServer.Server.Http;
using MyWebServer.Server.Results.Views;

namespace MyWebServer.Server.Results
{
    public class ViewResult : ActionResult
    {
        private const char pathSeparator = '/';

        public ViewResult(HttpResponse response,
            IViewEngine viewEngine,
            string viewName,
            string controllerName,
            object model,
            string UserId) : base(response)
            => this.GetHtml(viewEngine, model, viewName, controllerName, UserId);

        private void GetHtml(IViewEngine viewEngine, object model, string viewName, string controllerName, string userId)
        {
            if (!viewName.Contains(pathSeparator))
            {
                viewName = controllerName + pathSeparator + viewName;
            }

            var viewPath = Path.GetFullPath(Directory.GetCurrentDirectory()
                + "/Views/" + viewName.TrimStart(pathSeparator) + ".cshtml");

            if (!File.Exists(viewPath))
            {
                this.PrepareMissingViewError(viewPath);

                return;
            }

            var viewContent = File.ReadAllText(viewPath);

            var layoutPath = Path.GetFullPath("./Views/Layout.cshtml");

            if (File.Exists(layoutPath))
            {
                var layoutContent = File.ReadAllText(layoutPath);
                viewContent = layoutContent.Replace("@RenderBody()", viewContent);
            }

            if (model != null)
            {
                viewContent = viewEngine.RenderHtml(viewContent, model, userId);
            }

            this.SetContent(viewContent, HttpContentType.Html);
        }

        private void PrepareMissingViewError(string viewPath)
        {
            this.StatusCode = HttpStatusCode.NotFound;

            var errorMessage = $"View '{viewPath}' was not found";

            this.SetContent(errorMessage, HttpContentType.PlainText);
        }
    }
}

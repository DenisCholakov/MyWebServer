using System.Runtime.CompilerServices;

using MyWebServer.Server.Http;
using MyWebServer.Server.Identity;
using MyWebServer.Server.Results;
using MyWebServer.Server.Results.Views;

namespace MyWebServer.Server.Controllers
{
    public abstract class Controller
    {
        public const string UserSessionKey = "AuthenticatedUserId";

        private UserIdentity userIdentity;
        private IViewEngine viewEngine;

        protected Controller()
        {
            this.Response = new HttpResponse(HttpStatusCode.Ok);
        }

        protected HttpRequest Request { get; init; }

        protected HttpResponse Response { get; private init; }

        protected UserIdentity User 
        { 
            get
            {
                if(this.userIdentity == null)
                {
                    this.userIdentity = this.Request.Session.Contains(UserSessionKey)
                        ? new UserIdentity { Id = this.Request.Session[UserSessionKey] }
                        : new();
                }

                return this.userIdentity;
            }
        }

        protected IViewEngine ViewEngine
        {
            get
            {
                if (this.viewEngine == null)
                {
                    this.viewEngine = this.Request.Services.Get<IViewEngine>()
                        ?? new ParserViewEngine();
                }

                return this.viewEngine;
            }
        }

        protected void SignIn(string userId)
        {
            this.Request.Session[UserSessionKey] = userId;
            this.userIdentity = new UserIdentity { Id = userId };
        }

        protected void SignOut()
        {
            this.Request.Session.Remove(UserSessionKey);
            this.userIdentity = new UserIdentity();
        }

        protected ActionResult Text(string text)
            => new TextResult(this.Response, text);

        protected ActionResult Html(string html)
            => new HtmlResult(this.Response, html);

        protected ActionResult Redirect(string location)
            => new RedirectResult(this.Response, location);

        protected ActionResult View([CallerMemberName] string viewName = "")
            => this.GetViewResult(viewName, null);

        protected ActionResult View(object model, [CallerMemberName] string viewName = "")
            => this.GetViewResult(viewName, model);

        private ActionResult GetViewResult(string viewName, object model)
            => new ViewResult(this.Response, this.ViewEngine, viewName, this.GetType().GetControllerName(), model, this.User.Id);
    }
}

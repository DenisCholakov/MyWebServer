using System;

using MyWebServer.Server.Controllers;
using MyWebServer.Server.Controllers.Attributes;
using MyWebServer.Server.Http;
using MyWebServer.Server.Results;

namespace MyWebServer.App.Controllers
{
    public class AccountController : Controller
    {
        public HttpResponse Login()
        {
            var someUserId = "MyUserId";
            this.SignIn(someUserId);

            return Text("User is authenticated!");
        }

        public HttpResponse Logout()
        {
            this.SignOut();

            return Text("User is signed out!");

        }

        public HttpResponse AuthenticatedCheck()
        {
            if (this.User.IsAuthenticated)
            {
                return Text($"Authenticated user: {this.User.Id}");
            }

            return Text("User is not authenticated.");
        }

        [Authorize]
        public HttpResponse AuthorizationCheck()
        {
            return Text($"Current user {this.User.Id}");
        }

        public ActionResult CookiesCheck()
        {
            const string cookieName = "My-cookie";

            if (this.Request.Cookies.Contains(cookieName))
            {
                var cookie = this.Request.Cookies[cookieName];

                return Text($"Cookies already exist - {cookie}");
            }

            this.Response.Cookies.Add(cookieName, "My-Value");
            this.Response.Cookies.Add("My-Second-Cookie", "My-Second-Value");

            return Text("Cookies set!");
        }

        public HttpResponse SessionCheck()
        {
            const string currentDateKey = "CurrentDate";

            if (this.Request.Session.Contains(currentDateKey))
            {
                var currentDate = this.Request.Session[currentDateKey];

                return Text($"Stored date: {currentDate}");
            }

            this.Request.Session[currentDateKey] = DateTime.UtcNow.ToString();

            return Text("DateStored");
        }
    }
}

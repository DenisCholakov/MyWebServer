using MyWebServer.Server.Http.Collections;
using MyWebServer.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyWebServer.Server.Http
{
    public class HttpRequest
    {
        private static Dictionary<string, HttpSession> Sessions { get; set; } = new();

        public HttpMethod Method { get; private set; }

        public string Path { get; private set; }

        public QueryCollection Query { get; private set; }

        public FormCollection Form { get; private set; }

        public HeaderCollection Headers { get; private set;  }

        public CookieCollection Cookies { get; private set; }

        public HttpSession Session { get; private set; }

        public IServiceCollection Services { get; private set; }

        public string Body { get; private set; }

        public static HttpRequest Parse(string request, IServiceCollection services)
        {
            var lines = request.Split(GlobalConstants.NewLine);

            var startLine = lines.First().Split(" ");

            var method = ParseMethod(startLine[0]);
            var url = startLine[1];

            var (path, query) = ParseUrl(url);

            var headers = ParseHeaders(lines.Skip(1));

            var cookies = ParseCookies(headers);

            var session = GetSession(cookies);

            var body = String.Join(GlobalConstants.NewLine, lines.Skip(headers.Count + 2));

            var form = ParseForm(headers, body);

            return new HttpRequest
            {
                Method = method,
                Path = path,
                Query = query,
                Headers = headers,
                Cookies = cookies,
                Session = session,
                Form = form,
                Body = body,
                Services = services
            };
        }

        private static HttpMethod ParseMethod(string method)
        {
            try
            {
                return (HttpMethod)Enum.Parse(typeof(HttpMethod), method.ToUpper());
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method {method} is invalid");
            }
        }

        private static (string Path, QueryCollection Qeury) ParseUrl(string url)
        {
            var urlParts = url.Split('?', 2);

            var path = urlParts[0];
            var query = urlParts.Length > 1 ? ParseQuery(urlParts[1]).ToQueryCollection()
                : new QueryCollection();

            return (path, query);
        }

        private static Dictionary<string, string> ParseQuery(string queryString)
        {
            return HttpUtility.UrlDecode(queryString)
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('='))
                .Where(part => part.Length == 2)
                .ToDictionary(p => p[0], p => p[1], StringComparer.InvariantCultureIgnoreCase);
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
        {
            var headers = new HeaderCollection();

            foreach (var headerLine in headerLines)
            {
                if (headerLine == String.Empty)
                {
                    break;
                }

                var headerParts = headerLine.Split(":", 2);


                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid.");
                }

                var headerName = headerParts[0];
                var headerValue = headerParts[1].Trim();

                headers.Add(headerName, headerValue);
            }

            return headers;
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookieCollection = new CookieCollection();

            if (headers.Contains(HttpHeader.Cookie))
            {
                var cookieHeader = headers[HttpHeader.Cookie];

                var allCookies =
                    cookieHeader.
                    Split(';').Select(c => c.Trim());

                foreach (var cookieText in allCookies)
                {
                    var cookiesParts = cookieText.Split("=");

                    var cookieName = cookiesParts[0];
                    var cookieValue = cookiesParts[1];

                    cookieCollection.Add(cookieName, cookieValue);
                }

            }
            return cookieCollection;
        }

        private static FormCollection ParseForm(HeaderCollection headers, string body)
        {
            var result = new FormCollection();

            if (headers.Contains(HttpHeader.ContentType)
                && headers[HttpHeader.ContentType] == HttpContentType.FormUrlEncoded)
            {
                result = ParseQuery(body).ToFormCollection();
            }

            return result;
        }
        private static HttpSession GetSession(CookieCollection cookies)
        {
            var sessionId = cookies.Contains(HttpSession.SessionCookieName)
                ? cookies[HttpSession.SessionCookieName]
                : Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(sessionId))
            {
                Sessions[sessionId] = new HttpSession(sessionId) { IsNew = true };
            }

            return Sessions[sessionId];
        }
    }
}

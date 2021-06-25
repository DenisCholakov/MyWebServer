using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MyWebServer.Server.Common;
using MyWebServer.Server.Http.Collections;

namespace MyWebServer.Server.Http
{
    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers.Add(HttpHeader.Server, "My Web Server");
            this.Headers.Add(HttpHeader.Date, $"{DateTime.UtcNow.ToString("r")}");
        }

        public HttpStatusCode StatusCode { get; protected set; }

        public HeaderCollection Headers { get; } = new();

        public CookieCollection Cookies { get; } = new();

        public byte[] Content { get; protected set; }

        public bool HasContent => this.Content != null && this.Content.Any();

        public HttpResponse SetContent(string content, string contentType)
        {
            Guard.AgainstNull(content, nameof(content));
            Guard.AgainstNull(contentType, nameof(contentType));

            var contentLengh = Encoding.UTF8.GetByteCount(content).ToString();

            this.Headers.Add(HttpHeader.ContentType, contentType);
            this.Headers.Add(HttpHeader.ContentLength, contentLengh);

            this.Content = Encoding.UTF8.GetBytes(content);

            return this;
        }

        public HttpResponse SetContent(byte[] content, string contentType)
        {
            Guard.AgainstNull(content, nameof(content));
            Guard.AgainstNull(contentType, nameof(contentType));

            var contentLengh = content.Length.ToString();

            this.Headers.Add(HttpHeader.ContentType, contentType);
            this.Headers.Add(HttpHeader.ContentLength, contentLengh);

            this.Content = content;

            return this;
        }

        public static HttpResponse CreateErrorResponse(string exceptioMessage)
        {
            var response = new HttpResponse(HttpStatusCode.Ok);

            return response.SetContent(exceptioMessage, HttpContentType.PlainText);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (var header in this.Headers)
            {
                result.AppendLine(header.ToString());
            }

            foreach (var cookie in this.Cookies)
            {
                result.AppendLine($"{HttpHeader.SetCookie}: {cookie}");
            }

            if (this.HasContent)
            {
                result.AppendLine();
            }
            

            return result.ToString();
        }
    }
}

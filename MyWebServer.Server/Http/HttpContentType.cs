using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Http
{
    public class HttpContentType
    {
        public const string PlainText = "text/plain; charset=UTF-8";
        public const string Html = "text/html; charset=UTF-8";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";

        public static string GetByFileExtension(string fileExtension)
            => fileExtension switch
            {
                "css" => "text/css",
                "js" => "application/javascript",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "ico" => "image/x-icon",
                _ => PlainText
            };
    }
}

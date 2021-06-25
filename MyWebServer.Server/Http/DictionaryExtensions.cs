using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server.Http.Collections;

namespace MyWebServer.Server.Http
{
    public static class DictionaryExtensions
    {
        public static FormCollection ToFormCollection(this Dictionary<string, string> dictionary)
        {
            var result = new FormCollection();

            foreach (var item in dictionary)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }

        public static QueryCollection ToQueryCollection(this Dictionary<string, string> dictionary)
        {
            var result = new QueryCollection();

            foreach (var item in dictionary)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }
}

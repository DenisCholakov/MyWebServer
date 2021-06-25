using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Http.Collections
{
    public class HeaderCollection : IEnumerable<HttpHeader>
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HeaderCollection()
        {
            this.headers = new (StringComparer.InvariantCultureIgnoreCase);
        }

        public int Count => this.headers.Count;

        public string this[string name] => this.headers[name].Value;

        public void Add(string name, string value) => this.headers[name] = new HttpHeader(name, value);

        public bool Contains(string name) => this.headers.ContainsKey(name);

        public IEnumerator<HttpHeader> GetEnumerator() => this.headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

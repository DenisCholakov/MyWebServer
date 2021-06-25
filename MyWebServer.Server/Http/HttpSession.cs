using MyWebServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Http
{
    public class HttpSession
    {
        public const string SessionCookieName = "MyWebServerSID";

        private Dictionary<string, string> data;

        public HttpSession(string id)
        {
            Guard.AgainstNull(id, nameof(id));

            this.Id = id;
            this.data = new();
        }
        public string Id { get; private init; }

        public bool IsNew { get; set; }

        public string this[string key]
        {
            get => this.data[key];
            set => this.data[key] = value;
        }

        public bool Contains(string key)
            => this.data.ContainsKey(key);

        public void Remove(string key)
        {
            if (this.data.ContainsKey(key))
            {
                this.data.Remove(key);
            }
        }
    }
}

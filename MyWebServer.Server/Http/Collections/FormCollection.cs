using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer.Server.Http.Collections
{
    public class FormCollection : IEnumerable<string>
    {
        private readonly Dictionary<string, string> form;

        public FormCollection()
        {
            this.form = new (StringComparer.InvariantCultureIgnoreCase);
        }

        public string this[string value] => this.form[value];

        public void Add(string name, string value) => this.form[name] = value;

        public bool Contains(string name) => this.form.ContainsKey(name);

        public string GetValueOrDefault(string key) => this.form.GetValueOrDefault(key);

        public IEnumerator<string> GetEnumerator() => this.form.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

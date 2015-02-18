using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public class QuerystringOverNvc : IQuerystring
    {
        private NameValueCollection nvc;
        public QuerystringOverNvc(NameValueCollection nvc)
        {
            this.nvc = nvc;
        }
        public IEnumerable<string> GetQueryValues(string key)
        {
            var value = GetQueryValue(key);
            return value == null ? Enumerable.Empty<string>() : Enumerable.Repeat<string>(value, 1);
        }

        public string GetQueryValue(string key)
        {
            return nvc.Get(key);
        }

        public IEnumerable<Tuple<string, string>> GetQueryPairs()
        {
            return nvc.Keys.Cast<string>().Select(k => new Tuple<string, string>(k, nvc[k])).ToArray();
        }
    }


    public class MutableQuerystringOverNvc : QuerystringOverNvc, IMutableQuerystring
    {
        private NameValueCollection nvc;
        public MutableQuerystringOverNvc(NameValueCollection nvc):base(nvc)
        {
            this.nvc = nvc;
        }

        public void SetQueryValue(string key, string newValue)
        {
            nvc[key] = newValue;
        }

        public void SetQueryPairs(IEnumerable<Tuple<string, string>> pairs)
        {
            nvc.Clear();
            foreach (var t in pairs)
                nvc.Add(t.Item1, t.Item2);
        }
    }
}

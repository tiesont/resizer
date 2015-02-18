using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public class QuerystringOverDictionary : IQuerystring
    {
        private Dictionary<string, string> dict;
        public QuerystringOverDictionary( Dictionary<string, string> dict)
        {
            this.dict = dict;
        }
        public IEnumerable<string> GetQueryValues(string key)
        {
            var value = GetQueryValue(key); 
            return value == null ? Enumerable.Empty<string>() : Enumerable.Repeat<string>(value,1);
        }

        public string GetQueryValue(string key)
        {
            string v = null;
            if (dict.TryGetValue(key, out v)) return v;
            else return null;
        }

        public IEnumerable<Tuple<string, string>> GetQueryPairs()
        {
            return dict.Select((p) => new Tuple<string, string>(p.Key, p.Value)).ToArray();
        }
    }


    public class MutableQuerystringOverDictionary : QuerystringOverDictionary, IMutableQuerystring
    {
        private Dictionary<string, string> dict;
        public MutableQuerystringOverDictionary(Dictionary<string, string> dict):base(dict)
        {
            this.dict = dict;
        }
    
    
        public void SetQueryValue(string key, string newValue)
        {
            dict[key] = newValue;
        }

        public void SetQueryPairs(IEnumerable<Tuple<string,string>> pairs)
        {
            dict.Clear();
            foreach (var t in pairs)
                dict.Add(t.Item1, t.Item2);
        }
    }


}

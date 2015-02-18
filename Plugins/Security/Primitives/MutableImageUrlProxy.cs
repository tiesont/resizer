using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{

    
    public class MutableImageUrlProxy : IMutableImageUrl
    {
        Func<string> getPath;
        Action<string> setPath;
        IMutableQuerystring q;
        public MutableImageUrlProxy(IMutableQuerystring query, Func<string> getPath, Action<string> setPath)
        {
            q = query;
            this.getPath = getPath;
            this.setPath = setPath;
        }
        public string GetPath()
        {
            return getPath();
        }

        public void SetPath(string newPath)
        {
            setPath(newPath);
        }

        public IEnumerable<string> GetQueryValues(string key)
        {
            return q.GetQueryValues(key);
        }

        public string GetQueryValue(string key)
        {
            return q.GetQueryValue(key);
        }

        public IEnumerable<Tuple<string, string>> GetQueryPairs()
        {
            return q.GetQueryPairs();
        }


        public void SetQueryValue(string key, string newValue)
        {
            q.SetQueryValue(key, newValue);
        }

        public void SetQueryPairs(IEnumerable<Tuple<string, string>> pairs)
        {
            q.SetQueryPairs(pairs);
        }
    }
}

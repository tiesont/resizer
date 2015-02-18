using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public interface IQuerystring
    {
        /// <summary>
        /// Enumerates the values of all pairs with the given querystring key. Key lookup should be case sensitive, Ordinal. Returns null if key doesn't exist. Returns an empty string if the key is used, but has a blank value.  Keys and values are in URL decoded form.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<string> GetQueryValues(string key);

        /// <summary>
        /// Returns either the first value associated with the pair, or a comma-delimited list. If you wish to handle duplicate keys properly, use GetQueryValues. Provided for performance. Returns null if key doesn't exist. Returns an empty string if the key is used, but has a blank value. Keys and values are in URL decoded form.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetQueryValue(string key);

        /// <summary>
        /// Returns the querystring in the form of string-string pairs. Keys are not guaranteed to be unique. Values may be empty strings, but should never be null. Keys and values are in URL decoded form.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<string, string>> GetQueryPairs();
    }
}

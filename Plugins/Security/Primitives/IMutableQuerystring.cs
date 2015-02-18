using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{

    public interface IMutableQuerystring : IQuerystring
    {

        /// <summary>
        /// To delete the entire pair, provide a value of null. 
        /// In the case of pairs with duplicate keys, only one pair will be retained and modified. Keys and values are in URL decoded form.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        void SetQueryValue(string key, string newValue);

        /// <summary>
        /// Replaces the entire querystring with the given set. Keys and values are in URL decoded form.
        /// </summary>
        /// <param name="pairs"></param>
        void SetQueryPairs(IEnumerable<Tuple<string, string>> pairs);
    }
}

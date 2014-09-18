using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public interface IImageUrl
    {
        /// <summary>
        /// Returns the full path, including PathInfo, up to and excluding the querystring. Includes the application mount path.
        /// </summary>
        /// <returns></returns>
        string GetPath();

        /// <summary>
        /// Enumerates the values of all pairs with the given querystring key. Key lookup should be case sensitive, Ordinal.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<string> GetQueryValues(string key);

        /// <summary>
        /// Returns the querystring in the form of string-string pairs. Keys are not guaranteed to be unique. 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<string, string>> GetQueryPairs();
    }

    public interface IMutableImageUrl : IImageUrl
    {
        /// <summary>
        /// Sets the path. 
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        string SetPath(string newPath);

        
        /// <summary>
        /// To delete the entire pair, provide a value of null. 
        /// In the case of pairs with duplicate keys, only one pair will be retained.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        string SetQueryValue(string key, string newValue);

        /// <summary>
        /// Replaces the entire querystring with the given set
        /// </summary>
        /// <param name="pairs"></param>
        void SetQueryPairs(IEnumerable<Tuple<string, string>> pairs);

    }
}

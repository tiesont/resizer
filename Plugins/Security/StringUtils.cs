using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    class StringUtils
    {

        public string UrlDecode(string value)
        {
            return Uri.UnescapeDataString(value.Replace('+', ' '));
        }
        public string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }
        [Flags()]
        public enum QueryParseOptions
        {
            None = 0,
            /// <summary>
            /// UTF-8 URL decode keys
            /// </summary>
            UrlDecodeKeys = 1, //0001
            /// <summary>
            /// UTF-8 URL decode values
            /// </summary>
            UrlDecodeValues = 2, //0010
            UrlDecodeAll = 1 & 2, //0011
            /// <summary>
            /// If a dictionary is returned, use StringComparator.OrdinalIgnoreCase
            /// </summary>
            CaseInsensitiveDictionary = 1 << 3, //0100
            /// <summary>
            /// Call .ToLowerInvariant() on keys after url decode
            /// </summary>
            LowercaseKeys = 1 << 4,
            /// <summary>
            /// Trim whitespace from keys after URL decode.
            /// </summary>
            TrimKeyWhitespace = 1 << 5,
            /// <summary>
            /// Support semicolons in addition to question marks and ampersands as valid pair delimiters
            /// </summary>
            AllowSemicolonDelimiters = 1 << 6,
            /// <summary>
            /// Merges duplicate key/value pairs by concatenating the values in a comma-delimited list.
            /// </summary>
            MergeDuplicateKeys = 1 << 7,
            /// <summary>
            /// Overwrite duplicate keys, last one wins.
            /// </summary>
            OverwriteDuplicateKeys = 1 << 8,
            /// <summary>
            /// Url decoding, case-insensitive dictionaries, key whitespace removal. 
            /// </summary>
            Default = UrlDecodeAll | CaseInsensitiveDictionary | TrimKeyWhitespace

        }
        /// <summary>
        /// If neither MergeDuplicateKeys or OverwriteDuplicateKeys is used, the first value per unique key will be retained.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Dictionary<string, string> ParseQueryToDict(string query, QueryParseOptions flags)
        {
            var dict = ParseQueryToMultiDict(query, flags);
            var data = new Dictionary<string, string>(flags.HasFlag(QueryParseOptions.CaseInsensitiveDictionary) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            foreach (var p in dict)
            {
                data.Add(p.Key, p.Value.First());
            }
            return data;
        }

        public Dictionary<string, List<string>> ParseQueryToMultiDict(string query, QueryParseOptions flags)
        {
            var caseInsensitiveDict = flags.HasFlag(QueryParseOptions.CaseInsensitiveDictionary);
            var mergeDuplicates = flags.HasFlag(QueryParseOptions.MergeDuplicateKeys);
            var overwriteDuplicates = flags.HasFlag(QueryParseOptions.OverwriteDuplicateKeys);
            if (mergeDuplicates && overwriteDuplicates) throw new ArgumentException("OverwriteDuplicateKeys and MergeDuplicateKeys cannot be used together", "flags");

            var list = ParseQueryToList(query, flags);
            var data = new Dictionary<string, List<string>>(caseInsensitiveDict ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            foreach (var t in list)
            {
                List<string> values = null;
                if (!data.ContainsKey(t.Item1))
                {
                    data[t.Item1] = values = new List<string>(1);
                }

                if (mergeDuplicates && values.Count > 0)
                {
                    values[0] = values[0] + "," + t.Item2;
                }
                else if (overwriteDuplicates && values.Count > 0)
                {
                    values[0] = t.Item2;
                }
                else
                {
                    values.Add(t.Item2);
                }
            }
            return data;
        }

        public List<Tuple<string, string>> ParseQueryToList(string query, QueryParseOptions flags)
        {
            var allowSemicolons = flags.HasFlag(QueryParseOptions.AllowSemicolonDelimiters);
            var lowercaseKeys = flags.HasFlag(QueryParseOptions.LowercaseKeys);
            var urlDecodeKeys = flags.HasFlag(QueryParseOptions.UrlDecodeKeys);
            var urlDecodeValues = flags.HasFlag(QueryParseOptions.UrlDecodeValues);
            var trimKeyWhitespace = flags.HasFlag(QueryParseOptions.TrimKeyWhitespace);
            //RemoveEmptyEntries is correct behavior to drop ?&key=value -> ?key=value, etc.
            string[] pairs = query.Split(allowSemicolons ? new char[] { '?', '&', ';' } : new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            var data = new List<Tuple<string, string>>(pairs.Length);
            foreach (string s in pairs)
            {
                int ix = s.IndexOf('=');
                string k, v = "";
                if (ix < 0)
                {
                    k = s;
                }
                else
                {
                    k = s.Substring(0, ix);
                    v = s.Substring(ix + 1);
                }
                if (urlDecodeKeys) k = UrlDecode(k);
                if (urlDecodeValues) v = UrlDecode(v);
                if (lowercaseKeys) k = k.ToLowerInvariant();
                if (trimKeyWhitespace) k = k.Trim();

                data.Add(new Tuple<string, string>(k, v));
            }
            return data;
        }
        [Flags()]
        public enum QuerySerializationOptions
        {
            UrlEncode = 3,
            UseSemicolons = 1 << 2,
            Default = UrlEncode
        }

        /// <summary>
        /// First parses the given querystring per HTML spec, but also supporting semicolons as a valid pair delimiter.
        /// Keys and values are UTF-8 URL decoded. Keys are trimmed of whitespace and lowercased (invariant).
        /// Pairs are sorted by key (ordinal comparison) and serialized. Duplicate keys retain their order.
        /// Returned querystring is ampersand delimited without a ? prefix. 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string CanonicalizeQuery(string query)
        {
            var data = ParseQueryToMultiDict(query, QueryParseOptions.Default |
                                            QueryParseOptions.AllowSemicolonDelimiters |
                                            QueryParseOptions.LowercaseKeys |
                                            QueryParseOptions.TrimKeyWhitespace);

            var keys = data.Keys.ToList();
            keys.Sort(StringComparer.Ordinal);

            var result = new StringBuilder(query.Length);

            for (var i = 0; i < keys.Count; i++)
            {
                string k = keys[i];
                var values = data[k];
                k = UrlEncode(k);
                if (i > 0) result.Append("&");
                foreach (string value in values)
                {
                    //TODO bug? on duplicate keys where is the & to separate?
                    result.Append(k);
                    result.Append("=");
                    result.Append(UrlEncode(value));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns a querystring minus the ?
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string BuildQuery(IQuerystring query)
        {
            var result = new StringBuilder();
            var pairs = query.GetQueryPairs().ToList();

            for (var i = 0; i < pairs.Count(); i++)
            {
                if (i > 0) result.Append("&");

                result.Append(UrlEncode(pairs[i].Item1));
                result.Append("=");
                result.Append(UrlEncode(pairs[i].Item2));

            }
            return result.ToString();
        }

    }
}

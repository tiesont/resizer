using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public static class IMutableImageUrlExtensionMethods
    {   
        public static string GetQueryValue(this IImageUrl url, string key){
            return String.Join(",", url.GetQueryValues(key).Where((s) => s != null));
        }

        /// <summary>
        /// Parses all values associated with the given key, splitting on the given delimiter and unescaping the values. Result may enclude empty (but not null) strings.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static IEnumerable<string> ParseList(this IImageUrl url, string key, string delimiter = ",")
        {
            var list = new List<string>();
            foreach (var value in url.GetQueryValues(key))
            {
                if (value == null) continue;

                list.AddRange(value.Split(',').Select((s) => Uri.UnescapeDataString(s)));
            }
            return list;
        }

        public static void SetList(this IMutableImageUrl url, string key, IEnumerable<string> values, string delimiter = ",")
        {
            url.SetQueryValue(key, String.Join(delimiter, values.Select((s) => Uri.EscapeDataString(s))));
        }
        public static IEnumerable<string> EnumerateAppliedPolicyNames(this IImageUrl url)
        {
            return url.ParseList("ri-policies").Where((s) => s != string.Empty);
        }

        public static bool HasPolicy(this IImageUrl url, string name)
        {
            return EnumerateAppliedPolicyNames(url).Any((s) => s.Equals(name, StringComparison.Ordinal));
        }
         
        public static void EnsurePolicyAdded(this IMutableImageUrl url, string policyName)
        {
            var policies = url.EnumerateAppliedPolicyNames();
            if (!policies.Contains(policyName))
            {
                url.SetList("ri-policies", policies.Concat(new[] { policyName }));
            }
        }

        public static void RemovePolicy(this IMutableImageUrl url, string policyName)
        {
            var policies = url.EnumerateAppliedPolicyNames();
            if (policies.Contains(policyName, StringComparer.Ordinal))
            {
                policies = policies.Where((s) => !s.Equals(policyName, StringComparison.Ordinal));
                if (policies.Count() > 0)
                    url.SetList("ri-policies", policies);
                else
                    url.SetQueryValue("ri-policies", null);
            }
        }

    }
}

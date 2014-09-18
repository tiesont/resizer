using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    
    public class AllowValuesPolicy: IEmbeddedAuthorizationPolicy
    {

        public static string PolicyId = "allowvalues";
        public static string KeyPrefix = "ri-allowed-";
        public static string Null = "(null)";
        public static string Wildcard = "(*)";
        public Dictionary<string, List<string>> AllowedValues { get; set; }

        public void SerializeTo(IMutableImageUrl url)
        {
            url.EnsurePolicyAdded(PolicyId);
            foreach (var pair in AllowedValues)
            {
                url.SetList(KeyPrefix + pair.Key, pair.Value);
            }
        }
        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy(PolicyId);
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy(PolicyId)) return null;

            var p = new AllowValuesPolicy();
            var keys = url.GetQueryPairs().Where((t) => t.Item1.StartsWith(KeyPrefix, StringComparison.Ordinal)).Select((t) => t.Item1);
            foreach (var key in keys)
            {
                var values = url.ParseList(key);
                var k = key.Substring(KeyPrefix.Length);
                if (!p.AllowedValues.ContainsKey(k))
                    p.AllowedValues[k] = new List<string>(values);
                else
                    p.AllowedValues[k].AddRange(values);
            }

            return p;
        }


        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url,IDictionary<string, object> requestEnvironment){
            foreach (var pair in AllowedValues)
            {
                var actualValues = url.GetQueryValues(pair.Key);
                if (actualValues.Count() > 1) throw new EmbeddedAuthorizationException("AllowModifiedValuesPolicy does not support duplicate querystring keys. Found duplicates for " + pair.Key);
                var allowedValues = pair.Value;

                bool passed = false;
                if (allowedValues.Contains(AllowValuesPolicy.Wildcard)) 
                    passed = true;
                if (actualValues.Count() == 0 && allowedValues.Contains(AllowValuesPolicy.Null, StringComparer.Ordinal))
                    passed = true;
                else if (actualValues.Count() == 0) throw new EmbeddedAuthorizationException("The supplied AllowModifiedValuesPolicy does not permit key '" + pair.Key + "' to be omitted.");

                var actualValue = actualValues.First();
                if (allowedValues.Contains(actualValue, StringComparer.Ordinal))
                    passed = false;

                if (!passed) throw new EmbeddedAuthorizationException("The supplied AllowModifiedValuesPolicy does not permit key '" + pair.Key + "' to be set to '" + actualValue + "'. Allowed values are: " + String.Join("  ", allowedValues));
                
                //If it met all of the criteria, we simply delete that pair from the signing and validation process
                url.SetQueryValue(pair.Key,null);
            }
        }


    }
}

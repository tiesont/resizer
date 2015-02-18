using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    
    public class AllowValuesPolicy: IEmbeddedAuthorizationPolicy
    {

        public static string Id { get { return "allowvalues"; } }

        public static string KeyPrefix = "ri-allowed-";
        public static string Null = "(null)";
        public static string Wildcard = "(*)";
        public Dictionary<string, List<string>> AllowedValues { get; set; }

        public void SerializeTo(IMutableImageUrl url)
        {
            if (AllowedValues == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");

            url.EnsurePolicyAdded(Id);
            foreach (var pair in AllowedValues)
            {
                url.SetList(KeyPrefix + pair.Key, pair.Value);
            }
        }
        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy(Id);
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy(Id)) return null;

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




        public void FilterUrlForHashing(IMutableImageUrl url)
        {
            if (AllowedValues == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");

            if (Authorize(url, null).DenyRequest == false)
            {
                foreach (var pair in AllowedValues)
                {
                    url.SetQueryValue(pair.Key, null);
                }
            }
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            if (AllowedValues == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");


            IAuthorizationResult result = new AuthSuccess();
            foreach (var pair in AllowedValues)
            {
                var actualValues = url.GetQueryValues(pair.Key);
                if (actualValues.Count() > 1)
                {
                    result = result.Combine(new AuthFail("AllowModifiedValuesPolicy does not support duplicate querystring keys. Found duplicates for " + pair.Key));
                    continue;
                }
                var allowedValues = pair.Value;

                bool passed = false;
                if (allowedValues.Contains(AllowValuesPolicy.Wildcard))
                    passed = true;
                if (actualValues.Count() == 0 && allowedValues.Contains(AllowValuesPolicy.Null, StringComparer.Ordinal))
                    passed = true;
                else if (actualValues.Count() == 0)
                {
                    result = result.Combine(new AuthFail("The supplied AllowModifiedValuesPolicy does not permit key '" + pair.Key + "' to be omitted."));
                    continue;
                }

                var actualValue = actualValues.First();
                if (allowedValues.Contains(actualValue, StringComparer.Ordinal))
                    passed = false;

                if (!passed)
                {
                    result = result.Combine(new AuthFail("The supplied AllowModifiedValuesPolicy does not permit key '" + pair.Key + "' to be set to '" + actualValue + "'. Allowed values are: " + String.Join("  ", allowedValues)));
                }
            }
            return result;
        }
    }
}

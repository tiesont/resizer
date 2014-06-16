using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageResizer.Plugins.Security
{

    //Encryption; transmit the salt, IV, and access id.
    //Signing; 
    class RequestSigning
    {



        public string SignRequest(string fullPath, string query, byte[] id, byte[] secret)
        {
            var normalizedQuery = new StringUtils().CanonicalizeQuery(query);
            var resource = fullPath + "?" + normalizedQuery;
            var resourceBytes = UnicodeEncoding.UTF8.GetBytes(resource);
            var resourceAndIdBytes = resourceBytes.Concat(id).ToArray();
            string signature = null;
            using (var hmac256 = HMACSHA256.Create())
            {
                var resourceAndIdHash = hmac256.ComputeHash(resourceAndIdBytes);
                var fullHash = hmac256.ComputeHash(secret.Concat(resourceAndIdHash).ToArray());
                signature = PathUtils.ToBase64U(id) + "|" + PathUtils.ToBase64U(fullHash);
            }
            return PathUtils.AddQueryString(fullPath + "?" + normalizedQuery, "ri-signature=" + new StringUtils().UrlEncode(signature));
        }

        public bool CheckSignature(string fullPath, string query, Func<byte[], byte[]> secretKeyProvider)
        {
            var sig = ExtractSignature(query);
            var secret = secretKeyProvider(sig.Item1);

            var normalizedQuery = new StringUtils().CanonicalizeQuery(query);
            var resource = fullPath + "?" + normalizedQuery;
            var resourceBytes = UnicodeEncoding.UTF8.GetBytes(resource);
            var resourceAndIdBytes = resourceBytes.Concat(sig.Item1).ToArray();
            byte[] correctHash = null;
            using (var hmac256 = HMACSHA256.Create())
            {
                var resourceAndIdHash = hmac256.ComputeHash(resourceAndIdBytes);
                correctHash = hmac256.ComputeHash(secret.Concat(resourceAndIdHash).ToArray());
            }

            return (correctHash.SequenceEqual(sig.Item2));
        }

        public Tuple<byte[], byte[]> ExtractSignature(string query)
        {
            var dict = new StringUtils().ParseQueryToDict(query, StringUtils.QueryParseOptions.Default |
                StringUtils.QueryParseOptions.AllowSemicolonDelimiters);
            string signature = null;
            if (dict.TryGetValue("ri-signature", out signature) && !string.IsNullOrEmpty(signature))
            {
                string[] parts = signature.Split('|');
                if (parts.Length == 2)
                {
                    return new Tuple<byte[], byte[]>(PathUtils.FromBase64UToBytes(parts[0]), PathUtils.FromBase64UToBytes(parts[1]));
                }
            }
            return null;
        }

    }
 
  
}

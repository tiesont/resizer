using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Cryptography
{
    public class RequestSigner
    {
        public static string SignRequest(string fullPath, string query, byte[] id, byte[] secret)
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

        public static bool CheckSignature(string fullPath, string query, Func<byte[], byte[]> secretKeyProvider)
        {
            var sig = ExtractSignature(query);
            if (sig == null) return false;
            var normalizedQuery = new StringUtils().CanonicalizeQuery(query);
            var secret = secretKeyProvider(sig.Item1);

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


        public static bool CheckSignature(string fullPath, IQuerystring query, Func<byte[], byte[]> secretKeyProvider)
        {
            var sig = ExtractSignature(query);
            if (sig == null) return false;
            var normalizedQuery = new StringUtils().CanonicalizeQuery(new StringUtils().BuildQuery(query));
            var secret = secretKeyProvider(sig.Item1);

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

        public static Tuple<byte[], byte[]> ExtractSignature(string query)
        {
            return ExtractSignature(new QuerystringOverDictionary(new StringUtils().ParseQueryToDict(query, StringUtils.QueryParseOptions.Default |
                StringUtils.QueryParseOptions.AllowSemicolonDelimiters)));
        }

        public static Tuple<byte[], byte[]> ExtractSignature(IQuerystring query)
        {
            string signature = query.GetQueryValue("ri-signature");
            if (!string.IsNullOrEmpty(signature))
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

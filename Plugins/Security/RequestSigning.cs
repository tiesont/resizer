using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using ImageResizer.Plugins.Security.Cryptography;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImageResizer.Plugins.Security.Authorization;

namespace ImageResizer.Plugins.Security
{

    public class RequestSigningPlugin: IssueSink, IPlugin
    {

        public RequestSigningPlugin(ISecretKeyProvider keyProvider)
            : base("RequestSigning plugin")
        {
            this.KeyProvider = KeyProvider;
        }

        public RequestSigningPlugin()
            : base("RequestSigning plugin")
        {
            this.KeyProvider = new AppDataKeyProvider();
            AddedByWebConfig = true;
        }

        public RequestSigningPlugin(NameValueCollection args) : this() { }

        public ISecretKeyProvider KeyProvider { get; set; }

        private bool AddedByWebConfig = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="query"></param>
        /// <param name="id"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
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



        Config c;
        public IPlugin Install(Config c)
        {
            this.c = c;
            c.Plugins.add_plugin(this);
            c.Pipeline.AuthorizeImage += Pipeline_AuthorizeImage;
            return this;
        }

        private IDictionary<string, IEmbeddedAuthorizationPolicy> GetDeserializers()
        {
            //TODO - we need to use some kind of extensible registry
            var deserializers = new Dictionary<string, IEmbeddedAuthorizationPolicy>(StringComparer.OrdinalIgnoreCase);
            deserializers[ReadOnlyPolicy.Id] = new ReadOnlyPolicy();
            deserializers[AllowValuesPolicy.Id] = new AllowValuesPolicy();
            deserializers[ExpirationPolicy.Id] = new ExpirationPolicy();
            deserializers[HttpsOnlyPolicy.Id] = new HttpsOnlyPolicy();
            deserializers[AnyQueryPolicy.Id] = new AnyQueryPolicy();
            deserializers[IPAddressPolicy.Id] = new IPAddressPolicy();
            return deserializers;
        }

        private IEnumerable<IEmbeddedAuthorizationPolicy> DeserializePolicies(IImageUrl mui)
        {
            //TODO - probably better to loop through all policies and attempt deserialization, rather than hardcode them to ids.
            var policies = new List<IEmbeddedAuthorizationPolicy>();
            var deserializers = GetDeserializers();

            foreach (var s in mui.EnumerateAppliedPolicyNames())
            {
                IEmbeddedAuthorizationPolicy policy;
                if (deserializers.TryGetValue(s, out policy))
                {
                    policies.Add(policy.DeserializeFrom(mui));
                }
                else
                {
                    //thow exception, unknown policy 's'
                    
                }
            }
            return policies;
        }

        void Pipeline_AuthorizeImage(IHttpModule sender, HttpContext context, IUrlAuthorizationEventArgs e)
        {
            //let's hope that the query isn't too scrambled, eh?
            if (string.IsNullOrEmpty(e.QueryString["ri-signature"])) return; //Not signed


            var str = PathUtils.BuildQueryString(e.QueryString);
            var mui = new MutableImageUrlProxy(new MutableQuerystringOverNvc(e.QueryString), () => e.VirtualPath, (v) => { });

            var env = new RequestEnvironmentContext(context);

            var policies = DeserializePolicies(mui);

            foreach (var p in policies)
                p.FilterUrlForHashing(mui);


            IAuthorizationResult result = new AuthSuccess();

            foreach (var p in policies)
                result = result.Combine(p.Authorize(mui, env));


            bool verified = CheckSignature(e.VirtualPath, str, (idbytes) =>
            {
                var key = "requestsigning_" + PathUtils.ToBase64U(idbytes);
                return this.KeyProvider.GetKey(key, 32);
            });

            if (!verified)
            {
                result = result.Combine(new AuthFail("The signature applied to this request is incorrect."));
            }


            if (result.DenyRequest)
            {
                e.AllowAccess = false;

            }
            
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }
    }
 
  
}

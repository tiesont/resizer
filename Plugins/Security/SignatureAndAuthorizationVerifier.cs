using ImageResizer.Plugins.Security.Authorization;
using ImageResizer.Plugins.Security.Cryptography;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public class SignatureAndAuthorizationVerifier
    {

        public static SignatureAndAuthorizationVerifier DefaultForTesting()
        {
            var available = new IEmbeddedAuthorizationPolicy[] { new ReadOnlyPolicy(), new AllowValuesPolicy(), new ExpirationPolicy(), new HttpsOnlyPolicy(), new AnyQueryPolicy(), new IPAddressPolicy() };
            var universal = new IEmbeddedAuthorizationPolicy[] { new ReadOnlyPolicy() };

            return new SignatureAndAuthorizationVerifier(available, universal, new AppDataKeyProvider());
        }
        public SignatureAndAuthorizationVerifier(IEnumerable<IEmbeddedAuthorizationPolicy> availablePolicies, IEnumerable<IEmbeddedAuthorizationPolicy> universalPolicies, ISecretKeyProvider keyProvider)
        {
            AvailablePolicies = availablePolicies;
            UniversalPolicies = universalPolicies;
            KeyProvider = keyProvider;
        }

        /// <summary>
        /// Instances provided to enable deserialization of policies in the URL
        /// </summary>
        public IEnumerable<IEmbeddedAuthorizationPolicy> AvailablePolicies { get; private set; }

        /// <summary>
        /// These policies are applied whether they're requested in the URL or not. Only Authorize is called, not Filter
        /// </summary>
        public IEnumerable<IEmbeddedAuthorizationPolicy> UniversalPolicies { get; private set; }

        public ISecretKeyProvider KeyProvider { get; private set; }

       
        private IEnumerable<IEmbeddedAuthorizationPolicy> DeserializePolicies(IImageUrl mui)
        {
            return AvailablePolicies.Select( d => d.DeserializeFrom(mui)).Where(p => p != null);

        }

        public IAuthorizationResult AuthorizeAndVerify(IRequestEnvironment requestContext, IMutableImageUrl url)
        {
            var policies = DeserializePolicies(url);
            foreach (var p in policies)
                p.FilterUrlForHashing(url);


            IAuthorizationResult result = new AuthSuccess();

            foreach (var p in policies)
                result = result.Combine(p.Authorize(url, requestContext));


            bool verified = RequestSigner.CheckSignature(url.GetPath(), url, (idbytes) =>
            {
                var key = "requestsigning_" + PathUtils.ToBase64U(idbytes);
                return this.KeyProvider.GetKey(key, 32);
            });

            if (!verified)
            {
                result = result.Combine(new AuthFail("The signature applied to this request is incorrect."));
            }

            return result;

        }

     
    }
}

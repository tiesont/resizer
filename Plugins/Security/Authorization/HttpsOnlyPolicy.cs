using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class HttpsOnlyPolicy:IEmbeddedAuthorizationPolicy
    {
        public void SerializeTo(IMutableImageUrl url)
        {
            url.EnsurePolicyAdded("https");
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            return url.HasPolicy("https") ? new HttpsOnlyPolicy() : null;
        }

        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy("https");
        }

        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url, IDictionary<string, object> requestEnvironment)
        {
            if (requestEnvironment != null)
            {
                var scheme = requestEnvironment["owin.RequestScheme"] as string;
                if (!scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                    throw new EmbeddedAuthorizationException("This signature is only valid over an HTTPS connection");
            }
        }
    }
}

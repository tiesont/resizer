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

        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url, IRequestEnvironment env)
        {
            if (env != null)
            {
                bool? isHttps = null;
                if (env.HasOwin)
                {
                    var scheme = env.GetOwin()["owin.RequestScheme"] as string;
                    if (scheme != null)
                    {
                        isHttps = scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
                    }
                }
                if (env.HasContext && isHttps == null)
                {
                    dynamic context = env.GetConext();
                    dynamic request = context.Request;
                    isHttps = context.Request.IsSecureConnection;
                }
                if (!isHttps.HasValue)
                    throw new EmbeddedAuthorizationException("Failed to acquire information from the request environment about whether HTTPS was used. Please file a bug.");
                if (!isHttps.Value)
                    throw new EmbeddedAuthorizationException("This signature is only valid over an HTTPS connection");
            }
        }
    }
}

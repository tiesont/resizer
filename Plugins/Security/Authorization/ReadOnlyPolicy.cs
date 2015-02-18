using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    /// <summary>
    /// Doesn't even make sense unless we support POST/PUT/PATCH or something.
    /// </summary>
     class ReadOnlyPolicy:IEmbeddedAuthorizationPolicy
    {

        public void SerializeTo(IMutableImageUrl url)
        {
 	        url.EnsurePolicyAdded("read");
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
 	        return url.HasPolicy("read") ? new ReadOnlyPolicy() : null;
        }

        public void RemoveFrom(IMutableImageUrl url)
        {
 	        url.RemovePolicy("read");
        }



        public void FilterUrlForHashing(IMutableImageUrl url)
        {
           
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            string httpMethod = null;
            if (env.HasOwin)
            {
                httpMethod = env.GetOwin()["owin.RequestMethod"] as string;
            }
            if (httpMethod == null && env.HasContext)
            {
                httpMethod = ((dynamic)env.GetConext()).Request.HttpMethod;
            }
            if (httpMethod == null)
            {
                throw new EmbeddedAuthorizationException("Failed to access HTTP method from request environment. Please file a bug, with details about your server and configuration.");
            }
            if (!"GET".Equals(httpMethod, StringComparison.OrdinalIgnoreCase) &&
                !"HEAD".Equals(httpMethod, StringComparison.OrdinalIgnoreCase))
            {
                return new AuthFail("Only GET and HEAD HTTP requests permitted when ReadOnlyPolicy is applied.");
            }
            return AuthSuccess.Instance;
        }
    }
}

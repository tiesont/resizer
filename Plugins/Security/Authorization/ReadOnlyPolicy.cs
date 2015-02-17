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

        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url, IDictionary<string,object> requestEnvironment)
        {
            
            var isRead = requestEnvironment == null || "GET".Equals(requestEnvironment["owin.RequestMethod"] as string, StringComparison.OrdinalIgnoreCase) ||
                        "HEAD".Equals(requestEnvironment["owin.RequestMethod"] as string, StringComparison.OrdinalIgnoreCase);
            if (!isRead) throw new EmbeddedAuthorizationException("Only GET and HEAD HTTP requests permitted when ReadOnlyPolicy is applied.");
        }
    }
}

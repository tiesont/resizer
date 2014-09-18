using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class ExpirationPolicy:IEmbeddedAuthorizationPolicy
    {
        public ExpirationPolicy(DateTime accessExpiresUtc)
        {
            AccessExpires = accessExpiresUtc;
        }
        public DateTime AccessExpires;
        public void SerializeTo(IMutableImageUrl url)
        {
            url.EnsurePolicyAdded("expires");
            url.SetQueryValue("ri-expires", (AccessExpires.ToUniversalTime() - Epoch).TotalSeconds.ToString());
        }

        private DateTime Epoch { get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); } }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy("expires")) return null;
            string val = url.GetQueryValue("ri-expires");
            int seconds;
            if (int.TryParse(val,out seconds)){
                return new ExpirationPolicy(Epoch.AddSeconds(seconds));
            }
            else
            {
                throw new EmbeddedAuthorizationException("Failed to parse expiration time; given '" + val + "', expected seconds since unix epoch");
            }
            
        }
        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url, IDictionary<string, object> requestEnvironment)
        {
            if (AccessExpires < DateTime.UtcNow) throw new EmbeddedAuthorizationException("This URL has expired");
        }




        public void RemoveFrom(IMutableImageUrl url)
        {
            url.SetQueryValue("ri-expires", null);
            url.RemovePolicy("expires");
        }
    }
}

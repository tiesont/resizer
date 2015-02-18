using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class ExpirationPolicy:IEmbeddedAuthorizationPolicy
    {
        public static string Id { get { return "expires"; } }

        public ExpirationPolicy(DateTime accessExpiresUtc)
        {
            AccessExpires = accessExpiresUtc;
        }
        /// <summary>
        /// Warning! The parameterless constructor produces an instance that can only be used for calling DeserializeFrom()
        /// </summary>
        public ExpirationPolicy()
        {
            AccessExpires = null;
        }

        public DateTime? AccessExpires;
        public void SerializeTo(IMutableImageUrl url)
        {
            if (AccessExpires == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");
            url.EnsurePolicyAdded(Id);
            url.SetQueryValue("ri-expires", (AccessExpires.Value.ToUniversalTime() - Epoch).TotalSeconds.ToString());
        }

        private DateTime Epoch { get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); } }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy(Id)) return null;
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
  


        public void RemoveFrom(IMutableImageUrl url)
        {
            url.SetQueryValue("ri-expires", null);
            url.RemovePolicy(Id);
        }


        public void FilterUrlForHashing(IMutableImageUrl url)
        {
            
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            if (AccessExpires == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");

            if (AccessExpires < DateTime.UtcNow) return new AuthFail("This URL has expired.");
            else return AuthSuccess.Instance;
        }
    }
}

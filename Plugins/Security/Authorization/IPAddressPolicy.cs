using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class IPAddressPolicy :IEmbeddedAuthorizationPolicy
    {
        public IPAddressPolicy(string allowedIAddress)
        {
            allowedAddress = allowedIAddress;
        }

        private string allowedAddress;

        public void SerializeTo(IMutableImageUrl url)
        {
            url.EnsurePolicyAdded("ipaddr");
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy("ipaddr")) return null;
            string addr = url.GetQueryValue("ri-ipaddr-only");
            if (string.IsNullOrEmpty(addr)) return null;
            return new IPAddressPolicy(addr);
        }



        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy("ipaddr");
            url.SetQueryValue("ri-ipaddr-only",null);
        }


        public void FilterUrlForHashing(IMutableImageUrl url)
        {
            
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            string remoteIP = null;
            if (env.HasOwin)
            {
                remoteIP = env.GetOwin()["server.RemoteIpAddress"] as string;
            }
            if (string.IsNullOrEmpty(remoteIP) && env.HasContext)
            {
                dynamic context = env.GetConext();
                dynamic request = context.Request;
                remoteIP = context.Request.UserHostAddress; //IsSecureConnection
            }
            if (remoteIP == null)
            {
                //TODO: should this be considered a bug, and throw an exception? 
                return new AuthFail("Signed request only valid from client " + allowedAddress + ". No client IP address found associated with request environment. ");
            }
            if (!remoteIP.Equals(allowedAddress))
            {
                return new AuthFail("Signed request only valid from client " + allowedAddress + ", but arrived from " + remoteIP);
            }
            return AuthSuccess.Instance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class IPAddressPolicy :IEmbeddedAuthorizationPolicy
    {
        public static string Id { get { return "ipaddr"; } }

        /// <summary>
        /// Warning! The parameterless constructor produces an instance that can only be used for calling DeserializeFrom()
        /// </summary>
        public IPAddressPolicy()
        {

        }
        public IPAddressPolicy(string allowedIAddress)
        {
            allowedAddress = allowedIAddress;
        }

        private string allowedAddress = null;

        public void SerializeTo(IMutableImageUrl url)
        {
            if (allowedAddress == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");
            url.EnsurePolicyAdded(Id);
            url.SetQueryValue("ri-ipaddr-only", allowedAddress);
        }

        public IEmbeddedAuthorizationPolicy DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy(Id)) return null;
            string addr = url.GetQueryValue("ri-ipaddr-only");
            if (string.IsNullOrEmpty(addr)) return null;
            return new IPAddressPolicy(addr);
        }



        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy(Id);
            url.SetQueryValue("ri-ipaddr-only",null);
        }


        public void FilterUrlForHashing(IMutableImageUrl url)
        {
            
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            if (allowedAddress == null) throw new InvalidOperationException("This policy has not been configured. It may only be used to deserialize new policies.");

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

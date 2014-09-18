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

        public void ValidateAndFilterUrlForHashing(IMutableImageUrl url, IDictionary<string, object> requestEnvironment)
        {
            if (requestEnvironment != null){
                var remoteIP = requestEnvironment["server.RemoteIpAddress"];
                //var headers = requestEnvironment["owin.RequestHeaders"] as IDictionary<string, string[]>;
                if (!remoteIP.Equals(allowedAddress))
                    throw new EmbeddedAuthorizationException("Signed request only valid from client " + allowedAddress + ", but arrived from " + remoteIP);
            }
        }



        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy("ipaddr");
            url.SetQueryValue("ri-ipaddr-only",null);
        }
    }
}

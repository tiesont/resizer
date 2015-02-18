using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    public class AnyQueryPolicy: IEmbeddedAuthorizationPolicy
    {
        public void SerializeTo(IMutableImageUrl url)
        {
            url.EnsurePolicyAdded("anyquery");
        }

        public IEmbeddedAuthorizationPolicy  DeserializeFrom(IImageUrl url)
        {
            if (!url.HasPolicy("anyquery")) return null;
            return new AnyQueryPolicy();
            

        }
  

        public void RemoveFrom(IMutableImageUrl url)
        {
            url.RemovePolicy("anyquery");
        }


        public void FilterUrlForHashing(IMutableImageUrl url)
        {
            url.SetQueryPairs(new List<Tuple<string, string>>());
        }

        public IAuthorizationResult Authorize(IImageUrl url, IRequestEnvironment env)
        {
            return new AuthSuccess();
        }
    }
}

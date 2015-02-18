using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
  
    public class AuthSuccess : IAuthorizationResult
    {

        public bool DenyRequest
        {
            get { return false; }
        }

        public IEnumerable<string> Reasons
        {
            get { return Enumerable.Empty<string>();  }
        }

        static volatile AuthSuccess instance;
        public static AuthSuccess Instance
        {
            get{
                if (instance == null) instance = new AuthSuccess();
                return instance;
            }
        }
    }

   

    public  class AuthFail : IAuthorizationResult
    {
        public AuthFail(params string[] reasons)
        {
            Reasons = reasons ?? Enumerable.Empty<string>();
        }
        public AuthFail(IEnumerable<string> reasons)
        {
            Reasons = reasons ?? Enumerable.Empty<string>();
        }

        public bool DenyRequest
        {
            get { return true; }
        }

        public IEnumerable<string> Reasons { get; private set; }
    }

    public static class AuthExtensions
    {
        public static IAuthorizationResult Combine(this IAuthorizationResult a, IAuthorizationResult b)
        {
            if (a.DenyRequest || b.DenyRequest)
            {
                return new AuthFail((a.DenyRequest && !b.DenyRequest) ? a.Reasons : (b.DenyRequest && !a.DenyRequest) ? b.Reasons : Enumerable.Concat<string>(a.Reasons, b.Reasons));
            }
            else
            {
                return new AuthSuccess();
            }
        }
    }
}

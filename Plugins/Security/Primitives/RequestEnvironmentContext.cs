using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    public class RequestEnvironmentContext:IRequestEnvironment
    {

        object context;
        public RequestEnvironmentContext(object httpContextBase)
        {
            this.context = httpContextBase;
        }
        public bool HasOwin
        {
            get { return false; }
        }

        public bool HasContext
        {
            get { return true; }
        }

        public bool OwinIsNative
        {
            get { return false; }
        }

        public bool ContextIsNative
        {
            get { return true; }
        }

        public IDictionary<string, object> GetOwin()
        {
            return null;
        }

        public object GetConext()
        {
            return context;
        }
    }
}

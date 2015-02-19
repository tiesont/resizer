using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using ImageResizer.Plugins.Security.Cryptography;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImageResizer.Plugins.Security.Authorization;

namespace ImageResizer.Plugins.Security
{

    public class RequestSigningPlugin: IssueSink, IPlugin
    {

        public RequestSigningPlugin(SignatureAndAuthorizationVerifier verifier)
            : base("RequestSigning plugin")
        {
            this.Verifier = verifier;
        }

        public RequestSigningPlugin()
            : base("RequestSigning plugin")
        {
            this.Verifier = SignatureAndAuthorizationVerifier.DefaultForTesting();
            AddedByWebConfig = true;
        }

        public RequestSigningPlugin(NameValueCollection args) : this() { }

        public SignatureAndAuthorizationVerifier Verifier { get; set; }

        private bool AddedByWebConfig = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="query"></param>
        /// <param name="id"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
     



        Config c;
        public IPlugin Install(Config c)
        {
            this.c = c;
            c.Plugins.add_plugin(this);
            c.Pipeline.AuthorizeImage += Pipeline_AuthorizeImage;
            return this;
        }

    
        void Pipeline_AuthorizeImage(IHttpModule sender, HttpContext context, IUrlAuthorizationEventArgs e)
        {
            //let's hope that the query isn't too scrambled, eh?
            if (string.IsNullOrEmpty(e.QueryString["ri-signature"])) return; //Not signed


            var str = PathUtils.BuildQueryString(e.QueryString);
            var mui = new MutableImageUrlProxy(new MutableQuerystringOverNvc(e.QueryString), () => e.VirtualPath, (v) => { });

            var env = new RequestEnvironmentContext(context);

            if (Verifier.AuthorizeAndVerify(env, mui).DenyRequest)
            {
                e.AllowAccess = false;

            }
            
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }
    }
 
  
}

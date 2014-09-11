using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.UrlRewriter
{
    public class UrlRewriterPlugin: IPlugin
    {
        public IPlugin Install(Configuration.Config c)
        {
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Configuration.Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }


    }
    //Use cases:
    // 1. Map one prefix to another. Support ~ expansion on both. Select between ProcessedFiles and AllRequests
    // 2. 
    // Apply default settings to everything with a prefix
    //
    // Map .ashx requests back to the original file. Requires 'HasDirectives" support and RemoveSuffix
    // Set image defaults?


    //Match path prefix
    //Match regex
    //Match domain?
    //Match querystring
    //PostAuthorizeRequest or BeginRequest?
    //HasImageExtension
    //AllowFakeExtension
    //

    public interface IUrlRewriteCondition { }
    public interface IUrlRewriteAction { }

}

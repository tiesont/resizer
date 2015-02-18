using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{

    public interface IImageUrl : IQuerystring
    {
        /// <summary>
        /// Returns the full path, including PathInfo, up to and excluding the querystring. Includes the application mount path.
        /// </summary>
        /// <returns></returns>
        string GetPath();

    }
}

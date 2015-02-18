using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{



    public interface IMutableImageUrl : IImageUrl, IMutableQuerystring
    {
        /// <summary>
        /// Sets the path. 
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        void SetPath(string newPath);


    }
}

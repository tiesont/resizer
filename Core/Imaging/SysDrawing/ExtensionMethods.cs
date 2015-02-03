using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging.SysDrawing
{
    public class ExtensionMethods
    {
        public static byte[] ToBgra(this Color c)
        {
            return new byte[] { c.B, c.G, c.R, c.A };
        }



        public static IPixelFormat GetStandardPixelFormat(this Bitmap b)
        {
            return CommonPixelFormats.Format24bppBGR;
        }

        public static PixelFormat ToSysDrawingPixelFormat(this IPixelFormat f)
        {
            return PixelFormat.DontCare;//TODO, implement this
        }
        

    }
}

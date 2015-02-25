using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging.SysDrawing
{
    public static class ExtensionMethods
    {
        public static byte[] ToBgra(this Color c)
        {
            return new byte[] { c.B, c.G, c.R, c.A };
        }



        public static IPixelFormat GetStandardPixelFormat(this Bitmap b)
        {
            switch(b.PixelFormat){
                case PixelFormat.Format32bppArgb:
                    return new StandardPixelFormat( BitmapPixelFormats.Argb32b, BitmapPixelFormats.Bgra32b,32);

            }
            return new StandardPixelFormat(BitmapPixelFormats.None,4);
        }

        public static PixelFormat ToSysDrawingPixelFormat(this IPixelFormat f)
        {
            return PixelFormat.DontCare;//TODO, implement this
        }
        

    }
}

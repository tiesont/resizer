using ImageResizer.Plugins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class SysDrawingOperators
    {
          private IBitmapRegion r;

          public SysDrawingOperators(IBitmapRegion target)
          {
              r = target;
          }

          public  Bitmap WrapWithGdiBitmap()
          {
              return new Bitmap(r.Width, r.Height, r.Stride, SysDrawingPixelFormat(), r.Byte0);
          }

          public PixelFormat SysDrawingPixelFormat()
          {

              if (r.Format.BitwiseFormat == BitmapPixelFormats.Indexed8b) return PixelFormat.Format8bppIndexed;
              
              return PixelFormat.Undefined;
          }
    }
}

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
              r.Format.
              if (r.Format == BitmapPixelFormat.Indexed8) return PixelFormat.Format8bppIndexed;
              if (r.PixelFormat == BitmapPixelFormat.Gray8) return PixelFormat.Format8bppIndexed;
              if (r.PixelFormat == BitmapPixelFormat.Bgr24) return PixelFormat.Format24bppRgb;

              if (r.PixelFormat == BitmapPixelFormat.Bgra32)
              {
                  if (r.RespectAlpha) return PixelFormat.Format32bppArgb;
                  else return PixelFormat.Format32bppRgb;
              }
              return PixelFormat.Undefined;
          }
    }
}

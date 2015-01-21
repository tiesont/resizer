using ImageResizer.Plugins;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class ManagedOperators
    {
        private IBitmapRegion r;

        public ManagedOperators(IBitmapRegion target)
        {
            r = target;
        }

        public void FillRectangle(int x, int y, int w, int h, byte[] color)
        {
            //TODO: check bounds
            for (int j = y; j < y + h; j++)
                for (int i = x; i < x + w; i++)
                {
                    var pixel = r.Pixel0 + (j * r.Stride) + (x * r.BytesPerPixel);
                    Marshal.Copy(color, 0, pixel, color.Length);
                }
        }

        public bool RectangleIsColor(int x, int y, int w, int h, byte[] color)
        {
            byte[] compare = new byte[color.Length];
            //TODO: check bounds
            for (int j = y; j < y + h; j++)
                for (int i = x; i < x + w; i++)
                {
                    var pixel = r.Pixel0 + (j * r.Stride) + (x * r.BytesPerPixel);
                    Marshal.Copy(pixel, compare,0, color.Length);
                    if (!compare.SequenceEqual(color))
                    {
                        return false;
                    }
                }
            return true;
        }

    }
}

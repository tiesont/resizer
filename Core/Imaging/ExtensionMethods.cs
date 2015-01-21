using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public static class ExtensionMethods
    {
        public static void ApplyToRegion(this IBitmapFrame frame, int x, int y, int w, int h, Action<IBitmapRegion> action)
        {
            var region = frame.LockRegion(x, y, w, h);
            try
            {
                action(region);
            }
            finally
            {
                frame.UnlockRegion(region);
                if (!region.Disposed) region.Dispose();
            }
        }
        public static void ApplyToFrame(this IBitmapFrame frame,Action<IBitmapRegion> action)
        {
            ApplyToRegion(frame, 0, 0, frame.Width, frame.Height, action);
        }

        public static IBitmapRegion LockFrame(this IBitmapFrame frame)
        {
            return frame.LockRegion(0, 0, frame.Width, frame.Height);
        }

        public static ManagedOperators Managed(this IBitmapRegion r)
        {
            return new ManagedOperators(r);
        }

        public static SysDrawingOperators SysDrawing(this IBitmapRegion r)
        {
            return new SysDrawingOperators(r);
        }

        public static byte[] ToBgra(this Color c)
        {
            return new byte[] { c.B, c.G, c.R, c.A };

        }
    }
}

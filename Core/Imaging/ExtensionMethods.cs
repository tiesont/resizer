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
        public static void Unreference(this ITrackable t)
        {
            t.TrackingScope.Unreference(t);
        }
        public static void ApplyToRegion(this IBitmapFrame frame, int x, int y, int w, int h, Action<IBitmapRegion> action)
        {
            var region = frame.OpenRegion(x, y, w, h, RegionAccessMode.ReadWrite);
            try
            {
                action(region);
            }
            finally
            {
                region.Unreference();
            }
        }
        public static void ApplyToFrame(this IBitmapFrame frame,Action<IBitmapRegion> action)
        {
            ApplyToRegion(frame, 0, 0, frame.Width, frame.Height, action);
        }

        public static IBitmapRegion LockFrame(this IBitmapFrame frame)
        {
            return frame.OpenRegion(0, 0, frame.Width, frame.Height, RegionAccessMode.ReadWrite);
        }

        public static ManagedOperators Managed(this IBitmapRegion r)
        {
            return new ManagedOperators(r);
        }

        public static SysDrawingOperators SysDrawing(this IBitmapRegion r)
        {
            return new SysDrawingOperators(r);
        }


    }
}

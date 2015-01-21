using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class SysDrawingFrame:IBitmapFrame
    {
        private Bitmap b;
        public int Width
        {
            get { return b.Width; }
        }

        public int Height
        {
            get { return b.Height; }
        }

        public IBitmapRegion LockRegion(int x, int y, int w, int h)
        {
            return SysDrawingRegion.WindowInto(b, new Rectangle(x, y, w, h));
        }

        public void UnlockRegion(IBitmapRegion region)
        {
            region.Dispose();
        }
    }
}

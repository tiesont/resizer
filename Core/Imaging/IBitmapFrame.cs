using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface IBitmapFrame
    {
        int Width { get; }
        int Height { get; }
            
        IBitmapRegion LockRegion(int x, int y, int w, int h);

        void UnlockRegion(IBitmapRegion region);

    }
}

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

        private SysDrawingContainer parent;
        protected SysDrawingContainer ParentBitmap { get { return parent; } }
        public SysDrawingFrame(SysDrawingContainer parent)
        {
            this.parent = parent;
            parent.TrackingScope.TrackDependency(parent, this);
            IsDisposed = false;
            Width = parent.DangerousCurrentBitmap.Width;
            Height = parent.DangerousCurrentBitmap.Height;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Guid PixelFormatId { get; private set; }

        public IBitmapRegion OpenRegion(int x, int y, int w, int h)
        {
            return SysDrawingRegion.WindowInto(parent.DangerousCurrentBitmap, new Rectangle(x, y, w, h));
        }


        

        public IGraphicsHints Hints
        {
            get { throw new NotImplementedException(); }
        }

        public ITrackingScope TrackingScope { get; set; }

        public bool IsDisposed { get; private set; }

        public void ReleaseResources()
        {
            IsDisposed = true;
        }

        public bool HasLockedBits { get; }

        public IPixelFormat PixelFormat
        {
            get { throw new NotImplementedException(); }
        }

        public IBitmapRegion OpenRegion(int x, int y, int w, int h, RegionAccessMode accessMode)
        {
            throw new NotImplementedException();
        }

        public bool CanOpenRegion
        {
            get { throw new NotImplementedException(); }
        }
    }
}

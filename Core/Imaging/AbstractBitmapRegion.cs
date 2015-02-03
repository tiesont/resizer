using ImageResizer.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public abstract class AbstractBitmapRegion: IBitmapRegion
    {
        
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int Stride { get; protected set; }
        public IntPtr Byte0 { get; protected set; }
  

        public bool PixelsWriteable { get; protected set; }

        public bool PaddingWriteable { get; protected set; }



        public long ByteCount { get; protected set; }

        public int BytesPerPixel { get; protected set; }

        public ITrackingScope TrackingScope { get; set; }

        public abstract void ReleaseResources();


        public IPixelFormat Format { get; protected set; }


        protected bool Changed { get; set; }
        public void MarkChanged()
        {
            Changed = true;
        }

        public abstract void Close();
    }
}

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
        protected AbstractBitmapRegion()
        {
            ReliantOn = Enumerable.Empty<object>();
            OnDispose = new Stack<Action>();
        }


        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int Stride { get; protected set; }
        public IntPtr Pixel0 { get; protected set; }
        public virtual int BytesPerPixel
        {
            get
            {
                var fmt = PixelFormat;
                return (fmt == BitmapPixelFormat.Bgr24) ? 3 :
                  (fmt == BitmapPixelFormat.Bgra32) ? 4 :
                  (fmt == BitmapPixelFormat.Gray8 || fmt == BitmapPixelFormat.Indexed8) ? 1 : 0;

            }
        }
        public bool RespectAlpha { get; protected set; }
        public void MarkAlphaUsed()
        {
            if (!PixelsWriteable) throw new InvalidOperationException();
            RespectAlpha = true;
            
        }

        public virtual bool AlphaPossible
        {
            get
            {
                return PixelFormat == BitmapPixelFormat.Bgra32 || PixelFormat == BitmapPixelFormat.Indexed8;
            }
        }

        public bool PixelsWriteable { get; protected set; }

        public virtual void MarkReadonly()
        {
            PixelsWriteable = false;
        }

        public bool PaddingWriteable { get; protected set; }

        public BitmapPixelFormat PixelFormat { get; protected set; }

        public BitmapCompositingMode Compositing { get; set; }


        protected byte[] matte = null;
        public byte[] GetMatte()
        {
            return matte;
        }

        public virtual void SetMatte(byte[] color)
        {
            if (color != null && color.Length != this.BytesPerPixel) throw new ArgumentOutOfRangeException("Matte color bytes size must match BytesPerPixel");
            matte = color;
        }

        public bool Disposed { get; protected set; }



        public IEnumerable<object> ReliantOn { get; protected set; }

        public Stack<Action> OnDispose { get; protected set; }

        public virtual void Dispose()
        {
            
        }

        public abstract byte[] GetPalette();

        public abstract void SetPalette(byte[] palette);
    }
}

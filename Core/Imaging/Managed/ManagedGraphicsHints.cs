using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging.Managed
{
    public class ManagedGraphicsHints:IGraphicsHints
    {
        public BitmapCompositingMode Compositing { get; set; }

        protected byte[] matte = null;
        public byte[] GetMatte()
        {
            return matte;
        }

        public virtual void SetMatte(byte[] color)
        {
            //if (color != null && color.Length != this.BytesPerPixel) throw new ArgumentOutOfRangeException("Matte color bytes size must match BytesPerPixel");
            matte = color;
        }

        public bool RespectAlpha { get; protected set; }
        public void MarkAlphaUsed()
        {
            RespectAlpha = true;

        }
    }
}

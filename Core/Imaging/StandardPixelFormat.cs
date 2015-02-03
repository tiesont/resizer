using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class StandardPixelFormat:IPixelFormat
    {
        public Guid BitwiseId { get; private set; }

        public Guid BytewiseId { get; private set; }
        public int BitsPerPixel { get; private set; }

        public StandardPixelFormat(Guid bitwise, Guid bytewise, int bits)
        {
            this.BitwiseId = bitwise;
            this.BitsPerPixel = bits;
            this.BytewiseId = bytewise;
        }
        public StandardPixelFormat(Guid both, int bits)
        {
            this.BitwiseId = this.BytewiseId = both;
            this.BitsPerPixel = bits;
        }
    }
}

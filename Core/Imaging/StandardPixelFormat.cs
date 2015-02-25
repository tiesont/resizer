using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class StandardPixelFormat:IPixelFormat
    {
        public BitmapPixelFormats BitwiseFormat { get; private set; }

        public BitmapPixelFormats BytewiseFormat { get; private set; }
        public int BitsPerPixel { get; private set; }

        public StandardPixelFormat(BitmapPixelFormats bitwise, BitmapPixelFormats bytewise, int bits)
        {
            this.BitwiseFormat = bitwise;
            this.BitsPerPixel = bits;
            this.BytewiseFormat = bytewise;
        }
        public StandardPixelFormat(BitmapPixelFormats both, int bits)
        {
            this.BitwiseFormat = this.BytewiseFormat = both;
            this.BitsPerPixel = bits;
        }

   
    }
}

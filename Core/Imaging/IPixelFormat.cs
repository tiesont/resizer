using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface IPixelFormat
    {
        /// <summary>
        /// Represents a pixel layout. 
        /// Predictable bit significance; sytem byte order (endianess). 
        /// Use for algorithms that work with pixels in their 'common' data type, such as 'int' for BGRA.
        /// I.e, bit masks or bit shifting is used to extract or set individual channels.
        /// </summary>
        BitmapPixelFormats BitwiseFormat { get; }

        /// <summary>
        /// Represents a pixel layout. 
        /// Predictable byte order (endianess); system bit significance.
        /// Use for algorithms that access bytes 
        /// </summary>
        BitmapPixelFormats BytewiseFormat { get; }

        /// <summary>
        /// The number of bits per pixel.
        /// </summary>
        int BitsPerPixel { get; }
    }
}

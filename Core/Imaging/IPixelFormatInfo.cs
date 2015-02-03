using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public struct IChannelBits{
        byte bits;
        bool is_padding;
    }

    /// <summary>
    /// Does not attempt to address the meaning of values (no color space, gamma, etc), 
    /// but rather their serialization between bytes and numeric values
    /// </summary>
    public interface IPixelFormatInfo
    {
        /// <summary>
        /// Returns an IPixelFormat with the opposite endianess
        /// </summary>
        /// <returns></returns>
        IPixelFormatInfo GetInverseEndianForm();

        /// <summary>
        /// The number of bytes per pixel, or -1 if pixels do not align to byte boundaries
        /// </summary>
        int BytesPerPixel { get; }

        /// <summary>
        /// The number of bits per pixel. May be fewer than BytesPerPixel * 8.
        /// </summary>
        int BitsPerPixel { get; }

        byte Channels { get; }

        /// <summary>
        /// An array with one entry per meaningful bit.
        /// </summary>
        byte[] ChannelToBits { get; }

        //bool[] Ch
        //byte short int , float single double

        //BitConverter.IsLittleEndian

        //bit masks?
        //bits per pixel
        //serialize/deserialize to string
        //endianess
        //layout
        //data type

    }
}

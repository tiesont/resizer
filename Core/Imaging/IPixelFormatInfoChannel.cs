using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{

    // h s v l r g b a c m y k X Y Z  Y Cb Cr
    public interface IPixelFormatInfoChannel
    {
        int StartBit { get; }
        int Bits { get; }
        bool Integer {get;}
        bool Signed {get;}
    }
}

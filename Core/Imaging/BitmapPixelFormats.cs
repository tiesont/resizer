using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{

    public enum BitmapChannelSets
    {
        Indexed,
        BlackWhite,
        Gray,
        Rgb,
        Bgr,
        Argb,
        Bgra,
        CMYK,
        CYMKA,
        KYMC,

    }

    public enum ImageColorSpace
    {
        Unknown = 0,
        sRGB = 1,
        Adobe_RGB = 2,
        Linear_RGB = 3,
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dd371815%28v=vs.85%29.aspx
    //https://msdn.microsoft.com/en-us/library/windows/desktop/bb173059(v=vs.85).aspx
    //https://msdn.microsoft.com/en-us/library/windows/desktop/bb172558(v=vs.85).aspx
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee719797(v=vs.85).aspx
    // https://msdn.microsoft.com/en-us/library/windows/desktop/dd756766(v=vs.85).aspx
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ff485857(v=vs.85).aspx
    // https://github.com/imazen/freeimage/blob/master/Source/FreeImage.h


    /// <summary>
    /// Bits 42 through 62 are reserved for flags. Bits 32 through 41 are reserved for describing the number of bits required per pixel. 
    /// Bits 0 through 31 are reserved for describing unique things about the layout of the bits. 
    /// This format enumeration is designed to allow easier querying. 
    /// </summary>
    [Flags]
    public enum  BitmapPixelFormats: long
    {
        None = 0,

        

        Flags_UInteger = 1 << 62,
        Flags_FloatingPoint = 1 << 61,
        Flags_FixedPoint = 1 << 60,


        Flags_HasAlpha = 1 << 59,
        Flags_Premultiplied_By_Alpha = 1 << 58,

        Flags_Indexed = 1 << 57,
        Flags_Grayscale = 1 << 56,
        Flags_Rgb = 1 << 55,
        Flags_Cmyk = 1 << 54,
        Flags_XYZ = 1 << 53,
        Flags_LUV = 1 << 52,
        Flags_LAB = 1 << 51,
        Flags_YUV = 1 << 50,
        Flags_InvertOrder = 1 << 49,

        


        Indexed1b = Flags_Indexed & Flags_UInteger & (1 << 32),
        Indexed2b = Flags_Indexed & Flags_UInteger & (2 << 32),
        Indexed4b = Flags_Indexed & Flags_UInteger & (4 << 32),
        Indexed8b = Flags_Indexed & Flags_UInteger & (8 << 32),

        BlackWhite1b =  Flags_Grayscale & Flags_UInteger & (1 << 32),
        Gray2b =        Flags_Grayscale & Flags_UInteger & (2 << 32),
        Gray4b =        Flags_Grayscale & Flags_UInteger & (4 << 32),
        Gray8b =        Flags_Grayscale & Flags_UInteger & (8 << 32),
        Gray16b =       Flags_Grayscale & Flags_UInteger & (16 << 32),
        Gray32b_Float = Flags_Grayscale & Flags_FloatingPoint & (32 << 32),


        Rgb24b = Flags_Rgb & Flags_UInteger & (24 << 32),
        Bgr24b = Rgb24b & Flags_InvertOrder,

        Rgb32b = Flags_Rgb & Flags_UInteger & (32 << 32),
        Bgr32b = Rgb32b & Flags_InvertOrder,
        
        Bgra32b,
        Bgra32b_Premult,
        Argb32b,
        Argb32b_Premult,
        Bgr16b_555,
        Bgr16b_565,
        Cmyk32b,
        Kymc32b,

        Cmyk64b,
        Cmyka40b,
        Cmyka80b,
        Rgb48b,
        Rgba64b,
        Rgba64b_Premult,
        Rgb96b_Float,
        Rgba128b_Float,
        Rgba128b_Float_Premult,
        Rgba64_FixedPoint,
        

        



//Fmt64bppRGBAFixedPoint	


//Fmt48bppRGB	Fmt48bppRGB
//Fmt64bppRGBA
//	Fmt48bppRGB
//Fmt48bppRGB	Fmt64bppRGBA

//Fmt64bppRGBA	
//Fmt64bppPRGBA	
//Fmt64bppCMYK	
//Fmt80bppCMYKAlpha	
//Fmt96bppRGBFloat*	
//Fmt128bppRGBAFloat	
//Fmt128bppPRGBAFloat
    }
}

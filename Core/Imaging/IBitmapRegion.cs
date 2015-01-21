using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public enum BitmapPixelFormat {
        None = 0,
        Bgr24 = 24,
        Bgra32 = 32,
        Gray8 = 8, 
        Indexed8 = 7
    };
    public enum BitmapCompositingMode{
        Replace_self = 0, 
        Blend_with_self  =1,
        Blend_with_matte = 2
    };

    /// <summary>
    /// Represents a truecolor or grayscale bitmap region. Does not support indexed operations.
    /// </summary>
    public interface IBitmapRegion: IDisposable
    {   
        /// <summary>
        /// The width of the region or bitmap, in pixels
        /// </summary>
        int Width { get; }
        /// <summary>
        /// The height of the region or bitmap, in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The byte length of each row (will include unused padding if this is a cropped region)
        /// </summary>
        int Stride { get; }

        /// <summary>
        /// pointer to pixel 0,0 in the region; should be of length > h * stride
        /// </summary>
        IntPtr Pixel0 {get;}

        /// <summary>
        /// The number of bytes per pixel. (grayscale=1, 24-bit = 3, 32-bit = 4)
        /// </summary>
        int BytesPerPixel { get; }

        /// <summary>
        /// If true, the alpha channel will be honored if present.
        /// </summary>
        bool RespectAlpha { get;}

        /// <summary>
        /// If PixelsWriteable=false, an InvalidOperationException may occur. 
        /// Results in RespectAlpha being set to true.
        /// </summary>
        void MarkAlphaUsed();

        /// <summary>
        /// If true, we may modify any pixels
        /// </summary>
        bool PixelsWriteable { get; }

        /// <summary>
        /// Changes PixelsWriteable and PaddingWriteable to false
        /// </summary>
        void MarkReadonly();

        /// <summary>
        /// If true, we may modify any padding bytes between rows (equivalent to stride_readonly)
        /// </summary>
        bool PaddingWriteable { get; }

        /// <summary>
        /// The memory layout of each pixel
        /// </summary>
        BitmapPixelFormat PixelFormat {get;}

        /// <summary>
        /// If other images are drawn onto this canvas region, this setting controls how they will be composed.
        /// </summary>
        BitmapCompositingMode Compositing {get; set;}

        /// <summary>
        /// Gets the matte color to use when compositing (Blend_with_matte). If null, treat as transparent.
        /// </summary>
        /// <returns></returns>
        byte[] GetMatte();

        /// <summary>
        /// Changes the matte color to use when compositing (Blend_with_matte).
        /// </summary>
        /// <param name="color"></param>
        void SetMatte(byte[] color);

        /// <summary>
        /// Returns the palette used by this image, if indexed. Otherwise returns null. 
        /// Each pixel uses 4 bytes; BGRA format.
        /// </summary>
        /// <returns></returns>
        byte[] GetPalette();

        /// <summary>
        /// Replace the palette used by this image, if indexed. Each pixel uses 4 bytes, BGRA format.
        /// </summary>
        /// <param name="palette"></param>
        void SetPalette(byte[] palette);


        /// <summary>
        /// If true, unmanaged resources for this object have been disposed, and it may not be used
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Returns an enumeration of objects - which, if disposed, could break this object.
        /// </summary>
        IEnumerable<object> ReliantOn{ get; }


        Stack<Action> OnDispose{get;}

     
    }
}

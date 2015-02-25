using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Does not allow upside-down (win bmp, freeimage) representation. Vertical flip during lock/unlock if needed.
    /// </remarks>
    public interface IBitmapRegion : ITrackable
    {
        /// <summary>
        /// The width of the region or bitmap, in pixels
        /// </summary>
        int Width { get; }
        /// <summary>
        /// The height of the region or bitmap, in pixels; I.e, the number of scan rows.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The byte length of each row  (including unused padding, common for alignment or crop purposes)
        /// </summary>
        int Stride { get; }

        /// <summary>
        /// Pointer to the first byte in the region; should be of length > h * stride
        /// </summary>
        IntPtr Byte0 { get; }

        /// <summary>
        /// The number of bytes we are permitted to access following Pixel0. Should be >= Stride * Height
        /// </summary>
        long ByteCount { get; }

        /// <summary>
        /// If true, we may modify any pixels
        /// </summary>
        bool PixelsWriteable { get; }

        /// <summary>
        /// If true, we may modify any padding bytes between rows (equivalent to stride_readonly)
        /// </summary>
        bool PaddingWriteable { get; }

        /// <summary>
        /// The number of bytes per pixel, or -1 if pixels do not align to byte boundaries
        /// </summary>
        int BytesPerPixel { get; }

        /// <summary>
        /// The pixel format
        /// </summary>
        IPixelFormat Format { get; }

        /// <summary>
        /// Notify the implementation that pixels have been changed, and may need to be saved (if this is a temporary buffer).
        /// </summary>
        void MarkChanged();

        /// <summary>
        /// Closes the region, potentially causing any changes to be 
        /// copied back into the parent frame (if an intermediate buffer was required).
        /// Changes may be lost if MarkChanged() was not called.
        /// </summary>
        void Close();
    }
    
}

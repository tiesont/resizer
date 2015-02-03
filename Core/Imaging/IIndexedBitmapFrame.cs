using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface IIndexedBitmapFrame:   IBitmapFrame
    {
        /// <summary>
        /// Returns the palette used by this image, if indexed. Otherwise returns null. 
        /// Each pixel uses 4 bytes; BGRA format.
        /// Move to IBitmapFrame?
        /// </summary>
        /// <returns></returns>
        byte[] GetPalette();

        /// <summary>
        /// Replace the palette used by this image, if indexed. Each pixel uses 4 bytes, BGRA format.
        /// Move to IBitmapFrame?
        /// </summary>
        /// <param name="palette"></param>
        void SetPalette(byte[] palette);

        /// <summary>
        /// The pixel format used by palette colors
        /// </summary>
        IPixelFormat PalettePixelFormat { get; set; }
    }
}

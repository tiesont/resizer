using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{


    public interface IBitmapFrame: ITrackable
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
        /// The pixel data format
        /// </summary>
        IPixelFormat PixelFormat { get; }

        /// <summary>
        /// Makes a portion of the frame accessible in a contiguous buffer at a fixed point in memory. 
        /// Does not guarantee thread safety.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        IBitmapRegion OpenRegion(int x, int y, int w, int h, RegionAccessMode accessMode);

        /// <summary>
        /// Returns true if it is permissible to call OpenRegion. 
        /// Some implementations only permit one region per frame (or per bitmap!) to be opened at a time.
        /// This may be an expensive operation, especially if an intermediate buffer is required. Changes may not take effect until the region is Flushed.
        /// </summary>
        bool CanOpenRegion { get; }


        /// <summary>
        /// Rendering hints 
        /// </summary>
        IGraphicsHints Hints { get; }


        ///// <summary>
        ///// Checks if it is possible to change the dimensions of the frame - in place. 
        ///// </summary>
        ///// <returns></returns>
        //bool CanMutateDimensions();

        ///// <summary>
        ///// After rotating a frame by mutating the entire region, this can be used
        ///// to update the stride and dimensions to match.
        ///// May cause replacement of the underlying buffer or bitmap.
        ///// </summary>
        ///// <param name="w"></param>
        ///// <param name="h"></param>
        ///// <param name="stride"></param>
        //void MutateDimensions(int w, int h, int stride);
    }
}

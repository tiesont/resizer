using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface IBitmapContainer : ITrackable
    {
        /// <summary>
        /// Returns the number of frames (in each dimension) provided it this image. TIFF and volumetric images may be multi-dimensional.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<FrameDimension, long>> GetFrameCounts();

        /// <summary>
        /// Opens the given frame for access. Does not provide any thread safety.
        /// </summary>
        /// <param name="frameIndicies">Provide null to access the first frame</param>
        /// <returns></returns>
        IBitmapFrame OpenFrame(IEnumerable<Tuple<FrameDimension, long>> frameIndicies);

        /// <summary>
        /// Returns true if it is permissible to call OpenFrame. 
        /// Some implementations only permit one frame to be opened at a time.
        /// </summary>
        bool CanOpenFrame { get; }

    }
}

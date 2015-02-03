using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface ITrackable
    {
        /// <summary>
        /// Read the ITrackingScope managing the lifetime of this object. May be null. 
        /// Write access is for exclusive use by ITrackingScope implementations. 
        /// Changing this directly will trigger an InvalidOperationException at a later time.
        /// </summary>
        ITrackingScope TrackingScope { get; set; }

        /// <summary>
        /// May be called repeatedly; should release associated resources
        /// </summary>
        void ReleaseResources();
    }

}

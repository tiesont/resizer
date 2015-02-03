using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public enum RegionAccessMode
    {
        /// <summary>
        /// Open for reading only (changes may or may not be discarded)
        /// </summary>
        ReadOnly = 0x0001,

        /// <summary>
        /// Open for writing only (region buffer may contain random noise)
        /// </summary>
        WriteOnly = 0x0002,
            
        /// <summary>
        /// Open for reading and writing. The region  will contain valid data,
        /// and (if buffered), changes will be flushed on a call to Close() if it is preceded by a call to MarkChanged().
        /// </summary>
        ReadWrite = ReadOnly | WriteOnly
        
    }
}

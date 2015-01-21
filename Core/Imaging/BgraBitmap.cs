using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    internal struct BgraBitmap{
        /// <summary>
        /// bitmap width in pixels
        /// </summary>
        int w;
        /// <summary>
        /// bitmap height in pixels
        /// </summary>
        int h;
        /// <summary>
        /// byte length of each row (may include any amount of padding)
        /// </summary>
        int stride;
        /// <summary>
        /// pointer to pixel 0,0; should be of length > h * stride
        /// </summary>
        IntPtr pixels; 
        /// <summary>
        /// If true, we're not responsible for disposing of *pixels 
        /// </summary>
        bool borrowed_pixels; 
        /// <summary>
        /// If false, we can even ignore the alpha channel on 4bpp
        /// </summary>
        bool alpha_meaningful; 
        /// <summary>
        /// If false, we can edit pixels without affecting the stride
        /// </summary>
        bool pixels_readonly; 
        /// <summary>
        /// If false, we can change the stride of the image.
        /// </summary>
        bool stride_readonly; 
        /// <summary>
        /// Number of *bytes* (not bits) per pixel
        /// </summary>
        int bpp;
    } 
}

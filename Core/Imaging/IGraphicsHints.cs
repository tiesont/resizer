using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{

    public enum BitmapCompositingMode
    {
        Replace_self = 0,
        Blend_with_self = 1,
        Blend_with_matte = 2
    };


    public interface IGraphicsHints
    {

        //gamma
        //ICC profile


        /// <summary>
        /// If other images are drawn onto this canvas region, this setting controls how they will be composed.
        /// </summary>
        BitmapCompositingMode Compositing { get; set; }

        /// <summary>
        /// Gets the matte color to use when compositing (Blend_with_matte). If null, treat as transparent.
        /// </summary>
        /// <returns></returns>
        byte[] GetMatte();

        /// <summary>
        /// Changes the matte color to use when compositing (Blend_with_matte).
        /// Blend color should be in same pixel format as canvas.
        /// </summary>
        /// <param name="color"></param>
        void SetMatte(byte[] color);

        /// <summary>
        /// Indicates meaningful data in the alpha channel. 
        /// If true, the alpha channel should be honored when present.
        /// </summary>
        bool RespectAlpha { get; }

        /// <summary>
        /// Results in RespectAlpha being set to true.
        /// </summary>
        void MarkAlphaUsed();

        //TODO: allowreuse?

    }
}

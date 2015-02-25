using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class SysDrawingIndexedFrame:SysDrawingFrame, IIndexedBitmapFrame
    {
        public SysDrawingIndexedFrame()
        {

        }
        public  byte[] GetPalette()
        {
            var bit = this.ParentBitmap.DangerousCurrentBitmap;
            var colors = bit.Palette.Entries;
            byte[] bytes = new byte[colors.Length * 4];
            for (var i = 0; i < colors.Length; i++)
            {
                bytes[i * 4] = colors[i].B;
                bytes[i * 4 + 1] = colors[i].G;
                bytes[i * 4 + 2] = colors[i].R;
                bytes[i * 4 + 3] = colors[i].A;
            }
            return bytes;
        }

        public  void SetPalette(byte[] palette)
        {
            var bit = this.ParentBitmap.DangerousCurrentBitmap;
            var count = palette.Length / 4;
            var colors = bit.Palette.Entries;
            if (count != colors.Length)
            {
                throw new InvalidOperationException("You cannot change the size of the color palette on an image. Palette of size " + count + " provided, expected " + colors.Length);
            }
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.FromArgb(palette[i * 4 + 3], palette[i * 4 + 2], palette[i * 4 + 1], palette[i * 4]);
            }
        }

        public Guid PaletteFormat { get; set; }
        public Guid PaletteFormatLE { get; set; }


        public IPixelFormat PalettePixelFormat
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

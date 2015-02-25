using ImageResizer.Plugins;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{

    
    public class SysDrawingRegion : AbstractBitmapRegion
    {
        private Bitmap underlying_bitmap;
        private BitmapData locked_bitmap_data;
        
        public override void ReleaseResources()
        {
            if (locked_bitmap_data != null){
                underlying_bitmap.UnlockBits(locked_bitmap_data);
                locked_bitmap_data = null;
            }
        }
        private enum SysDrawingPaletteFlags{
            None = 0,
            PaletteFlagsHasAlpha    = 0x0001,
            PaletteFlagsGrayScale   = 0x0002,
            PaletteFlagsHalftone    = 0x0004 
        };

        public static SysDrawingRegion WindowInto(Bitmap source)
        {
            return WindowInto(source, new Rectangle(0, 0, source.Width, source.Height));
        }
        public static SysDrawingRegion WindowInto(Bitmap source, Rectangle from)
        {
            var paletteFlags = (source != null && source.Palette != null) ? (SysDrawingPaletteFlags)source.Palette.Flags : SysDrawingPaletteFlags.None;

            bool paletteGray = (paletteFlags & SysDrawingPaletteFlags.PaletteFlagsGrayScale) > 0;
            bool paletteAlpha = (paletteFlags & SysDrawingPaletteFlags.PaletteFlagsGrayScale) > 0;

            bool shouldnt_have_alpha = source.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed && !paletteAlpha;

            return new SysDrawingRegion(source, from, false, false, !shouldnt_have_alpha, paletteGray, false);
        }
        public SysDrawingRegion(Bitmap source, Rectangle from, bool pixels_writeable, bool padding_writeable, bool alpha_meaningful, bool continous_grayscale, bool dispose_source){
            this.underlying_bitmap = source;
            //var regionFormat = ConvertFormat(source.PixelFormat,continous_grayscale);

            if (from.X < 0 || from.Y < 0 || from.Right > source.Width || from.Bottom > source.Height || from.Width < 1 || from.Height < 1) {
                throw new ArgumentOutOfRangeException("from");
            }

            //if (regionFormat == BitmapPixelFormat.None){
            //    throw new ArgumentOutOfRangeException("source", "Invalid pixel format " + source.PixelFormat.ToString());
            //}


            //bool isCropped = from.X != 0 || from.Y != 00 || from.Width != source.Width || from.Height != source.Height;

            var lockMode = (pixels_writeable || padding_writeable) ? ImageLockMode.ReadWrite : ImageLockMode.ReadOnly;

            locked_bitmap_data = source.LockBits(from,lockMode,source.PixelFormat);

            this.Width = locked_bitmap_data.Width;
            this.Height = locked_bitmap_data.Height;
            this.Stride = locked_bitmap_data.Stride;
            this.Byte0 = locked_bitmap_data.Scan0;
            this.PixelsWriteable = pixels_writeable;
            this.PaddingWriteable = padding_writeable;
            //this.PixelFormat = regionFormat;
            //this.RespectAlpha = alpha_meaningful && FormatCouldHaveAlpha(this.PixelFormat);

        }

        //private BitmapPixelFormat ConvertFormat(System.Drawing.Imaging.PixelFormat fmt, bool assumeGrayscale){
        //    if (fmt == System.Drawing.Imaging.PixelFormat.Format24bppRgb) return BitmapPixelFormat.Bgr24;
        //    if (fmt == System.Drawing.Imaging.PixelFormat.Format32bppArgb || 
        //        fmt == System.Drawing.Imaging.PixelFormat.Format32bppRgb) return BitmapPixelFormat.Bgra32;
        //    if (fmt == System.Drawing.Imaging.PixelFormat.Format8bppIndexed){
        //        return assumeGrayscale ?  BitmapPixelFormat.Gray8 : BitmapPixelFormat.Indexed8;
        //    }
        //    return BitmapPixelFormat.None;
        //}
  

        //private bool FormatCouldHaveAlpha(BitmapPixelFormat fmt){
        //    return fmt == BitmapPixelFormat.Bgra32 || fmt == BitmapPixelFormat.Indexed8;
        //}

        public override void Close()
        {
            throw new NotImplementedException();
        }
  

    }
}

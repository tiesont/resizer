using System.Collections.Generic;
using System.Text;
using Xunit;
using ImageResizer.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Net;
using ImageResizer.Tests;
using ImageResizer.Resizing;
using ImageResizer.ExtensionMethods;
using ImageResizer.Imaging;

namespace ImageResizer.Core.Tests
{
    public class ImageRegionTest
    {

        [Fact]
        public void TestLockBitsAndDrawImageCanOverlap()
        {
            using (var b = new Bitmap(100, 100, PixelFormat.Format32bppArgb))
            {


                using (var g1 = Graphics.FromImage(b))
                {
                    g1.FillRectangle(Brushes.Red, new Rectangle(0, 0, 100, 100));
                    using (var region = SysDrawingRegion.WindowInto(b))
                    {
                        g1.FillRectangle(Brushes.Brown, new Rectangle(30, 0, 70, 100));
                        
                        region.Managed().FillRectangle(15, 0, 85, 100, new byte[] { 255, 0, 0, 255 });
                        using (var b2 = region.SysDrawing().WrapWithGdiBitmap())
                        {
                            using (var g3 = Graphics.FromImage(b2))
                            {
                                g3.FillRectangle(Brushes.Yellow, new Rectangle(60, 0, 40, 100));
                            }
                        }
                    }
                    
                }

                using (var r = SysDrawingRegion.WindowInto(b))
                {
                    Assert.True(r.Managed().RectangleIsColor(0, 0, 15, 100, Color.Red.ToBgra()));
                    Assert.True(r.Managed().RectangleIsColor(15, 0, 30, 100, new byte[] { 255, 0, 0, 255 }));
                    Assert.True(r.Managed().RectangleIsColor(30, 0, 30, 100, Color.Brown.ToBgra()));
                    Assert.True(r.Managed().RectangleIsColor(60, 0, 40, 100, Color.Yellow.ToBgra()));
                    

                }
            }
        }
    }
}

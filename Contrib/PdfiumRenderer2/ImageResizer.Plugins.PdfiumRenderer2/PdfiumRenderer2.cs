// Copyright (c) 2012 Jason Morse and Nathanael Jones
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

using ImageResizer.Configuration;
using ImageResizer.Configuration.Issues;
using ImageResizer.Resizing;
using ImageResizer.Util;

namespace ImageResizer.Plugins.PdfiumRenderer2
{
    public class PdfiumRenderer2Plugin : BuilderExtension, IPlugin, IFileExtensionPlugin, IIssueProvider, IQuerystringPlugin
    {
        #region Fields

        private static readonly IList<string> QueryStringKeys = new ReadOnlyCollection<string>(new[]
        {
            "page",
            "width",
            "height",
            "maxwidth",
            "maxheight",
            "annotations",
            "lcd",
            "grayscale",
            "halftone",
            "print",
            "transparent"
        });

        private static readonly IList<string> SupportedExtensions = new ReadOnlyCollection<string>(new[]
        {
            ".pdf"
        });

        private int _defaultHeight = 600;
        private int _defaultWidth = 800;
        private int _maxHeight = 4000;
        private int _maxWidth = 4000;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets maximum height in pixels for rendered output.
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///   Value is less than or equal to zero.
        /// </exception>
        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "Maximum height must be greater than zero.");
                _maxHeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets maximum width in pixels for rendered output.
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///   Value is less than or equal to zero.
        /// </exception>
        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "Maximum width must be greater than zero.");
                _maxWidth = value;
            }
        }

        /// <summary>
        ///   Gets or sets the default height in pixels for rendered output if neither height nor width are specified.
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///   Value is less than or equal to zero.
        /// </exception>
        public int DefaultHeight
        {
            get { return _defaultHeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "Default height must be greater than zero.");
                _defaultHeight = value;
            }
        }

        /// <summary>
        ///   Gets or sets the default width in pixels for rendered output if neither height nor width are specified.
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///   Value is less than or equal to zero.
        /// </exception>
        public int DefaultWidth
        {
            get { return _defaultWidth; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "Default width must be greater than zero.");
                _defaultWidth = value;
            }
        }

        #endregion

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Need to retain stream for bitmap backing store.")]
        public override Bitmap DecodeStream(Stream stream, ResizeSettings settings, string optionalPath)
        {
            if (string.IsNullOrEmpty(optionalPath))
            {
                if (stream.CanSeek)
                {
                    //Check the header instead if no filename is present.
                    byte[] header = new byte[4];
                    stream.Read(header, 0, 4);
                    bool isPdf = (header[0] == '%' && header[1] == 'P' && header[2] == 'D' && header[3] == 'F');
                    stream.Seek(-4, SeekOrigin.Current); //Restore position.

                    if (!isPdf) return null;
                }
                else
                {
                    return null; //It's not seekable, we can't check the header. 
                }
            }
            else if (!SupportedExtensions.Contains(Path.GetExtension(optionalPath), StringComparer.OrdinalIgnoreCase))
            {
                // Not a supported format
                return null;
            }

            // Do not allow decoding with PDFium there are issues with performing this decode
            IIssue[] issues = GetIssues().ToArray();
            if (issues.Length > 0)
            {
                string message = string.Join(Environment.NewLine, issues.Select(x => x.Summary).ToArray());
                throw new InvalidOperationException(message);
            }
            
            byte[] bytes;
            
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                bytes = ms.ToArray();
            }

            using (var pdf = new PDFiumSharp.PdfDocument(bytes, 0))
            {
                // Extract the requested page number from resize settings, or default to first page
                int pageNumber = settings.GetValueOrDefault("page", 1);

                // Try to get the page number from PDF info. If not available, abort. This is caused by
                // requesting a page that does not exist.
                if (pageNumber < 1 || pageNumber > pdf.Pages.Count)
                {
                    return null;
                }

                // Calculate the output size based on the actual size of the page.
                var page = pdf.Pages[pageNumber - 1];
                var outputSize = GetOutputSize(settings, page.Width, page.Height);

                // Build the render flags from the provided settings.
                var flags = BuildFlags(settings);

                var bitmap = new Bitmap(outputSize.Width, outputSize.Height);
                bitmap.SetResolution(96, 96);

                if (!(settings.Get<bool>("transparent") ?? false))
                {
                    // Some PDF files do not specify a color for their background. 
                    // Using a Graphics object lets us fill an arbitrary color (white, in this case).
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.White);
                    }
                }

                // Generate the PDF page.
                page.Render(bitmap, (0, 0, outputSize.Width, outputSize.Height), page.Orientation, flags);

                return bitmap;
            }
        }

        public IPlugin Install(Config c)
        {
            if (c != null)
            {
                c.Plugins.add_plugin(this);
            }
            return this;
        }

        public bool Uninstall(Config c)
        {
            if (c != null)
            {
                c.Plugins.remove_plugin(this);
            }
            return true;
        }

        public IEnumerable<IIssue> GetIssues()
        {
            return Enumerable.Empty<IIssue>();
        }

        public IEnumerable<string> GetSupportedFileExtensions()
        {
            return SupportedExtensions;
        }

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return QueryStringKeys;
        }

        private Size GetOutputSize(ResizeSettings settings, double boundingWidth, double boundingHeight)
        {
            // Output size is determined by resize settings, if available.
            //  maxwidth, maxheight 
            //      – Fit the image within the specified bounds, preserving aspect ratio.
            //  width, height 
            //      – Force the final width and/or height to certain dimensions. 
            //        Whitespace will be added if the aspect ratio is different.
            // This plugin renders to a size within the requested size and then expects remaining plugins in the 
            // pipeline to perform and additional processing such as adding whitespace, etc.
            // It can safely treat width/height as maxwidth/maxheight.

            double imageRatio = boundingWidth / boundingHeight;
            double width = settings.Width;
            double height = settings.Height;
            double maxwidth = settings.MaxWidth;
            double maxheight = settings.MaxHeight;

            //Allow overrides with pdfwidth and pdfheight when we *want* to rescale afterwards.
            int pw = settings.Get("pdfwidth", -1);
            int ph = settings.Get("pdfheight", -1);
            if (pw > 0) { width = pw; maxwidth = -1; }
            if (ph > 0) { height = ph; maxheight = -1; }

            //Handle cases of width/maxheight and height/maxwidth as in legacy versions. 
            if (width != -1 && maxheight != -1) maxheight = Math.Min(maxheight, (width / imageRatio));
            if (height != -1 && maxwidth != -1) maxwidth = Math.Min(maxwidth, (height * imageRatio));

            //Eliminate cases where both a value and a max value are specified: use the smaller value for the width/height 
            if (maxwidth > 0 && width > 0) { width = Math.Min(maxwidth, width); maxwidth = -1; }
            if (maxheight > 0 && height > 0) { height = Math.Min(maxheight, height); maxheight = -1; }

            //Move values to width/height
            if (width <= 0) width = maxwidth;
            if (height <= 0) height = maxheight;

            //Calculate missing value(s) 
            if (width > 0 && height <= 0) height = width / imageRatio;
            else if (height > 0 && width <= 0) width = height * imageRatio;
            else if (width <= 0 && height <= 0)
            { // If neither width nor height as specified use default values
                width = DefaultWidth;
                height = DefaultHeight;
            }

            // Limit maximum output size
            width = Math.Min(width, this.MaxWidth);
            height = Math.Min(height, this.MaxHeight);


            // Determine the scaling values, and use the smallest to ensure we fit in the bounding box without changing 
            // the aspect ratio otherwise we will crop.
            //Use a scaled version of boundingBox inside our maximum width and height constraints.
            return PolygonMath.RoundPoints(PolygonMath.ScaleInside(new SizeF((float)boundingWidth, (float)boundingHeight), new SizeF((float)width, (float)height)));
        }

        private PDFiumSharp.RenderingFlags BuildFlags(ResizeSettings settings)
        {
            var flags = PDFiumSharp.RenderingFlags.None;

            if (settings.Get<bool>("annotations").GetValueOrDefault())
                flags |= PDFiumSharp.RenderingFlags.Annotations;

            if (settings.Get<bool>("lcd").GetValueOrDefault())
                flags |= PDFiumSharp.RenderingFlags.LcdText;

            if (settings.Get<bool>("grayscale").GetValueOrDefault())
                flags |= PDFiumSharp.RenderingFlags.Grayscale;

            if (settings.Get<bool>("halftone").GetValueOrDefault())
                flags |= PDFiumSharp.RenderingFlags.ForceHalftone;

            if (settings.Get<bool>("print").GetValueOrDefault())
                flags |= PDFiumSharp.RenderingFlags.Printing;

            //if (settings.Get<bool>("transparent").GetValueOrDefault())
            //    flags |= PDFiumSharp.RenderingFlags.Transparent;

            return flags;
        }
    }
}

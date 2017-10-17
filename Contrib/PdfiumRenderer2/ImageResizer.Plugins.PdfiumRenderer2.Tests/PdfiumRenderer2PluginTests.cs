using NUnit.Framework;
using ImageResizer.Plugins.PdfiumRenderer2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using ImageResizer.Configuration.Issues;

namespace ImageResizer.Plugins.PdfiumRenderer2.Tests
{
    [TestFixture]
    public class PdfiumRenderer2PluginTests
    {
        private PdfiumRenderer2Plugin _decoder;

        /// <summary>
        ///   Name of embedded PDF test document. 
        /// </summary>
        /// <remarks>
        ///   This document was generated in Word and printed to PDF. The page size is 8.5" x 11"
        ///   It has 2 pages:
        ///   Page 1: Portrait, with large letter 'A' in red box with black border centered vertically and horizontally, 
        ///   Page 2: Landscape, with large letter 'B' in green box with black border centered vertically and horizontally.
        /// </remarks>
        private const string TestDocumentFileName = "Test.pdf";

        private static Stream OpenStream(string fileName)
        {
            string resourceName = typeof(PdfiumRenderer2PluginTests).Namespace + "." + fileName;
            return typeof(PdfiumRenderer2PluginTests).Assembly.GetManifestResourceStream(resourceName);
        }

        public PdfiumRenderer2PluginTests()
        {
            _decoder = new PdfiumRenderer2Plugin();

            IIssue[] issues = _decoder.GetIssues().ToArray();
            if (issues.Length > 0)
            {
                string issuesMessage = string.Join(Environment.NewLine, issues.Select(x => x.Summary).ToArray());
                throw new InvalidOperationException(String.Format("Expecting there are no plugin issues reported: {0}", issuesMessage));
            }
        }

        private void BitmapEqual(Bitmap expected, Stream stream)
        {
            using (var actual = (Bitmap)Image.FromStream(stream))
            {
                Assert.AreEqual(expected.Width, actual.Width);
                Assert.AreEqual(expected.Height, actual.Height);

                for (int y = 0; y < actual.Height; y++)
                {
                    for (int x = 0; x < actual.Width; x++)
                    {
                        Assert.AreEqual(expected.GetPixel(x, y).ToArgb(), actual.GetPixel(x, y).ToArgb());
                    }
                }
            }
        }

        [Test]
        public void GetSupportedFileExtensions_ExpectPdf()
        {
            // Act
            var supportedFileExtensions = _decoder.GetSupportedFileExtensions().ToArray();

            // Assert
            Assert.Contains(".pdf", supportedFileExtensions);
        }

        #region Page1 (Portrait)

        [Test]
        public void DecodeStream_WhenHeightSpecified_ExpectPage1()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(400, bitmap.Height);
            Assert.AreEqual(309, bitmap.Width);
        }

        [Test]
        public void DecodeStream_WhenWidthSpecified_ExpectPage1()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["width"] = "400";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(400, bitmap.Width);
            Assert.AreEqual(518, bitmap.Height);
        }

        [Test]
        public void DecodeStream_WhenWidthHeightSpecified_ExpectPage1()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";
            settings["width"] = "400";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(400, bitmap.Height);
            Assert.AreEqual(309, bitmap.Width);
        }

        [Test]
        public void DecodeStream_Output_ExpectPage1()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            BitmapEqual(bitmap, OpenStream("Page1.png"));
        }

        #endregion

        #region Page2 (Landscape)

        [Test]
        public void DecodeStream_WhenHeightSpecified_ExpectPage2()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";
            settings["page"] = "2";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(400, bitmap.Height);
            Assert.AreEqual(518, bitmap.Width);
        }

        [Test]
        public void DecodeStream_WhenWidthSpecified_ExpectPage2()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["width"] = "400";
            settings["page"] = "2";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(400, bitmap.Width);
            Assert.AreEqual(309, bitmap.Height);
        }

        [Test]
        public void DecodeStream_WhenWidthHeightSpecified_ExpectPage2()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";
            settings["width"] = "400";
            settings["page"] = "2";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.AreEqual(309, bitmap.Height);
            Assert.AreEqual(400, bitmap.Width);
        }

        [Test]
        public void DecodeStream_Output_ExpectPage2()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";
            settings["width"] = "400";
            settings["page"] = "2";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            BitmapEqual(bitmap, OpenStream("Page2.png"));
        }

        #endregion

        #region Page3 (Does not exist)

        [Test]
        public void DecodeStream_WhenInvalidPageSpecified_ExpectNull()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["page"] = "3";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream(TestDocumentFileName), settings, TestDocumentFileName);

            // Assert
            Assert.Null(bitmap);
        }

        #endregion

        #region Transparency

        [Test]
        public void DecodeStream_Output_ExpectTransparency()
        {
            // Arrange
            ResizeSettings settings = new ResizeSettings();
            settings["height"] = "400";
            settings["width"] = "400";
            settings["page"] = "2";
            settings["transparent"] = "1";

            // Act
            Bitmap bitmap = _decoder.DecodeStream(OpenStream("TransparencyTest.pdf"), settings, TestDocumentFileName);

            // Assert
            BitmapEqual(bitmap, OpenStream("TransparencyPage1.png"));
        }

        #endregion
    }
}
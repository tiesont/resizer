using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public class SysDrawingContainer:IBitmapContainer
    {
        public SysDrawingContainer(ITrackingScope scope, Trackable<Bitmap> initalBitmap)
        {
            this.scope = scope;
            this.b = initalBitmap;
            scope.TrackDependency(this.b, this);
        }
        protected Trackable<Bitmap> b;
        protected ITrackingScope scope;
        public Bitmap DangerousCurrentBitmap { get { return b.Value; } }
        


        private SysDrawingFrame lastLockedFrame = null;
        public IBitmapFrame OpenFrame(IEnumerable<Tuple<FrameDimension,long>> frameIndicies)
        {
            if (lastLockedFrame != null && !lastLockedFrame.IsDisposed)
            {
                throw new InvalidOperationException("You cannot lock a new frame until the previously locked frame has been disposed.");
            }
            if (frameIndicies != null)
            {
                foreach (var ix in frameIndicies)
                {
                    b.Value.SelectActiveFrame(ToDimensionGuid(ix.Item1), (int)ix.Item2);
                }
            }
            if (b.Value.PixelFormat == System.Drawing.Imaging.PixelFormat.)
            lastLockedFrame = new SysDrawingFrame(this);
            return lastLockedFrame;
        }


        public bool CanOpenFrame
        {
            get { return lastLockedFrame == null || !lastLockedFrame.HasLockedBits; }
        }


        public ITrackingScope TrackingScope { get; set; }

        public void ReleaseResources()
        {
            //Nothing to release, actually, that's handled by Trackable
        }

        public IEnumerable<Tuple<FrameDimension, long>> GetFrameCounts()
        {
            var dimensions = b.Value.FrameDimensionsList;
            return dimensions.Select(g => Tuple.Create(FromGuid(g), (long)b.Value.GetFrameCount(GuidToDimensionGuid(g)))).ToArray();
        }


        private System.Drawing.Imaging.FrameDimension ToDimensionGuid(FrameDimension d)
        {
            if (d == FrameDimension.Page) return System.Drawing.Imaging.FrameDimension.Page;
            if (d == FrameDimension.Resolution) return System.Drawing.Imaging.FrameDimension.Resolution;
            if (d == FrameDimension.Time) return System.Drawing.Imaging.FrameDimension.Time;
            throw new ArgumentOutOfRangeException("d", "The provided dimension type is not supported by System.Drawing: " + d.ToString());
        }
        private FrameDimension FromGuid(Guid g)
        {
            if (g == System.Drawing.Imaging.FrameDimension.Page.Guid) return FrameDimension.Page;
            if (g == System.Drawing.Imaging.FrameDimension.Resolution.Guid) return FrameDimension.Resolution;
            if (g == System.Drawing.Imaging.FrameDimension.Time.Guid) return FrameDimension.Time;
            throw new ArgumentOutOfRangeException("g", "The provided dimension guid is not recognized: " + g.ToString());
        }
        private System.Drawing.Imaging.FrameDimension GuidToDimensionGuid(Guid g)
        {
            foreach (var w in new[] { System.Drawing.Imaging.FrameDimension.Page, System.Drawing.Imaging.FrameDimension.Time, System.Drawing.Imaging.FrameDimension.Resolution })
            {
                if (w.Guid == g) return w;
            }
            throw new ArgumentOutOfRangeException("g", "The provided dimension guid is not recognized: " + g.ToString());
        }


    }
}

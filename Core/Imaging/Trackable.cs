using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{

    /// <summary>
    /// Wraps any IDisposable object in an ITrackable wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Trackable<T>: ITrackable where T:IDisposable
    {
        public T Value { get; private set; }
        public Trackable(T obj)
        {
            Value = obj;
        }
        public ITrackingScope TrackingScope { get; set; }

        public void ReleaseResources()
        {
            this.Value.Dispose();
        }
    }
}

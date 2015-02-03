using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public interface ITrackingScope : IDisposable
    {
        /// <summary>
        /// Increment the external reference count for the given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        T Reference<T>(T obj) where T : ITrackable;

        /// <summary>
        /// Decrement the external reference count for the given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        T Unreference<T>(T obj) where T : ITrackable;

        /// <summary>
        /// True if all objects have been cleaned and deleted
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Mark the given object as 'preserved'. It will not be cleaned up.
        /// </summary>
        /// <param name="obj"></param>
        void Skip(ITrackable obj);

        bool IsSkipped(ITrackable obj);

        /// <summary>
        /// Establish an internal reference/dependency between two objects
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dependency"></param>
        void TrackDependency(ITrackable target, ITrackable dependency);

        /// <summary>
        /// Clean all objects (as much as possible) and return a new scope with the remnants (which have been Skipped).
        /// </summary>
        /// <returns></returns>
        ITrackingScope CleanAndReturnSkipped();
    }
}

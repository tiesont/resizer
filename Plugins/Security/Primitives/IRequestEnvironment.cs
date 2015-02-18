using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security
{
    /// <summary>
    /// Host-agnostic environment abstraction. Places burden on the client to write interaction code for both.
    /// </summary>
    public interface IRequestEnvironment
    {
        /// <summary>
        /// If true, GetOwin() should return a valid OWIN environment
        /// </summary>
        bool HasOwin { get; }

        /// <summary>
        /// If true, GetConext() should return a non-null, HttpContextBase-like-object;
        /// </summary>
        bool HasContext { get; }

        /// <summary>
        /// If true, OWIN is the native request context. Changes should not be made to a non-native request context.
        /// </summary>
        bool OwinIsNative { get; }

        /// <summary>
        /// If true, the HttpContextBase-like-object is the native request context. Changes should not be made to a non-native request context.
        /// </summary>
        bool ContextIsNative { get; }


        /// <summary>
        /// Should return null, or a valid OWIN evironment. 
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetOwin();

        /// <summary>
        /// Should return an HttpContextBase-like-object, if present. May return null if OWIN hosted
        /// </summary>
        /// <returns></returns>
        object GetConext();
    }
}

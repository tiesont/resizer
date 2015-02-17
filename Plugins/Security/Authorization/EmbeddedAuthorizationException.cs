using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.Security.Authorization
{
    [Serializable]
    public class EmbeddedAuthorizationException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public EmbeddedAuthorizationException()
        {
        }

        public EmbeddedAuthorizationException(string message)
            : base(message)
        {
        }

        public EmbeddedAuthorizationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected EmbeddedAuthorizationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("ResourceReferenceProperty", ResourceReferenceProperty);
            base.GetObjectData(info, context);
        }

    }
}

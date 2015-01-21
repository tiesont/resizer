using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public struct Size<T> where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        private T w;
        private T h;
        public bool IsEmpty
        {
            get
            {
                return default(T).Equals(w) && default(T).Equals(h);
            }
        }
        public Size(T w, T h)
        {
            this.w = w;
            this.h = h;
        }

        public T Width { get { return w; } }
        public T Height { get { return h; } }

    }
}

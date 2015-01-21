using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public struct Rectangle<T> where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        private T x;
        private T y;
        private T w;
        private T h;
        public bool IsEmpty
        {
            get
            {
                return default(T).Equals(x) && default(T).Equals(y) &&
                    default(T).Equals(w) && default(T).Equals(h);
            }
        }
        public Rectangle(T x, T y, T w, T h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public T Left { get { return x; } }
        public T Top { get { return y; } }
        public T X { get { return x; } }
        public T Y { get { return y; } }
        public T Width { get { return w; } }
        public T Height { get { return h; } }
        public T X2 { get { return ((dynamic)x) + ((dynamic)w); } }
        public T Y2 { get { return ((dynamic)y) + ((dynamic)h); } }
    }
}

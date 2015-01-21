using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Imaging
{
    public struct Point<T> where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        private T x;
        private T y;
        public bool IsEmpty
        {
            get
            {
                return default(T).Equals(x) && default(T).Equals(y);
            }
        }
        public Point(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public T X { get { return x; } }
        public T Y { get { return y; } }

    }
}

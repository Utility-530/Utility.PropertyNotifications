using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityStruct
{
    public struct Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }


        public double X { get;  }
        public double Y { get; }


    }
}

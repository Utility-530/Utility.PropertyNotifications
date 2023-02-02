
namespace Utility.Structs
{
    public readonly struct Point3
    {
        public Point3(double x, double y,double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public double X { get; }

        public double Y { get; }

        public double Z { get; }
    }
}

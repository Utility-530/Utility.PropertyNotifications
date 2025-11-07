namespace Utility.Structs
{
    public readonly struct IntegerPoint3
    {
        public IntegerPoint3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }

        public int Y { get; }

        public int Z { get; }
    }
}
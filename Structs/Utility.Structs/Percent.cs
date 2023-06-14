using System;

namespace Utility.Structs
{
    public readonly struct Percent
    {
        public Percent(decimal val)
        {
            Decimal = val;
        }

        public Percent(double val) : this((decimal)val)
        { }


        public Percent(int percent)
        {
            Decimal = percent / 100m;
        }


        // User-defined conversion from Probability to decimal
        public static implicit operator decimal(Percent i)
        {
            return i.Decimal;
        }

        public static implicit operator double(Percent i)
        {
            return (double)i.Decimal;
        }
        public static implicit operator int(Percent i)
        {
            return (int)(i * 100M);
        }

        public static implicit operator Percent(decimal i)
        {
            return new Percent(i);
        }


        public static implicit operator Percent(double i)
        {
            return new Percent((decimal)i);
        }

        public static implicit operator Percent(int i)
        {
            return new Percent(i);
        }

        public decimal Decimal { get; }

    }
}
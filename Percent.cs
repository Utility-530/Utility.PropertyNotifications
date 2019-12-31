using System;
public struct Percent
{
    public Percent(decimal val)
    {
         this.Decimal = val;
    }

    public Percent(double val) : this((decimal)val)
    { }


    public Percent(int percent)
    {
         this.Decimal = percent / 100m;
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
        return (int)(i*100M);
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


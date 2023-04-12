using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityModel
{
    public struct Fraction
    {
        public int Numerator { get; }

        public int Denominator { get; }

        public Fraction(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public Fraction(double number)
        {
            var fraction = FractionHelper.GetFraction(number);
            Numerator = fraction.Item1;
            Denominator = fraction.Item2;
        }
    }

    public static class FractionExtension
    {
        public static Fraction Invert(this Fraction fraction) => new Fraction(fraction.Denominator, fraction.Numerator);

    }

    public static class FractionHelper
    {

        //https://stackoverflow.com/questions/14320891/convert-percentage-to-nearest-fraction
        // answered Jan 14 '13 at 16:44    DasKrümelmonster
        public static (int, int) GetFraction(double value, double tolerance = 0.02)
        {
            double f0 = 1 / value;
            double f1 = 1 / (f0 - Math.Truncate(f0));

            int a_t = (int)Math.Truncate(f0);
            int a_r = (int)Math.Round(f0);
            int b_t = (int)Math.Truncate(f1);
            int b_r = (int)Math.Round(f1);
            int c = (int)Math.Round(1 / (f1 - Math.Truncate(f1)));

            if (Math.Abs(1.0 / a_r - value) <= tolerance)
                return (1, a_r);
            else if (Math.Abs(b_r / (a_t * b_r + 1.0) - value) <= tolerance)
                return (b_r, a_t * b_r + 1);
            else
                return (c * b_t + 1, c * a_t * b_t + a_t + c);
        }
    }
}

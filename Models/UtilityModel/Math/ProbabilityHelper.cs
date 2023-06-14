using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityModel
{

    public static class ProbabilityHelper
    {
        public static Probability GetFromMoneyLine(int value)
        {
            if (value < -100 || value > 100)
            {
                return (value < 0) ?
                       new Probability((-value) / (-(value) + 100m)) :
                    new Probability(100m / value + 100);
            }
            throw new ArgumentException("Value must be less than -100 or greater than 100.");
        }

        public static Probability GetFromEnglishOdd((int, int) value)
        {
            if (value.Item2 > 1 && value.Item1 >= 1)
            {
                return (((decimal)(value.Item2)) / value.Item1);
            }
            throw new ArgumentException("value must be greater than 1");
        }

        public static Probability GetFromEnglishOdd(Fraction value)
        {
            if (value.Denominator > 1 && value.Numerator >= 1)
            {
                return (((decimal)(value.Denominator)) / value.Numerator);
            }

            throw new ArgumentException("value must be greater than 1");
        }

        public static Probability GetFromEuropeanOdd(double value)
        {
            if (value > 1)
            {
                return new Probability(1d / (value - 1));
            }

            throw new ArgumentException("value must be greater than 1");
        }

        public static Probability GetFromEuropeanOdd(decimal value)
        {
            if (value > 1)
            {
                return new Probability(1m / (value - 1));
            }

            throw new ArgumentException("value must be greater than 1");
        }

        public static Probability GetFromPercent(int value)
        {
            if (value >= 0 && value <= 100)
            {
                new Probability(value);
            }
            throw new ArgumentException("Value must be less than 100 and greater than or equal to 0.");
        }
    }

}

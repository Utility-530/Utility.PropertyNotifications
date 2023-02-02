using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Structs
{
    public readonly struct Probability
    {
        public Probability(double val)
        {
            if (val < 1 && val > 0)
            {
                this.Value = val;
            }
            else
            {
                throw new ArgumentException("value must be between 0 and 1");
            }
        }

        public Probability(decimal val) : this((double)val)
        { }

        public Probability(int percent)
        {
            if (percent <= 100 && percent >= 0)
            {
                this.Value = percent / 100d;
            }
            else
            {
                throw new ArgumentException("value must be between 0 and 1");
            }
        }

        // User-defined conversion from Probability to decimal
        public static implicit operator decimal(Probability i)
        {
            return i.Decimal;
        }

        public static implicit operator double(Probability i)
        {
            return (double)i.Value;
        }

        // User-defined conversion from Probability to decimal
        public static implicit operator int(Probability i)
        {
            return (int)i.Percent;
        }

        public static implicit operator Percent(Probability i)
        {
            return i.Value;
        }

        public static implicit operator Probability(decimal i)
        {
            return new Probability(i);
        }

        public static implicit operator Probability(double i)
        {
            return new Probability((decimal)i);
        }

        public static implicit operator Probability(int i)
        {
            return new Probability(i);
        }

        public static implicit operator Probability(Percent i)
        {
            return new Probability(i);
        }

        public double Value { get; }
        public decimal Decimal => (decimal)Value;

        public double Percent => Value * 100d;

        /// <summary>
        /// EuropeanOdd
        /// </summary>
        public double EuropeanOdd => 1 / Value;

        public int MoneyLine
        {
            get
            {
                var percent = Percent;
                if (Value > 0.5d)
                    return (int)(-(percent / (100d - percent)) * 100);
                else
                    return (int)((100d - percent) / percent * 100);
            }
        }

        public (int, int) EnglishOdd
        {
            get
            {
                var fraction = GetFraction((double)Value);
                return (fraction.Item1 + fraction.Item2, fraction.Item2);
            }
        }

        //https://stackoverflow.com/questions/14320891/convert-percentage-to-nearest-fraction
        // answered Jan 14 '13 at 16:44    DasKrümelmonster
        private (int, int) GetFraction(double value, double tolerance = 0.02)
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

        public override string ToString()
        {
            return Value.ToString("N2") + " %";
        }
    }

    public static class ProbabilityEx
    {
        public static Probability GetFromMoneyLine(int value)
        {
            if (value < -100 || value > 10)
            {
                return (value < 0) ?
                       new Probability((-value) / (-(value) + 100m)) :
                    new Probability(100m / value + 100);
            }
            throw new ArgumentException("Value must be less than -100 or greater than 100.");
        }

        public static Probability GetFromEnglishOdd((int, int) value) => (((decimal)(value.Item1 + value.Item2)) / value.Item2) - 1;

        public static Probability GetFromEuropeanOdd(double value) => new(1 / value);

        public static Probability GetFromEuropeanOdd(decimal value) => new(1 / value);

        public static Probability GetFromPercent(int value) => new(value / 100m);

        public static Probability GetFromPercent(double value) => new(value / 100d);

        public static IEnumerable<(decimal dcml, Probability probability)> ToProbabilities(this ICollection<decimal> odds)
        {
            decimal sum = Sum();

            foreach (decimal o in odds)
            {
                yield return (o, new Probability(1 / (o * sum)));
            }

            decimal Sum()
            {
                decimal sum_ = 0;

                foreach (decimal o in odds)
                {
                    if (o < 1) throw new Exception("values are not odds ");
                    sum_ += 1 / o;
                }

                return sum_;
            }
        }

        /// <summary>
        /// returns  'perfect percent' - all percent sums to zero
        /// </summary>
        /// <param name="match"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static (Probability home, Probability draw, Probability away) GetPerfectProbability(Probability home, Probability draw, Probability away)
        {
            var margin = GetMargin(home, draw, away);
            return (home.Value / margin, draw.Value / margin, away.Value / margin);
        }

        /// <summary>
        /// Gets the remainder of the sums after dividing by zero.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static double GetMargin(Probability home, Probability draw, Probability away) => new[] { home, draw, away }.Sum(val => val.Value) - 1;
    }
}
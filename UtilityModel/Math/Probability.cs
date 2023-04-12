using System;

namespace UtilityModel
{
    public struct Probability
    {


        public Probability(decimal val) 
        {
            if (val <= 1 && val >= 0)
            {
                this.Decimal = val;
            }
            else
            {
                throw new ArgumentException("value must be between 0 and 1");
            }
        }

        public Probability(double val) : this((decimal)val)
        { }
  

        public Probability(int percent) 
        {
            if (percent <= 100 && percent >= 0)
            {
                this.Decimal = percent/100m;
            }
            else
            {
                throw new ArgumentException("value must be between 0 and 100");
            }
        }


        // User-defined conversion from Probability to decimal
        public static implicit operator decimal(Probability i)
        {
            return i.Decimal;
        }

        public static implicit operator Probability(decimal i)
        {
            return new Probability(i);
        }


        public static implicit operator Probability(double i)
        {
            return new Probability((decimal)i);
        }

        public decimal Decimal { get; }

        public int Percent => (int)(Decimal * 100m);

        /// <summary>
        /// EuropeanOdd
        /// </summary>
        public decimal EuropeanOdd => 1 + 1m/Decimal;


        public int MoneyLine
        {
            get
            {
                int percent = Percent;
                if (Decimal > 0.5m)
                    return (int)(-(percent / (100m - percent)) * 100);
                else
                    return (int)(((100m - percent) / percent) * 100);
            }
        }


        public Fraction EnglishOdd
        {
            get
            {
                var fraction = FractionHelper.GetFraction((double)Decimal);
                return new Fraction(fraction.Item2, fraction.Item1);
            }
        }

    }


}

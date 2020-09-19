

namespace UtilityStruct
{

    public struct Odd
    {
        public Odd(Probability value) : this(PriceType.Bid, value)
        {
        }

        public Odd(PriceType priceType, Probability value)
        {
            Type = priceType;
            Value = value;
        }

        public PriceType Type { get; }

        public Probability Value { get; }


        // User-defined conversion from Digit to double
        public static implicit operator double(Odd odd)
        {
            return (odd.Type == PriceType.Offer ? -1 : 1) * odd.Value.EuropeanOdd;
        }

        // User-defined conversion from Digit to double
        public static implicit operator decimal(Odd odd)
        {
            return (odd.Type == PriceType.Offer ? -1 : 1) * (decimal)odd.Value.EuropeanOdd;
        }

        public enum PriceType
        {
            Bid, Offer
        }

        public override string ToString()
        {
            return Value.ToString() + " " + Type.ToString();
        }
    }


    public static class OddExtension
    {
        public static bool IsOffer(this Odd odd) => odd.Type == Odd.PriceType.Offer;

        public static bool IsBid(this Odd odd) => odd.Type == Odd.PriceType.Bid;

        /// <summary>
        /// Portion of unit bet that is gained i.e the unit bet minus original stake
        /// </summary>
        /// <param name="odd"></param>
        /// <returns></returns>
        public static decimal AdjustedPrice(this Odd odd) => odd - 1m;
    }
}



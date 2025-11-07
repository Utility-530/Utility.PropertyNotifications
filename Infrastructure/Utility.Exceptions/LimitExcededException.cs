using System;

namespace Utility.Exceptions
{
    public class LimitExceededException : Exception
    {
        public LimitExceededException(int limit, int excess) : base($"Number of items exceeds limit ({limit}) by {excess}")
        {
            Limit = limit;
            Excess = excess;
            Data.Add("Limit", limit);
            Data.Add("Excess", excess);
        }

        public int Limit { get; set; }

        public int Excess { get; set; }
    }
}
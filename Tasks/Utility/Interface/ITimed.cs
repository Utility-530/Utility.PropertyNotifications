using System;

namespace Utility
{
    public interface ITimed
    {
        public DateTime Start { get; }

        public DateTime Finish { get; }
    }
}



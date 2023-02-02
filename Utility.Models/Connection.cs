using Utility.Models;
using Utility.Interfaces.Generic;

namespace Graph.Library
{
    public class Connection : IKey<Key>
    {
        public Key Key { get; init; }
        public Key Source { get; init; }
        public Key Target { get; init; }
    }
}

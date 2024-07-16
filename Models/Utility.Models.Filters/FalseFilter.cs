using Utility.Models;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models.Filters
{
    public class FalseFilter : Filter, IPredicate
    {
        public FalseFilter() : base("False")
        {
        }

        public override Property<bool> Value { get; } = new Property<bool>(false);

        public override bool Evaluate(object value)
        {
            return false;
        }
    }
}

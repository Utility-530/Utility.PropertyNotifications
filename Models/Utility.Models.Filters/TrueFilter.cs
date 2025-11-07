using Utility.Interfaces.NonGeneric;

namespace Utility.Models.Filters
{
    public class TrueFilter : Filter, IPredicate
    {
        public TrueFilter() : base("True")
        {
        }

        public override Property<bool> Value { get; } = new Property<bool>(true);

        public override bool Evaluate(object value)
        {
            return true;
        }
    }
}
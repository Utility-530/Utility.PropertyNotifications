using Utility.Models;
using Utility.Interfaces.NonGeneric;

namespace Utility.ViewModels.Filters
{
    public class TrueFilter : Filter, IPredicate
    {
        public TrueFilter() : base("True")
        {
        }
        public override Property<bool> Value { get; } = new Property<bool>(true);

        public override bool Invoke(object value)
        {
            return true;
        }
    }
}

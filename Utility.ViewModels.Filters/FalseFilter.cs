using Utility.Models;
using Utility.Interfaces.NonGeneric;

namespace Utility.ViewModels.Filters
{
    public class FalseFilter : Filter, IPredicate
    {
        public FalseFilter() : base("False")
        {
        }

        public override Property<bool> Value { get; } = new Property<bool>(false);

        public override bool Invoke(object value)
        {
            return false;
        }
    }
}

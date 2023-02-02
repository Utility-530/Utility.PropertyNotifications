using Utility.Helpers;

namespace UtilityWpf.Service
{
    public class ContainsFilter
    {
        private readonly string? a;
        private readonly string property;

        public ContainsFilter(string a)
        {
            this.a = a;
        }

        public ContainsFilter(string a, string? property = null)
        {
            this.property = property;
            this.a = a;
        }

        public bool Filter(object o) => property == null ?
            ((string)o).Contains(a, System.StringComparison.InvariantCultureIgnoreCase) :
            o.GetPropertyRefValue<string>(property)?.Contains(a, System.StringComparison.InvariantCultureIgnoreCase) ?? false;
    }
}
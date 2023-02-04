using UtilityWpf.Demo.Data.Model;
using Utility.Models.Filters;

namespace UtilityWpf.Demo.Data.Factory
{
    public class ProfileFilterCollection
    {
        public static readonly Filter[] Filters =
        {
            new TopFilter(5),
            new StringMatchFilter(),
            new PropertyFilter(nameof(Profile.IsAvailable)),
            new TrueFilter(),
            new FalseFilter(),
            new RandomFilter(),
        };
    }

    public class TopFilter : TopLimitFilter<Profile>
    {
        public TopFilter(int count) : base(count)
        {
        }
    }

    public class StringMatchFilter : StringMatchFilter<Profile>
    {
    }

    public class PropertyFilter : PropertyFilter<Profile>
    {
        public PropertyFilter(string property) : base(property)
        {
        }

        protected override object Set(string value)
        {
            return value;
        }
    }
}
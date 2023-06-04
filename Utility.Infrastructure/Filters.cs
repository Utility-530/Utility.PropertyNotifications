using System.Collections;
using System.ComponentModel;

namespace Utility.PropertyTrees.Infrastructure
{
    public abstract class Filters : IEnumerable<Predicate<PropertyDescriptor>>
    {
        public abstract IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
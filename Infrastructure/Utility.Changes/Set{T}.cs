using System.Collections.ObjectModel;

namespace Utility.Changes
{
    public class Set<T> : ReadOnlyCollection<Change<T>>
    {
        public Set(IList<Change<T>> list) : base(list)
        {
        }

        public Set(params Change<T>[] list) : base(list)
        {
        }

    }
}
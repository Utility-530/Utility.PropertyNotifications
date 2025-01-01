using System.Collections;

namespace Utility.Nodes.Filters
{
    public interface IChildrenSelector
    {

        public IEnumerable Select(object data);

    }
}

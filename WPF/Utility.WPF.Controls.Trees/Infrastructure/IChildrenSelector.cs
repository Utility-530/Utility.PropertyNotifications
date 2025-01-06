using System.Collections;

namespace Utility.WPF.Controls.Trees
{
    public interface IChildrenSelector
    {

        public IEnumerable Select(object data);

    }
}

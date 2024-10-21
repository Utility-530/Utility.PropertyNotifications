using System.Windows.Controls;

namespace Utility.Trees.WPF.Abstractions
{
    public interface ITreeViewItemFactory
    {
        ItemsControl Make(object instance, ItemsControl parent);

    }
}

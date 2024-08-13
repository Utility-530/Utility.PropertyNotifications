using System.Windows.Controls;

namespace Utility.Trees.WPF.Abstractions
{
    public interface ITreeViewItemFactory
    {
        HeaderedItemsControl Make(object instance, ItemsControl parent);

    }
}

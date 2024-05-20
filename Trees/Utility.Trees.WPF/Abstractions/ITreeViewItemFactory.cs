using System.Windows.Controls;

namespace Utility.Trees.WPF.Abstractions
{
    public interface ITreeViewItemFactory
    {
        TreeViewItem Make(object instance);

    }
}

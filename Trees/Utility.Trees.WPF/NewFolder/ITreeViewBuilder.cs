using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using Views.Trees;

namespace Utility.WPF.Nodes
{
    public interface ITreeViewBuilder
    {
        IDisposable Build(
            TreeView treeView,
            IItems root,
            ITreeViewItemFactory factory,
            IValueConverter ItemsPanelConverter,
            StyleSelector styleSelector,
            DataTemplateSelector dataTemplateSelector,
            ITreeViewFilter filter);

    }
}

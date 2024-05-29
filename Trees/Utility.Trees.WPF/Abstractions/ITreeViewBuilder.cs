using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.WPF.Abstractions
{
    public interface ITreeViewBuilder
    {
        IDisposable Build(
            ItemsControl treeView,
            IItems root,
            ITreeViewItemFactory factory,
            IValueConverter ItemsPanelConverter,
            StyleSelector styleSelector,
            DataTemplateSelector dataTemplateSelector,
            ITreeViewFilter filter);

    }
}

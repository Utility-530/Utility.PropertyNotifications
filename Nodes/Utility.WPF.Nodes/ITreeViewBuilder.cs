using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Views.Trees
{
    public interface ITreeViewBuilder
    {
        IDisposable Build(TreeView treeView, object root, ITreeViewItemFactory factory, IValueConverter ItemsPanelConverter, DataTemplateSelector dataTemplateSelector, ITreeViewFilter filter);

    }
}

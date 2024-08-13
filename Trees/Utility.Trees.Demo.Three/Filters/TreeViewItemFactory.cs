using AnyClone;
using System;
using System.Collections;
using System.Windows.Controls;
using Utility.Commands;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.Trees.WPF.Abstractions;
using Utility.WPF.Controls.Trees;

namespace Utility.Trees.Demo.MVVM.MVVM
{

    public class TreeViewItemFactory : ITreeViewItemFactory
    {
        Random random = new();
        public HeaderedItemsControl Make(object instance, ItemsControl parent)
        {
            HeaderedItemsControl item = null;
            if (instance is Node node && node.Data as ICollectionDescriptor != null && (node.Data as ICollectionDescriptor).ElementType == typeof(Table))
            {

                item = new ComboTreeViewItem
                {
                    Header = instance,
                    DataContext = instance,
                    IsExpanded = true
                };
            }
            else
            {
                item = new CustomTreeViewItem
                {
                    //AddCommand = new Command(() => { if (instance is ITree { } item) item.Add(new ModelTree(Helpers.Names.Random(random), Guid.NewGuid(), ((GuidKey)item.Key).Value)); }),
                    //RemoveCommand = new Command(() => { if (instance is IParent<ITree> { Parent: { } parent }) parent.Remove(instance); }),
                    AddCommand = new Command(() => Add(instance)),
                    RemoveCommand = new Command(() => Remove(instance)),
                    Header = instance,
                    DataContext = instance,
                    IsExpanded = true
                };
            }
            if(item is TreeViewItem treeViewItem)
            {
                treeViewItem.Selected += (s,e)=> { if (parent is ISelection selection) selection.Selection = treeViewItem; };
            }
            return item;
        }


        private void Remove(object instance)
        {

        }

        private void Add(object instance)
        {
            if (instance is ITree { Parent: ITree {  } tree, Data: IInstance { Instance: { } _instance } })
                if (tree is { Data: IInstance { Instance: IList lst } data } item &&
                    tree is { Data: ICollectionDetailsDescriptor { CollectionItemPropertyType: { } type } })
                {
                    var newInstance = Activator.CreateInstance(type);
                    _instance.CloneTo(newInstance);
                    lst.Add(newInstance);
                    if (data is IRefresh refresh)
                    {
                        refresh.Refresh();
                    }
                }
        }

        public static TreeViewItemFactory Instance { get; } = new();
    }
}

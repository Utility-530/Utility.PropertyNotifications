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
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Trees.WPF.Abstractions;
using Utility.WPF.Controls.Trees;

namespace Utility.Trees.Demo.MVVM.MVVM
{

    public class TreeViewItemFactory : ITreeViewItemFactory
    {
        Random random = new();

        private TreeViewItemFactory()
        {
            Predicate = new TreeViewItemDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
            {
                new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null && (item.Data as ICollectionDescriptor).ElementType == typeof(Table)),
                    instance=>
                        new ComboTreeViewItem
                        {
                            Header = instance,
                            DataContext = instance,
                            IsExpanded = true
                        }),
                    new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item =>true){  }, instance =>
                    new CustomTreeViewItem
                    {
                        //AddCommand = new Command(() => { if (instance is ITree { } item) item.Add(new ModelTree(Helpers.Names.Random(random), Guid.NewGuid(), ((GuidKey)item.Key).Value)); }),
                        //RemoveCommand = new Command(() => { if (instance is IParent<ITree> { Parent: { } parent }) parent.Remove(instance); }),
                        AddCommand = new Command(() => Add(instance)),
                        RemoveCommand = new Command(() => Remove(instance)),
                        Header = instance,
                        DataContext = instance,
                        IsExpanded = true
                    }),
            };
        }

        public DecisionTree Predicate { get; set; }

        public HeaderedItemsControl Make(object instance, ItemsControl parent)
        {
            Predicate.Reset();
            Predicate.Input = instance;
            Predicate.Evaluate();
            if (Predicate.Backput is TreeViewItem treeViewItem)
            {
                treeViewItem.Selected += (s, e) => { if (parent is ISelection selection) selection.Select(treeViewItem.Header); };
                return treeViewItem;
            }
            throw new Exception("DS 3333");
        }

        private void Remove(object instance)
        {

        }

        private void Add(object instance)
        {
            if (instance is ITree { Parent: ITree { } tree, Data: IInstance { Instance: { } _instance } })
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

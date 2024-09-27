using AnyClone;
using Splat;
using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.Commands;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Reflections;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Trees.WPF;
using Utility.Trees.WPF.Abstractions;
using Utility.WPF.Controls.Trees;
using Views.Trees;

namespace Utility.Trees.Demo.MVVM.MVVM
{

    public class TreeViewItemFactory : ITreeViewItemFactory
    {
        public static readonly Guid _guid = Guid.Parse("a996b3a6-3e73-4e93-8b34-cf324aac5749");

        Random random = new();

        private TreeViewItemFactory()
        {
            Predicate = new TreeViewItemDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
            {
                new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null))
                {
                      new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Data as ICollectionDescriptor).ElementType == typeof(Table)),
                      instance =>
                      {
                        var table = Activator.CreateInstance(typeof(Table));
                        var root = DescriptorFactory.CreateRoot(table, _guid, "table_add").Take(1).Wait();
                        var reflectionNode = new ReflectionNode(root);
                        var item =  new ComboTreeViewItem
                        {
                            Header = instance,
                            DataContext = instance,
                            NewObject = DataTreeViewer(reflectionNode),
                            IsExpanded = true
                        };
                        item.FinishEdit += Item_FinishEdit;

                        void Item_FinishEdit(object sender, NewObjectRoutedEventArgs e)
                        {
                            if(e.IsAccepted)
                            {
                                var newObject = (e.NewObject as TreeViewer).ViewModel as ReflectionNode;
                                var root= newObject.Data as IValueDescriptor;
                                var inst = root.Get();
                                var cd = ((instance as IReadOnlyTree).Data as ICollectionDescriptor).Collection as IList;
                                cd.Add(inst);
                                
                                ((instance as IReadOnlyTree).Data as IRefresh).Refresh();
                            }
                        }
                        return item;
                        })
                },
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

        public static TreeViewer DataTreeViewer(object data)
        {
            return new TreeViewer
            {
                ViewModel = data,
                TreeViewItemFactory = TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = ItemsPanelConverter.Instance,
                DataTemplateSelector = DataTemplateSelector.Instance,
                TreeViewFilter = Filter.Instance,
                StyleSelector = StyleSelector.Instance,
                EventListener = EventListener.Instance
            };
        }
    }
}

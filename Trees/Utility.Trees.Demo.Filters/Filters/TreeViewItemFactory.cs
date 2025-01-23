using AnyClone;
using Splat;
using System;
using System.Collections;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.Commands;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Reflections;
using Utility.Repos;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.Trees.Demo.Filters.Infrastructure;
using Utility.Trees.WPF;
using Utility.Trees.WPF.Abstractions;
using Utility.WPF.Controls.Trees;
using Views.Trees;

namespace Utility.Trees.Demo.Filters
{
    public class TreeViewItemFactory : ITreeViewItemFactory
    {
        private TreeViewItemFactory()
        {
            Predicate = new TreeViewItemDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
            {
                new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionDescriptor != null))
                {
                      new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Data as ICollectionDescriptor).ElementType == typeof(Table)),
                      instance =>
                      {

                        var guid = (((ITree)instance).Data as IDescriptor).Guid;
                        var item = new ComboTreeViewItem
                        {
                                Header = instance,
                                DataContext = instance,
                                IsExpanded = true
                        };

                        Locator.Current.GetService<ITreeRepository>()
                        .Find(guid, "table_add", typeof(Table))
                        .Subscribe(_guid =>
                        {
                            var table = (Table)Activator.CreateInstance(typeof(Table));
                            var root = DescriptorFactory.CreateRoot(table, _guid.Guid, "table_add").Take(1).Wait();
                            var reflectionNode = new ReflectionNode(root) { Parent = (ITree)instance };
                            item.NewObject = DataTreeViewer(reflectionNode);
                            item.FinishEdit += Item_FinishEdit;
                        });
                        void Item_FinishEdit(object sender, NewObjectRoutedEventArgs e)
                        {
                            if(e.IsAccepted)
                            {
                                var newObject = (e.NewObject as TreeViewer).ViewModel as ReflectionNode;
                                var root= newObject.Data as IValueDescriptor;
                                var inst = root.Get();
                                if(instance is IReadOnlyTree tree)
                                {
                                    var cd = (tree.Data as ICollectionDescriptor).Collection as IList;
                                    cd.Add(inst);
                                    //root.Initialise();
                                    (tree.Data as IRefresh).Refresh();
                                }
                            }
                        }
                        return item;
                        })
                },
                new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item =>(item.Data as IReferenceDescriptor)!=null))
                {
                  new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Data as ICollectionItemReferenceDescriptor)!=null))
                  {
                    new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Data as ICollectionItemReferenceDescriptor).Type == typeof(Table)),
                    instance =>
                    {
                        return new CustomTreeViewItem
                        {
                            //AddCommand = new Command(() => { if (instance is ITree { } item) item.Add(new ModelTree(Helpers.Names.Random(random), Guid.NewGuid(), ((GuidKey)item.Key).Value)); }),
                            //RemoveCommand = new Command(() => { if (instance is IParent<ITree> { Parent: { } parent }) parent.Remove(instance); }),

                            Header = instance,
                            DataContext = instance,
                            IsExpanded = true
                        };
                    }),
                  },
                    new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Data as IReferenceDescriptor).Type == typeof(Table)))
                    {

                        new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => item.Parent as ICollectionDescriptor!=null), instance =>
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

                        new TreeViewItemDecisionTree(new Decision<IReadOnlyTree>(item => (item.Parent as ICollectionDescriptor)==null),
                        instance =>
                        {
                            var tree = (instance as IReadOnlyTree);
                            var data = tree?.Data as IDescriptor;
                            var table = (data as IValue)?.Value as Table;
                            if(table is { Type: { } type, Name:{ } name })
                            {
                                var treeViewer = DataTreeViewer();
                                Locator.Current.GetService<ITreeRepository>()
                                .Find(data.Guid, "branch")
                                .Subscribe(_guid =>
                                {
                                    var root =  DescriptorFactory.CreateRoot(table.Type , _guid.Guid, table.Name).Take(1).Wait();
                                    var reflectionNode = new ReflectionNode(root);
                                    treeViewer.ViewModel = reflectionNode;
                                });

                                return treeViewer;
                            }
                            else if ((instance as IReadOnlyTree)?.Parent.Data is not ICollectionDescriptor)
                            {
                                var treeViewer = DataTreeViewer();
                                ((instance as IReadOnlyTree)?.Data as IValueChanges)
                                .Subscribe(change =>
                                {
                                    var tree = (instance as IReadOnlyTree);
                                    var data = tree?.Data as IDescriptor;
                                    var table = (data as IValue)?.Value as Table;
                                    if(table is { Type: { } type, Name:{ } name })
                                    {
                                        var root =  DescriptorFactory.CreateRoot(table.Type , data.Guid, table.Name).Take(1).Wait();
                                        var reflectionNode = new ReflectionNode(root);
                                        treeViewer.ViewModel = reflectionNode;
                                    }
                            });
                                return treeViewer;
                            }
                            else
                            {
                                return new CustomTreeViewItem
                                {
                                //AddCommand = new Command(() => { if (instance is ITree { } item) item.Add(new ModelTree(Helpers.Names.Random(random), Guid.NewGuid(), ((GuidKey)item.Key).Value)); }),
                                //RemoveCommand = new Command(() => { if (instance is IParent<ITree> { Parent: { } parent }) parent.Remove(instance); }),

                                Header = instance,
                                DataContext = instance,
                                IsExpanded = true
                                };
                            }
                        })
                    }
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

        public ItemsControl Make(object instance, ItemsControl parent)
        {
            Predicate.Reset();
            Predicate.Input = instance;
            Predicate.Evaluate();
            if (Predicate.Backput is TreeViewItem treeViewItem)
            {
                treeViewItem.Selected += (s, e) => { if (parent is ISelection selection) selection.Select(treeViewItem.Header); };

            }
            if (Predicate.Backput is ItemsControl itemsControl)
            {
                return itemsControl;
            }
            throw new Exception("DS 3333");
        }

        private void Remove(object instance)
        {
            {
                if (instance is ITree { Parent: ITree { } tree, Data: IInstance { Instance: { } _instance } })
                    if (tree is { Data: IInstance { Instance: IList lst } data } item)
                    {
      
                        lst.Remove(_instance);
                        if (data is IRefresh refresh)
                        {
                            refresh.Refresh();
                        }
                    }
            }
        }

        private void Add(object instance)
        {
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
            {

                if (instance is ITree { Data: IInstance { Instance: IList lst } data } tree &&
                    tree is { Data: ICollectionDetailsDescriptor { CollectionItemPropertyType: { } type } })
                {
                    var newInstance = Activator.CreateInstance(type);
                    lst.Add(newInstance);
                    if (data is IRefresh refresh)
                    {
                        refresh.Refresh();
                    }
                }
            }
        }

        public static TreeViewItemFactory Instance { get; } = new();

        public static TreeViewer DataTreeViewer(object? data = null)
        {
            return new TreeViewer
            {
                ViewModel = data,
                TreeViewItemFactory = TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = ItemsPanelConverter.Instance,
                DataTemplateSelector = DataTemplateSelector.Instance,
                TreeViewFilter = TreeViewFilter.Instance,
                StyleSelector = StyleSelector.Instance,
                EventListener = EventListener.Instance
            };
        }
    }
}

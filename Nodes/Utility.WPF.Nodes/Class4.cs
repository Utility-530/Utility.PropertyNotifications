//using Infrastructure;
//using Trees;
//using System;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Reactive.Linq;
//using System.Collections.Generic;
//using System.Reactive.Disposables;

//namespace Views.Trees
//{
//    public class ViewModelTreeViewBuilder : ITreeViewBuilder
//    {
//        public IDisposable Build(TreeView treeView, object rootViewModel, ITreeViewItemFactory factory, IValueConverter ItemsPanelConverter, DataTemplateSelector dataTemplateSelector)
//        {
//            return Tre#eExtensions.ExploreTree(
//                treeView.Items,
//                (itemcollection, viewModel) =>
//                {
//                    var item = factory.Make();
//                    item.Header = viewModel;
//                    item.DataContext = viewModel;
//                    item.HeaderTemplateSelector = dataTemplateSelector;
//                    SetBinding(ItemsPanelConverter, item);
//                    itemcollection.Add(item);
//                    return item.Items;
//                },
//                (a, b) => a.Remove(new TreeViewItem { Header = b }),
//                rootViewModel,
//                vm =>
//                {
//                    return Observable.Create<ChangeSet>(obs =>
//                    {
//                        var type = vm.GetType();
//                        var properties = type.GetProperties();
//                        List<Change> changes = new();
//                        foreach (var property in properties)
//                        {
//                            //changes.Add(new Change { IsAdd = true, Value = property.GetValue(vm) });
//                        }
//                        obs.OnNext(new ChangeSet { Changes = changes });
//                        return Disposable.Empty;
//                    });
//                });

//            static void SetBinding(IValueConverter ItemsPanelConverter, TreeViewItem item)
//            {
//                var binding = new Binding()
//                {
//                    //Source = new PropertyPath()
//                    Converter = ItemsPanelConverter,
//                    Mode = BindingMode.OneTime
//                };

//                item.SetBinding(ItemsControl.ItemsPanelProperty, binding);
//            }
//        }


//        public static ViewModelTreeViewBuilder Instance { get; } = new();
//    }

//}

using System.Reactive.Linq;
using System;
using System.Windows;
using Utility.Trees.Demo.MVVM.MVVM;
using Utility.WPF.Reactives;
using Views.Trees;
using Utility.Infrastructure;
using Utility.Trees.Abstractions;
using Utility.Descriptors;
using Utility.Nodes.Reflections;
using Utility.Trees.WPF;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Linq;
using Utility.Trees.Demo.MVVM.Views;
using Utility.WPF.Templates;
using static Utility.Helpers.EnumHelper;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Repos;
using Splat;
using Utility.Reactives;
using Utility.Extensions;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Trees;
using System.Collections.Generic;
using Utility.Trees.Demo.MVVM.Infrastructure;
using System.ComponentModel.Design;

namespace Utility.Trees.Demo.MVVM
{

    public class Table
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public Type Type { get; set; }
    }

    public class Model
    {
        public List<Table> Tables { get; set; } = new();
        public Table SelectedTable { get; set; }
    }

    public partial class App
    {
        ReflectionNode model;

        public void InitialiseModel()
        {
            DescriptorFactory.CreateRoot(Locator.Current.GetService<Model>(), Guid.Parse("76bca65d-6496-4a45-84f9-87705e665599"), "model")
                .Subscribe(a =>
            {
                model = new ReflectionNode(a);
                UpdateModel(model);
                Initialise();
            });
        }


        //public void InitialiseModelOld()
        //{
        //    MakeComboBox();
        //    Initialise();

        //    MainView.Instance.combobox
        //    .ValueChanges()
        //    .Cast<Utility.Repos.Key>()
        //    // type can be null because type from another assembly not loaded 
        //    .Where(a => a.Instance != null)
        //    .Subscribe(a =>
        //    {

        //        var model = CreateModel(a.Instance.GetType(), a.Name, a.Guid);
        //        UpdateModel(model);
        //        //disposable?.Dispose();
        //        //disposable = Disposable();
        //    });


        //    ReflectionNode CreateModel(Type type, string name, Guid guid)
        //    {
        //        model = new ReflectionNode(DescriptorFactory.CreateRoot(type, guid, name).GetAwaiter().GetResult()) { };
        //        return model;
        //    }
        //}



        void Initialise()
        {
            AutoCompleteConverter.Instance.Subscribe(a =>
            {
                var _ = model;

                if (a is SuggestionPrompt { Value: IDescriptor { ParentType: { } type } value, Filter: { } filter })
                {
                    //if(type == typeof(Connection) && model is Diagram)
                    //{

                    //}
                }
            });
            //viewModel = new() { Key = model.Key };
            //view = new() { Key = model.Key };
            //data = new() { Key = model.Key };

            //var uGrid = new UniformGrid() { Rows = 1 };

            EventListener.Instance.Subscribe(a =>
            {
                if (a is ClickChange { Node: IReadOnlyTree { Data: MemberDescriptor data } tree, Source: { } source })
                {
                    if (data.Name == "Table")
                    {
                        var model = Locator.Current.GetService<Model>();
                        //var node = (Utility.Extensions.TreeExtensions.MatchDescendant(model, a => (a.Data is MemberDescriptor { Name: "SelectedTable" }))).MatchDesc;
                        model.SelectedTable = ((data as ICollectionItemReferenceDescriptor).Instance as Table);
                        //pipeRepository.Get((node.Data as MemberDescriptor).Guid);

                        //Pipe.Instance.Queue(new QueueItem(default, ParentGuid: Guid.Parse("1f51406b-9e37-429d-8ed2-c02cff90cfdb")));
                        Pipe.Instance.Queue(new QueueItem(default, ParentGuid: Guid.Parse("72022097-e5f6-4767-a18a-50763514ca01")));
                    }
                    MainView.Instance.propertygrid.Content = new PropertyGrid { SelectedObject = data };
                    Filter.Instance.Convert(tree);
                    DataTemplateSelector.Instance.SelectTemplate(tree, null);
                    StyleSelector.Instance.SelectStyle(source, null);
                }
            });

            ReflectionNode? last = null;

            Pipe.Instance.WithChangesTo(a => a.Next).Skip(1).Subscribe(a =>
            {
                if (a is QueueItem { Guid: { } guid } qi)
                {
                    var d = TreeExtensions.MatchDescendant(model, a => (a.Data as MemberDescriptor).Guid == guid);
                    if (last is ReflectionNode node)
                    {
                        node.IsSelected = false;
                        node.RaisePropertyChanged(nameof(ViewModelTree.IsSelected));

                    }
                    if (d is ReflectionNode tree)
                    {
                        last = tree;
                        tree.IsSelected = true;
                        tree.RaisePropertyChanged(nameof(ViewModelTree.IsSelected));
                    }
                }
            });

            if (false)
            {
                //uGrid.Children.Add(ViewModelTreeViewer(viewModel));
                //uGrid.Children.Add(ViewTreeViewer(view));
            }

        }

        void UpdateModel(object data)
        {
            var treeViewer = DataTreeViewer(data);
            MainView.Instance.scrollviewer.Content = treeViewer;
            MainView.Instance.PipeView.DataContext = Splat.Locator.Current.GetService<PipeController>();
            MainView.Instance.filtertree.Content = treeViewer;
            MainView.Instance.filtertree.ContentTemplate = this.Resources["TVF"] as DataTemplate;
            MainView.Instance.datatemplatetree.Content = treeViewer;
            MainView.Instance.datatemplatetree.ContentTemplate = this.Resources["DTS"] as DataTemplate;
            MainView.Instance.styletree.Content = treeViewer;
            MainView.Instance.styletree.ContentTemplate = this.Resources["SS"] as DataTemplate;
        }

        void MakeComboBox()
        {
            var rootKeys = Splat.Locator.Current.GetService<ITreeRepository>().SelectKeys().GetAwaiter().GetResult();
            MainView.Instance.combobox.ItemsSource = rootKeys;
            MainView.Instance.combobox.DisplayMemberPath = nameof(Utility.Repos.Key.Name);
            MainView.Instance.combobox.SelectedIndex = 0;
        }


        //IDisposable Disposable()
        //{
        //    CompositeDisposable disposable = new();

        //    if (false)
        //        //view model
        //        model.Subscribe((a =>
        //        {
        //            if (a.Type == Changes.Type.Add)
        //            {
        //                var clone = a.Value.Key;
        //                var x = new ViewModelTree { Key = clone };
        //                var parentMatch = TreeExtensions.MatchDescendant(viewModel, (d => d.Key?.Equals(a.Value.Parent?.Key) == true)) as Tree;
        //                if (parentMatch != null)
        //                    parentMatch.Add(x);
        //            }
        //            if (a.Type == Changes.Type.Remove)
        //            {
        //                var match = TreeExtensions.MatchDescendant(viewModel, (d => d.Key.Equals(a.Value.Key))) as Tree;
        //                match.Parent.Remove(a.Value);
        //            }
        //        })).DisposeWith(disposable);


        //    // View
        //    if (false)
        //        model.Subscribe(a =>
        //        {
        //            if (a.Type == Changes.Type.Add)
        //            {
        //                var clone = a.Value.Key;
        //                var guid = ((GuidKey)a.Value.Key)?.Value;
        //                if (guid == null)
        //                    return;
        //                var type = TreeRepository.Instance.GetType(guid.Value, nameof(Model));
        //                Tree tree = null;
        //                //if (type != null)
        //                //{
        //                //    var instance = Activator.CreateInstance(type);
        //                //    //var rootDescriptor = new RootDescriptor(type);
        //                //    //var data = await DescriptorFactory.ToValue(instance, rootDescriptor, guid.Value);
        //                //    //var reflectionNode = new ReflectionNode(data);
        //                //    //reflectionNode.RefreshChildrenAsync();

        //                //    tree = new Tree { Key = clone, Data = instance };

        //                //}
        //                //else
        //                //{
        //                //    tree = new Tree { Key = clone, Data = null };
        //                //}
        //                tree = new Tree { Key = clone, Data = null };

        //                var parentMatch = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Parent?.Key))) as Tree;
        //                if (parentMatch != null)
        //                    parentMatch.Add(tree);
        //            }
        //            if (a.Type == Changes.Type.Remove)
        //            {
        //                var match = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Key))) as Tree;
        //                match.Parent.Remove(a.Value);
        //            }
        //        }).DisposeWith(disposable);

        //    // Data
        //    model.Subscribe(a =>
        //    {
        //        if (a.Type == Changes.Type.Add)
        //        {
        //            var clone = a.Value.Key;
        //            //var guid = ((GuidKey)a.Value.Key)?.Value;
        //            //if (guid == null)
        //            //    return;
        //            //var type = TreeRepository.Instance.GetType(guid.Value, nameof(Model));

        //            Tree tree = null;
        //            //object? instance = default;
        //            //if (type != null)
        //            //{
        //            //    var types = ValueDataTemplateSelector.Instance.Types;

        //            //    if (ValueDataTemplateSelector.Instance.Types.Contains(type))
        //            //    {
        //            //        var value = TreeRepository.Instance.Get(guid.Value);
        //            //        if (value == null)
        //            //        {
        //            //            instance = Activator.CreateInstance(type);
        //            //        }
        //            //        else
        //            //        {
        //            //            instance = value.Value;
        //            //        }
        //            //    }              
        //            //}
        //            tree = new Tree { Key = clone, Data = a.Value.Data };

        //            var parentMatch = TreeExtensions.MatchDescendant(data, (d => d.Key.Equals(a.Value.Parent?.Key))) as Tree;
        //            parentMatch?.Add(tree);
        //        }
        //        if (a.Type == Changes.Type.Remove)
        //        {
        //            var match = TreeExtensions.MatchDescendant(data, (d => d.Key.Equals(a.Value.Key))) as Tree;
        //            match?.Parent?.Remove(a.Value);
        //        }
        //    }).DisposeWith(disposable);
        //    return disposable;
        //}

        //public static TreeViewer ModelTreeViewer(object model)
        //{
        //    return new TreeViewer
        //    {
        //        ViewModel = model,
        //        TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
        //        TreeViewBuilder = TreeViewBuilder.Instance,
        //        PanelsConverter = Default.ItemsPanelConverter.Instance,
        //        DataTemplateSelector = Default.DataTemplateSelector.Instance,
        //        TreeViewFilter = Model.Filter.Instance,
        //        StyleSelector = Model.StyleSelector.Instance,
        //        EventListener = Default.EventListener.Instance
        //    };
        //}

        //public static TreeViewer ViewModelTreeViewer(object viewModel)
        //{
        //    return new TreeViewer
        //    {
        //        ViewModel = viewModel,
        //        TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
        //        TreeViewBuilder = TreeViewBuilder.Instance,
        //        PanelsConverter = Default.ItemsPanelConverter.Instance,
        //        DataTemplateSelector = Default.DataTemplateSelector.Instance,
        //        TreeViewFilter = Default.Filter.Instance,
        //        StyleSelector = ViewModel.StyleSelector.Instance,
        //        EventListener = Default.EventListener.Instance
        //    };
        //}

        //public static TreeViewer ViewTreeViewer(object view)
        //{
        //    return new TreeViewer
        //    {
        //        ViewModel = view,
        //        TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
        //        TreeViewBuilder = TreeViewBuilder.Instance,
        //        PanelsConverter = View.ItemsPanelConverter.Instance,
        //        DataTemplateSelector = View.DataTemplateSelector.Instance,
        //        StyleSelector = View.StyleSelector.Instance,
        //        TreeViewFilter = Default.Filter.Instance,
        //        EventListener = Default.EventListener.Instance
        //    };
        //}

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

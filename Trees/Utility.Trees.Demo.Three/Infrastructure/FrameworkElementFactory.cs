using System.Reactive.Linq;
using System;
using System.Windows;
using Utility.Descriptors.Repositorys;
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

namespace Utility.Trees.Demo.MVVM
{
    public partial class App
    {
        public Window CreateWindow()
        {
            //IDisposable? disposable = null;


            MakeComboBox();


            var window = new Window { Content = MainView.Instance };


            MainView.Instance.combobox
           .ValueChanges()
           .Cast<Descriptors.Repositorys.Key>()
           // type can be null because type from another assembly not loaded 
           .Where(a => a.Type != null)
           .Subscribe(a =>
           {

               MakeGrid(a.Type, a.Name, a.Guid);
               //disposable?.Dispose();
               //disposable = Disposable();
           });

            return window;
        }

        void MakeGrid(Type type, string name, Guid guid)
        {
            model = new ReflectionNode(DescriptorFactory.CreateRoot(type, guid, name).GetAwaiter().GetResult()) { };


            AutoCompleteConverter.Instance.Subscribe(a =>
            {
                var _ = model;
            
                if(a is SuggestionPrompt { Value: IDescriptor { ParentType:{ } type } value, Filter: { } filter })
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
                if (a is ClickChange { Node: IReadOnlyTree { Data: MemberDescriptor data } tree, Source:{ } source })
                {
                    MainView.Instance.propertygrid.Content = new PropertyGrid { SelectedObject = data };
                    Filter.Instance.Convert(tree);
                    DataTemplateSelector.Instance.SelectTemplate(tree, null);
                    StyleSelector.Instance.SelectStyle(source, null);               
                }
            });

   
            if (false)
            {
                //uGrid.Children.Add(ViewModelTreeViewer(viewModel));
                //uGrid.Children.Add(ViewTreeViewer(view));
            }
            var x = DataTreeViewer(model); ;

            MainView.Instance.scrollviewer.Content = x;


            //var _tr = ModelTreeViewer(model);

            //_tr.Style = this.Resources["A"] as Style;
       
            //MainView.Instance.modeltree_ScrollViewer.Content = _tr;

            MainView.Instance.filtertree.Content = x;
            MainView.Instance.filtertree.ContentTemplate = this.Resources["TVF"] as DataTemplate;
            MainView.Instance.datatemplatetree.Content = x;
            MainView.Instance.datatemplatetree.ContentTemplate = this.Resources["DTS"] as DataTemplate;
            MainView.Instance.styletree.Content = x;
            MainView.Instance.styletree.ContentTemplate = this.Resources["SS"] as DataTemplate;


        }

        void MakeComboBox()
        {
            var rootKeys = TreeRepository.Instance.SelectKeys().GetAwaiter().GetResult();
            MainView.Instance.combobox.ItemsSource = rootKeys;
            MainView.Instance.combobox.DisplayMemberPath = nameof(Descriptors.Repositorys.Key.Name);
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

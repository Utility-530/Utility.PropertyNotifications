using System.Reactive.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Descriptors.Repositorys;
using Utility.Trees.Demo.MVVM.MVVM;
using Utility.WPF.Reactives;
using Views.Trees;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Extensions;
using Utility.Keys;
using System.Reactive.Disposables;

namespace Utility.Trees.Demo.MVVM
{
    public partial class App
    {

        public Window CreateWindow()
        {

            DockPanel dockPanel = new();
            ComboBox comboBox = MakeComboBox(dockPanel);

            var window = new Window { Content = dockPanel };



            return window;
        }

        Panel MakeGrid(Type type)
        {
            model = new RootNode();
            model.Initialise(type).GetAwaiter().GetResult();
            viewModel = new() { Key = model.Key };
            view = new() { Key = model.Key };
            data = new() { Key = model.Key };



            var grid = new UniformGrid() { Rows = 1 };
            grid.Children.Add(ModelTreeViewer(model));
            grid.Children.Add(ViewModelTreeViewer(viewModel));
            grid.Children.Add(ViewTreeViewer(view));
            grid.Children.Add(DataTreeViewer(data));
            return grid;
        }

        ComboBox MakeComboBox(DockPanel dockPanel)
        {
            var rootKeys = TreeRepository.Instance.SelectKeys().GetAwaiter().GetResult();
            ComboBox comboBox = new()
            {
                ItemsSource = rootKeys,
                DisplayMemberPath = nameof(Descriptors.Repositorys.Key.Name),
                SelectedIndex = 0
            };

            DockPanel.SetDock(comboBox, Dock.Top);
            dockPanel.Children.Add(comboBox);

            IDisposable? disposable = null;
            comboBox
                .ValueChanges()
                .Cast<Descriptors.Repositorys.Key>()
                // type can be null because type from another assembly not loaded 
                .Where(a => a.Type != null)
                .Subscribe(a =>
                {
                    if (dockPanel.Children.Count > 1)
                        dockPanel.Children.RemoveAt(1);
                    dockPanel.Children.Add(MakeGrid(a.Type));
                    disposable?.Dispose();
                    disposable = Disposable();
                });

            return comboBox;
        }


        IDisposable Disposable()
        {
            CompositeDisposable disposable = new();
            model.Subscribe((a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    var clone = a.Value.Key;
                    var x = new ViewModelTree { Key = clone };
                    var parentMatch = TreeExtensions.MatchDescendant(viewModel, (d => d.Key?.Equals(a.Value.Parent?.Key) == true)) as Tree;
                    if (parentMatch != null)
                        parentMatch.Add(x);
                }
                if (a.Type == Changes.Type.Remove)
                {
                    var match = TreeExtensions.MatchDescendant(viewModel, (d => d.Key.Equals(a.Value.Key))) as Tree;
                    match.Parent.Remove(a.Value);
                }
            })).DisposeWith(disposable);


            // View
            model.Subscribe(a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    var clone = a.Value.Key;
                    var guid = ((GuidKey)a.Value.Key)?.Value;
                    if (guid == null)
                        return;
                    var type = TreeRepository.Instance.GetType(guid.Value, nameof(Model));
                    Tree tree = null;
                    //if (type != null)
                    //{
                    //    var instance = Activator.CreateInstance(type);
                    //    //var rootDescriptor = new RootDescriptor(type);
                    //    //var data = await DescriptorFactory.ToValue(instance, rootDescriptor, guid.Value);
                    //    //var reflectionNode = new ReflectionNode(data);
                    //    //reflectionNode.RefreshChildrenAsync();

                    //    tree = new Tree { Key = clone, Data = instance };

                    //}
                    //else
                    //{
                    //    tree = new Tree { Key = clone, Data = null };
                    //}
                    tree = new Tree { Key = clone, Data = null };

                    var parentMatch = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Parent?.Key))) as Tree;
                    if (parentMatch != null)
                        parentMatch.Add(tree);
                }
                if (a.Type == Changes.Type.Remove)
                {
                    var match = TreeExtensions.MatchDescendant(view, (d => d.Key.Equals(a.Value.Key))) as Tree;
                    match.Parent.Remove(a.Value);
                }
            }).DisposeWith(disposable);

            // Data
            model.Subscribe(a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    var clone = a.Value.Key;
                    //var guid = ((GuidKey)a.Value.Key)?.Value;
                    //if (guid == null)
                    //    return;
                    //var type = TreeRepository.Instance.GetType(guid.Value, nameof(Model));

                    Tree tree = null;
                    //object? instance = default;
                    //if (type != null)
                    //{
                    //    var types = ValueDataTemplateSelector.Instance.Types;

                    //    if (ValueDataTemplateSelector.Instance.Types.Contains(type))
                    //    {
                    //        var value = TreeRepository.Instance.Get(guid.Value);
                    //        if (value == null)
                    //        {
                    //            instance = Activator.CreateInstance(type);
                    //        }
                    //        else
                    //        {
                    //            instance = value.Value;
                    //        }
                    //    }              
                    //}
                    tree = new Tree { Key = clone, Data = a.Value.Data };

                    var parentMatch = TreeExtensions.MatchDescendant(data, (d => d.Key.Equals(a.Value.Parent?.Key))) as Tree;
                    parentMatch?.Add(tree);
                }
                if (a.Type == Changes.Type.Remove)
                {
                    var match = TreeExtensions.MatchDescendant(data, (d => d.Key.Equals(a.Value.Key))) as Tree;
                    match?.Parent?.Remove(a.Value);
                }
            }).DisposeWith(disposable);
            return disposable;
        }

        public static TreeViewer ModelTreeViewer(object model)
        {
            return new TreeViewer
            {
                ViewModel = model,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Default.ItemsPanelConverter.Instance,
                DataTemplateSelector = Default.DataTemplateSelector.Instance,
                TreeViewFilter = Model.Filter.Instance,
                StyleSelector = Model.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

        public static TreeViewer ViewModelTreeViewer(object viewModel)
        {
            return new TreeViewer
            {
                ViewModel = viewModel,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Default.ItemsPanelConverter.Instance,
                DataTemplateSelector = Default.DataTemplateSelector.Instance,
                TreeViewFilter = Default.Filter.Instance,
                StyleSelector = ViewModel.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

        public static TreeViewer ViewTreeViewer(object view)
        {
            return new TreeViewer
            {
                ViewModel = view,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,

                PanelsConverter = View.ItemsPanelConverter.Instance,
                DataTemplateSelector = View.DataTemplateSelector.Instance,
                StyleSelector = View.StyleSelector.Instance,

                TreeViewFilter = Default.Filter.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

        public static TreeViewer DataTreeViewer(object data)
        {
            return new TreeViewer
            {
                ViewModel = data,
                TreeViewItemFactory = Data.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Data.ItemsPanelConverter.Instance,
                //DataTemplateSelector = Utility.WPF.Templates.CustomDataTemplateSelector.Instance,
                DataTemplateSelector = Data.DataTemplateSelector.Instance,
                TreeViewFilter = Model.Filter.Instance,
                StyleSelector = Data.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }
    }

}

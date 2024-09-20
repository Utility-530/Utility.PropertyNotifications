using System.Reactive.Linq;
using System;
using System.Windows;
using Utility.Trees.Demo.MVVM.MVVM;
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
using Utility.Interfaces.NonGeneric;
using Splat;
using Utility.Extensions;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Trees;
using System.Collections.Generic;
using Utility.Trees.Demo.MVVM.Infrastructure;

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
        ReflectionNode root;

        public void Initialise()
        {
            DescriptorFactory.CreateRoot(Locator.Current.GetService<Model>(), Guid.Parse("76bca65d-6496-4a45-84f9-87705e665599"), "model")
                .Subscribe(descriptor =>
            {
                root = new ReflectionNode(descriptor);
                UpdateView(root);
                InitialiseView(root);
            });
        }

        void InitialiseView(ReflectionNode model)
        {
            AutoCompleteConverter.Instance.Subscribe(a =>
            {
                //var _ = model;

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
                        Pipe.Instance.Queue(new RepoQueueItem(default, QueueItemType.SelectKeys, ParentGuid: Guid.Parse("72022097-e5f6-4767-a18a-50763514ca01")));
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
                if (a is RepoQueueItem { Guid: { } guid } qi)
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

        void UpdateView(object data)
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

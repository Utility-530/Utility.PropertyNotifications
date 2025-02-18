using System.Reactive.Linq;
using System;
using System.Windows;
using Views.Trees;
using Utility.Trees.Abstractions;
using Utility.Descriptors;
using Utility.Nodes.Reflections;
using Utility.Trees.WPF;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Linq;
using Utility.Nodes.Demo.MVVM.Views;
using Utility.WPF.Templates;
using Utility.Interfaces.NonGeneric;
using Splat;
using Utility.Extensions;
using Utility.PropertyNotifications;
using Utility.Pipes;
using Utility.Trees.Decisions;
using Utility.Helpers.NonGeneric;
using Utility.Trees.Demo.Connections;
using System.Collections.Generic;
using Utility.Trees.Demo.Filters;
using Utility.Entities.Comms;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions.Async;
using Utility.Nodes.WPF;

namespace Utility.Nodes.Demo.MVVM
{
    public partial class App
    {
        private ReflectionNode root;
        private TreeViewer treeViewer;

        public void Initialise()
        {
            //DescriptorFactory.CreateRoot(Locator.Current.GetService<Model>(), Guid.Parse("76bca65d-6496-4a45-84f9-87705e665599"), "model")
            //DescriptorFactory.CreateRoot(new Product(), Guid.Parse("910ee619-8fa2-41f2-baaf-883b092f70aa"), "product")
            DescriptorFactory.CreateRoot(new Models(), Guid.Parse("fc7208ca-6502-4dcc-acba-a0e5ca0ca52b"), "models")
            //DescriptorFactory.CreateRoot(Locator.Current.GetService<Table>(), Guid.Parse("98f29d9c-0528-4096-acef-73f089646e82"), "table_model")
                .Subscribe(descriptor =>
            {
                root = new ReflectionNode(descriptor);
                Locator.CurrentMutable.RegisterConstant(root);
                UpdateView(root);
                UpdateConnectionsView();
                InitialiseView(root);
            });
        }

        void InitialiseView(ReflectionNode model)
        {
            AutoCompleteConverter.Instance.Subscribe(a =>
            {
                if (a is SuggestionPrompt { Value: IDescriptor { ParentType: { } type } value, Filter: { } filter })
                {
                }
            });

            EventListener.Instance.Subscribe(a =>
            {
                if (a is ClickChange { Node: IReadOnlyTree { Data: MemberDescriptor data } tree, Source: { } source })
                {
                    if (data.Name == "Table" && data is ICollectionItemReferenceDescriptor { Instance: Table table })
                        //{
                        //    var x = (TreeExtensions.MatchDescendant(root, a => (a.Data as IDescriptor).Name == "SelectedTable").Data as IRaisePropertyChanged);
                        //    x.RaisePropertyChanged(table);
                        //}
                        if (Locator.Current.GetService<Model>().SelectedTable != table)
                        {
                            //Locator.Current.GetService<Model>().SelectedTable = table;   
                            root.Descendant( a => a.tree.Data is IReferenceDescriptor { Name: "SelectedTable" })
                            .Subscribe(x =>
                            {
                                var yt = (x.NewItem.Data as ISetValue);
                                yt.Value = table;

                                Locator.Current.GetService<ITreeRepository>()
                                    .Find((x.NewItem as IGuid).Guid, "SelectedTable", type: typeof(Table))
                                    .Subscribe(parentGuid =>
                                    {
                                        //var rqi = new RepoItem(default, RepoItemType.SelectKeys, "SelectedTable", ParentGuid: parentGuid);
                                        //Pipe.Instance.New(new ForwardItem(Locator.Current.GetService<PipeRepository>().Predicate, rqi, new()));
                                    });
                            });
                        }
                    MainView.Instance.propertygrid.Content = new PropertyGrid { SelectedObject = data };
                    //Filter.Instance.Convert(tree);
                    DataTemplateSelector.Instance.SelectTemplate(tree, null);
                    StyleSelector.Instance.SelectStyle(source, null);
                }
            });

            ReflectionNode? last = null;

            Pipe.Instance.WithChangesTo(a => a.Next).Skip(1).Subscribe(a =>
            {
                if (a is DecisionQueueItem { Value: RepoItem { Guid: { } guid } qi })
                {
                    var d = model.Descendant(a => (a.tree.Data as MemberDescriptor).Guid == guid);
                    if (last is ReflectionNode node)
                    {
                        node.IsSelected = false;
                        node.RaisePropertyChanged(nameof(ReflectionNode.IsSelected));

                    }
                    if (d is ReflectionNode tree)
                    {
                        last = tree;
                        tree.IsSelected = true;
                        tree.RaisePropertyChanged(nameof(ReflectionNode.IsSelected));
                    }
                }
            });

            Pipe.Instance.WhenReceivedFrom(a => a.SelectedCompletedItem).Skip(1).Subscribe(a =>
            {
                if (a is DecisionQueueItem { Value: RepoItem { Guid: { } guid } qi })
                {
                    var d = model.Descendant(a => (a.tree.Data as MemberDescriptor).Guid == guid);
                    if (d is ReflectionNode tree)
                    {
                        last = tree;
                        tree.IsSelected = true;
                        tree.RaisePropertyChanged(nameof(ReflectionNode.IsSelected));
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
            treeViewer = DataTreeViewer(data);
            MainView.Instance.scrollviewer.Content = treeViewer;
            MainView.Instance.pipe_view.Content = Locator.Current.GetService<PipeController>();
            MainView.Instance.queue_view.Content = Pipe.Instance;
            MainView.Instance.filtertree.Content = treeViewer;
            MainView.Instance.filtertree.ContentTemplate = this.Resources["TVF"] as DataTemplate;
            MainView.Instance.datatemplatetree.Content = treeViewer;
            MainView.Instance.datatemplatetree.ContentTemplate = this.Resources["DTS"] as DataTemplate;
            MainView.Instance.styletree.Content = treeViewer;
            MainView.Instance.styletree.ContentTemplate = this.Resources["SS"] as DataTemplate;
            MainView.Instance.pipe_repository_tree.Content = Locator.Current.GetService<PipeRepository>();
            MainView.Instance.pipe_repository_tree.ContentTemplate = this.Resources["Pipe_Repository_Template"] as DataTemplate;
            MainView.Instance.repo_view.Content = Locator.Current.GetService<PipeRepository>();
        }

        void UpdateConnectionsView()
        {
            //var tree = new ViewModelTree(new ViewModel() { Name = "root" },
            //         new ViewModelTree(new ViewModel { Name = "A" },
            //         new ViewModelTree(new ViewModel { Name = "B" }),
            //         new ViewModelTree(new ViewModel { Name = "C" }),
            //         new ViewModelTree(new ViewModel { Name = "D" })));

            //TreeView.ItemsSource = tree.Items;

            List<Service> ms = typeof(Methods).ToServices().ToList();

            var _viewModel = new ConnectionsViewModel()
            {
                ServiceModel = ms,
                Tree = root
            };

            var x = new ConnectionsService()
            {
                TreeView = treeViewer,
                TreeView2 = MainView.Instance.TreeView2,
                viewModel = _viewModel,
                Container = MainView.Instance.maingrid
            };
            x.Loaded();

            //MainView.Instance.TreeView2.ItemsSource = _viewModel.ServiceModel;
            //MainView.Instance.Lines.ItemsSource = _viewModel.Lines;

            (root.Data as IValueChanges).Subscribe(t =>
            {
                foreach (var connection in _viewModel.Connections.Where(a => a.ViewModelName == t.Name && a.Movement == Movement.FromViewModelToService))
                {
                    if (connection == null)
                        return;
                    var serviceName = connection.ServiceName;
                    var service = _viewModel.ServiceModel.Single(a => a.Name == serviceName);
                    service.OnNext(t);
                }
            });


            foreach (var service in ms)
            {
                service.Subscribe(a =>
                {
                    foreach (var connection in _viewModel.Connections.Where(a => a.ServiceName == service.Name && a.Movement == Movement.ToViewModelFromService))
                    {
                        var viewModelName = connection.ViewModelName;
                        var viewModel = root.Descendant(a => (a.tree.Data as ViewModel).Name == viewModelName)
                        .Subscribe(vm => {
                            Pipe.Instance.Queue(new TreeQueueItem(vm.NewItem, a));
                        });
                    }
                });
            }

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
                TreeViewFilter = TreeViewFilter.Instance,
                StyleSelector = StyleSelector.Instance,
                EventListener = EventListener.Instance
            };
        }
    }

}

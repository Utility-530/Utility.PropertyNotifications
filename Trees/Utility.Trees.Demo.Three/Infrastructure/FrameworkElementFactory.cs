using System.Reactive.Linq;
using System;
using System.Windows;
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
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.Pipes;
using Utility.Trees.Decisions;
using Utility.Repos;
using Utility.Helpers.NonGeneric;

namespace Utility.Trees.Demo.MVVM
{

    public partial class App
    {
        ReflectionNode root;

        public void Initialise()
        {
            //DescriptorFactory.CreateRoot(Locator.Current.GetService<Model>(), Guid.Parse("76bca65d-6496-4a45-84f9-87705e665599"), "model")
            DescriptorFactory.CreateRoot(new Product(), Guid.Parse("910ee619-8fa2-41f2-baaf-883b092f70aa"), "product")
            //DescriptorFactory.CreateRoot(Locator.Current.GetService<Table>(), Guid.Parse("98f29d9c-0528-4096-acef-73f089646e82"), "table_model")
                .Subscribe(descriptor =>
            {
                root = new ReflectionNode(descriptor);
                Locator.CurrentMutable.RegisterConstant(root);
                UpdateView(root);
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
                            var x = TreeExtensions.MatchDescendant(root, a => a.Data is IReferenceDescriptor { Name: "SelectedTable" });
                            var yt = (x.Data as ISetValue);
                            yt.Value = table;

                            Locator.Current.GetService<ITreeRepository>()
                                .Find((x as IGuid).Guid, "SelectedTable", typeof(Table))
                                .Subscribe(parentGuid =>
                                {
                                    //var rqi = new RepoItem(default, RepoItemType.SelectKeys, "SelectedTable", ParentGuid: parentGuid);
                                    //Pipe.Instance.New(new ForwardItem(Locator.Current.GetService<PipeRepository>().Predicate, rqi, new()));
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

            Pipe.Instance.WhenReceivedFrom(a => a.SelectedCompletedItem).Skip(1).Subscribe(a =>
            {
                if (a is DecisionQueueItem { Value: RepoItem { Guid: { } guid } qi })
                {
                    var d = TreeExtensions.MatchDescendant(model, a => (a.Data as MemberDescriptor).Guid == guid);
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

using Swordfish.NET.Collections.Auxiliary;
using Utility.Infrastructure;
using Utility.Nodes;
using Utility.PropertyTrees.WPF.Demo.Views;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Observables.Generic;
using static Utility.PropertyTrees.Events;
using Utility.WPF.Adorners.Infrastructure;
using Utility.WPF.Reactive;
using NetFabric.Hyperlinq;
using System.Collections.Specialized;
using Utility.Helpers;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewController : BaseObject
    {
        private TreeView treeView;
        private RootProperty node;
        private Grid grid;
        private TreeView dataGrid = new();
        private GetViewModelResponse response;
        private ValueNode valueNode;

        public override Key Key => new(Guids.ViewController, nameof(ViewController), typeof(ViewController));

        public void OnNext(StartEvent startEvent)
        {
            Context.Post((_) =>
            {
                var content = CreateContent(node = startEvent.Property);
                var window = new Window { Content = content };
                window.Show();
            }, default);
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
        

            object CreateContent(ValueNode valueNode)
            {
                treeView = new();
                CreateSelected(valueNode);

                grid = new Grid();
                grid.RowDefinitions.AddRange(new[]
                {
                    new RowDefinition { Height = new GridLength(30, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                });

                var subGrid = new Grid() { };
                var commandView = new CommandView();
                Grid.SetRow(subGrid, 1);
                Grid.SetRow(dataGrid, 2);
                grid.Children.Add(commandView);
                grid.Children.Add(subGrid);
                grid.Children.Add(dataGrid);
                subGrid.Children.Add(treeView);

                var adorner = new Button
                {
                    Content = "click",
                    CommandParameter = new TreeClickEvent(node),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                AdornerHelper.AddIfMissingAdorner(dataGrid, adorner);
                dataGrid.SetValue(AdornerEx.IsEnabledProperty, true);
                adorner.Click += Adorner_Click;

                return grid;
            }
        }

        private void CreateSelected(ValueNode valueNode)
        {
            this.valueNode = valueNode;
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                .Subscribe(a =>
                {
                    //modelViewModel.IsConnected = true;
                    //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });


            var adorner = new Button
            {
                Content = "click",
                CommandParameter = new TreeClickEvent(node),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            treeView
                .MouseDoubleClickTreeSelections()
                .Subscribe(a =>
                {

                    if (a is { Header: ReferenceProperty { IsCollection: true } refNode })
                    {
                        if (refNode.Data is IList collection)
                        {
                            var type = collection.GetType().GenericTypeArguments().SingleOrDefault();
                            var instance = Activator.CreateInstance(type);
                            collection.Add(instance);
                            if (collection is not INotifyCollectionChanged collectionChanged)
                            {
                                //refNode.RefreshAsync();
                            }                            
                        }

                    }
                    if (a is { Header: ValueNode valueNode } treeviewItem)
                    {
                        this.valueNode = valueNode;
                        treeviewItem.IsExpanded = !treeviewItem.IsExpanded;
                        this.Observe<GetViewModelResponse, GetViewModelRequest>(new(valueNode.Key))
                            .Subscribe(response =>
                            {
                                Context.Post((_) =>
                                {
                                    this.response = response;
                                    dataGrid.Visibility = Visibility.Visible;
                                    //dataGrid.ItemsSource = response.ViewModels;
                                    var root = new RootProperty(Guid.NewGuid()) { Data =new { ViewModels = response.ViewModels } };
                                    Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(dataGrid, root))
                                    .Subscribe(a =>
                                    {
                                        //modelViewModel.IsConnected = true;
                                        //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                                    });
                                }, default);
                                //dataGrid.ItemsSource = response.ViewModels;
                            });
                    }          
                });

            //    Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
            //.Subscribe(a =>
            //{
            //    //modelViewModel.IsConnected = true;
            //    //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            //});
        }

        private void CreateSelected2()
        {
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, new RootViewModelsProperty()))
                .Subscribe(a =>
                {
                });
        }

        private void Adorner_Click(object sender, RoutedEventArgs e)
        {
            this.Observe<SetViewModelResponse, SetViewModelRequest>(new(valueNode.Key, response.ViewModels.Single()))
                .Subscribe(response =>
                {
                });
        }

        public void OnNext(RefreshRequest request)
        {
            treeView.Items.Clear();
            //Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, node))
            //    .Subscribe(a =>
            //    {
            //        //modelViewModel.IsConnected = true;
            //        //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            //    });
        }

        public void OnNext(TreeMouseDoubleClickEvent command)
        {

        }
    }
}
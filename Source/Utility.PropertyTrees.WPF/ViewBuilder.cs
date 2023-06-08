using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Panels;
using System;
using System.Collections.Generic;
using Utility.PropertyTrees.Infrastructure;
using Utility.Infrastructure;
using Utility.Models;
using System.Reactive.Linq;
using System.ComponentModel;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Nodes;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.Generic;
using System.Reactive.Disposables;
using Utility.PropertyTrees.Services;
using static Utility.PropertyTrees.Events;
using Grid = System.Windows.Controls.Grid;
using Dock = System.Windows.Controls.Dock;
using Utility.Helpers;
using Utility.Enums;
using Utility.WPF.Helpers;
using Swordfish.NET.Collections.Auxiliary;
using Utility.WPF.Adorners.Infrastructure;
using System.Windows.Media;

namespace Utility.PropertyTrees.WPF
{
    public class ViewBuilder : BaseObject
    {
        readonly Dictionary<PanelKey, ItemsPanelTemplate> panelsDictionary = new() { { new(0, System.Windows.Controls.Orientation.Vertical, Arrangement.Stacked), defaultTemplate } };
        static readonly ItemsPanelTemplate defaultTemplate = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory => factory.SetValue(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray) { Opacity = 0.1 }));

        public override Key Key => new(Guids.ViewBuilder, nameof(ViewBuilder), typeof(ViewBuilder));

        public Utility.Interfaces.Generic.IObservable<TreeViewResponse> OnNext(TreeViewRequest request)
        {
            return Create<TreeViewResponse>(observer =>
            {
                CompositeDisposable disposables = new();
                var dis = BuildTree(request.TreeView, request.PropertyNode, out var disposable)
                .Subscribe(a =>
                {
                    observer.OnProgress(a.Item1, a.Item2);
                    observer.OnNext(new TreeViewResponse(request.TreeView));
                },
                e =>
                {

                },
                () =>
                {

                }).DisposeWith(disposables);
                disposables.Add(disposable);
                return disposables;
            });
        }

        public IObservable<(int, int)> BuildTree(TreeView treeView, ValueNode property, out IDisposable disposable)
        {
            return PropertyExplorer.ExploreTree(treeView.Items, (items, prop) =>
            {
                var treeViewItem = MakeTreeViewItem(prop);
                items.Add(treeViewItem);
                return treeViewItem.Items;
            }, property, out disposable);

            TreeViewItem MakeTreeViewItem(PropertyBase node)
            {
                //try
                //{
                ItemsPanelTemplate? panelTemplate = DefaultItemsPanelTemplate();
                DataTemplate? headerTemplate = default;

                //ViewModel viewModel = new() { CollectionPanel = new() { Grid = new() }, Panel = new() { Grid = new() { } }, Template = new() { } };
                //node.ViewModel = viewModel;

                var treeViewItem = new TreeViewItem() { Header = node, IsExpanded = true };

                //_ = Observe<ActivationResponse, ActivationRequest>(new(node.Guid, new RootDescriptor(node), node, PropertyType.Root))
                //    .Select(a => a.PropertyNode)
                //.Subscribe(propertyNode =>
                //{
                //    propertyNode.Predicates = new ViewModelPredicate();
                //    PropertyExplorer.ExploreTree(new List<object>(), (a, b) => a, propertyNode, out var disposable)
                //    .Subscribe(a => { },
                //    () =>
                //    {
                //        var viewModel = node.ViewModel as ViewModel;
                //        panelTemplate = viewModel.Panel?.Type != null ? (ItemsPanelTemplate)Application.Current.TryFindResource(viewModel.Panel.Type) : DefaultItemsPanelTemplate();
                //        if (viewModel.Template.DataTemplateKey != null)
                //            headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.Template.DataTemplateKey);
                //        else
                //        {
                //            var key = new DataTemplateKey(node.PropertyType);
                //            headerTemplate = (DataTemplate)Application.Current.TryFindResource(key);
                //        }
                //        //treeViewItem.HeaderTemplate = headerTemplate;
                //        treeViewItem.ItemsPanel = panelTemplate;
                //    });
                //});

                _ = Observe<GetViewModelResponse, GetViewModelRequest>(new(node.Key))
                    .Subscribe(x =>
                    {
                        var viewModel = x.ViewModel;
                        try
                        {
                            panelTemplate = GetPanelsTemplate(panelTemplate, viewModel);
                            treeViewItem.ItemsPanel = panelTemplate;
                            treeViewItem.IsExpanded = viewModel.IsExpanded;
                            Grid.SetRow(treeViewItem, viewModel.GridRow);
                            Grid.SetColumn(treeViewItem, viewModel.GridColumn);
                            Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan);
                            Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan);
                            DockPanel.SetDock(treeViewItem, (Dock)viewModel.Dock);
                            treeViewItem.Margin = new Thickness(viewModel.Left, viewModel.Top, viewModel.Right, viewModel.Bottom);
                            //treeViewItem.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.2 };
                            if (string.IsNullOrEmpty(viewModel.DataTemplateKey) == false)
                            {
                                node.DataTemplateKey = viewModel.DataTemplateKey;
                                //var headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.DataTemplateKey);
                                //treeViewItem.HeaderTemplate = headerTemplate;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    });


                var adorner = new Button
                {
                    Content = "click",
                    Command = node.Command,
                    CommandParameter = new TreeClickEvent(node),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Utility.WPF.Adorners.Infrastructure.AdornerHelper.AddIfMissingAdorner(treeViewItem, adorner);

                treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);

                return treeViewItem;
                //}
                //catch (Exception ex)
                //{
                //    return new TreeViewItem();
                //}
            }
        }

        private ItemsPanelTemplate GetPanelsTemplate(ItemsPanelTemplate panelTemplate, IViewModel viewModel)
        {
            if (viewModel.Arrangement != null)
                if (System.Enum.TryParse(typeof(Arrangement), viewModel.Arrangement, out var result))
                {
                    panelTemplate = panelsDictionary.GetValueOrCreate(new PanelKey(0, System.Windows.Controls.Orientation.Vertical, (Arrangement)result), () => LayOutHelper.ItemsPanelTemplate(0, System.Windows.Controls.Orientation.Vertical, (Arrangement)result));
                }
                else if (Application.Current.TryFindResource(viewModel.Arrangement.ToString()) is ItemsPanelTemplate template)
                {
                    panelTemplate = template;
                }
                else
                {
                    panelTemplate = defaultTemplate;
                }
            else
            {
                panelTemplate = defaultTemplate;
            }

            return panelTemplate;
        }

        record PanelKey(int Index, System.Windows.Controls.Orientation Orientation, Arrangement Arrangement);

        static ItemsPanelTemplate DefaultItemsPanelTemplate()
        {
            FrameworkElementFactory factoryPanel = new(typeof(UniformStackPanel));
            factoryPanel.SetValue(System.Windows.Controls.Panel.IsItemsHostProperty, true);
            ItemsPanelTemplate template = new()
            {
                VisualTree = factoryPanel
            };
            return template;
        }
    }



    public class ViewModelPredicate : Filters
    {
        private List<Predicate<PropertyDescriptor>> predicates;

        public ViewModelPredicate()
        {
            predicates = new(){
                new Predicate<PropertyDescriptor>(descriptor=>
            {
                   return descriptor.PropertyType==typeof(IViewModel);
            }) };
        }

        public override IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator()
        {
            return predicates.GetEnumerator();
        }
    }

}

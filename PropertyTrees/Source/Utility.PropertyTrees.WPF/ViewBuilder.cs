using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Panels;
using System;
using System.Collections.Generic;
using Utility.Infrastructure;
using Utility.Models;
using System.Reactive.Linq;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Nodes;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.Generic;
using System.Reactive.Disposables;
using Utility.PropertyTrees.Services;
using Utility.Helpers;
using Utility.Enums;
using Utility.WPF.Helpers;
using Swordfish.NET.Collections.Auxiliary;
using Utility.WPF.Adorners.Infrastructure;
using System.Windows.Media;
using Orientation = System.Windows.Controls.Orientation;

namespace Utility.PropertyTrees.WPF
{
    public class HorizontalPanel : StackPanel
    {
        public HorizontalPanel()
        {
            Orientation = Orientation.Horizontal;
        }
    }

    public class ViewBuilder : BaseObject
    {
        int count;
        private TreeViewItem columnsTreeViewItem;
        private Dictionary<object, int> dictionary = new();
        private Style flatstyle;
        private Dictionary<int, object> cache = new();
        private Style horizontalStyle;
        private readonly DataTemplateSelector dataTemplateSelector;

        readonly Dictionary<PanelKey, ItemsPanelTemplate> panelsDictionary = new() {
            { new(0, Orientation.Vertical, Arrangement.Stacked), defaultTemplate } ,
            { new(1, Orientation.Horizontal, Arrangement.Stacked), defaultTemplate }
        };

        static readonly DataTemplate emptyTemplate = TemplateGenerator.CreateDataTemplate(() =>
            {
                return new Control();
            });

        static readonly ItemsPanelTemplate defaultTemplate = TemplateGenerator.CreateItemsPanelTemplate<StackPanel>(factory =>
        factory.SetValue(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray) { Opacity = 0.1 }));
        static readonly ItemsPanelTemplate uniformStackTemplate = TemplateGenerator.CreateItemsPanelTemplate<UniformStackPanel>(factory =>
        factory.SetValue(Panel.IsItemsHostProperty, true));
        static readonly ItemsPanelTemplate horizontalTemplate = TemplateGenerator.CreateItemsPanelTemplate<HorizontalPanel>(factory =>
        {
            factory.SetValue(Control.BackgroundProperty, new SolidColorBrush(Colors.LightGray) { Opacity = 0.1 });
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
        });

        // Only shows the item presenter
        private Style FlatStyle
        {
            get
            {
                if (flatstyle != default)
                    return flatstyle;
                flatstyle = new Style { TargetType = typeof(TreeViewItem) };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                var factory = new FrameworkElementFactory(typeof(ItemsPresenter));
                factory.SetValue(Control.NameProperty, "ItemsHost");
                template.VisualTree = factory;
                var setter = new Setter { Property = Control.TemplateProperty, Value = template };
                flatstyle.Setters.Add(setter);
                return flatstyle;
            }
        }

        private Style HorizontalStyle
        {
            get
            {
                horizontalStyle = new Style(typeof(HeaderedContentControl));
                //horizontalStyle.Setters.Add(new Setter(FontSizeProperty, 16));
                horizontalStyle.Setters.Add(new Setter(Control.TemplateProperty,
                    new ControlTemplate(typeof(HeaderedContentControl))
                    {
                        VisualTree = BuildHeaderedContentControlFactory()
                    }));
                return horizontalStyle;
            }
        }

        static FrameworkElementFactory BuildHeaderedContentControlFactory()
        {
            FrameworkElementFactory gridFactory = new (typeof(CustomGrid));
            gridFactory.Name = "PART_StackPanel";
            FrameworkElementFactory headerPresenterFactory = new (typeof(ContentPresenter));
            headerPresenterFactory.Name = "PART_HeaderPresenter";
            headerPresenterFactory.SetValue(Grid.ColumnProperty, 0);
            headerPresenterFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            FrameworkElementFactory contentPresenterFactory = new (typeof(ContentPresenter));
            contentPresenterFactory.Name = "PART_ContentPresenter";
            contentPresenterFactory.SetValue(Grid.ColumnProperty, 1);
            gridFactory.AppendChild(headerPresenterFactory);
            gridFactory.AppendChild(contentPresenterFactory);
            return gridFactory;
        }

        private class CustomGrid : Grid
        {
            readonly ColumnDefinition 
                column1 = new() { Width = new GridLength(160) },
                column2 = new() { Width = new GridLength(2, GridUnitType.Star) },
                column3 = new() { Width = new GridLength(1, GridUnitType.Star) };

            public CustomGrid()
            {
                ColumnDefinitions.Add(column1);
                ColumnDefinitions.Add(column2);
            }
        }



        public override Key Key => new(Guids.ViewBuilder, nameof(ViewBuilder), typeof(ViewBuilder));

        public ViewBuilder(DataTemplateSelector dataTemplateSelector)
        {
            this.dataTemplateSelector = dataTemplateSelector;
        }


        public Utility.Interfaces.Generic.IObservable<TreeViewResponse> OnNext(TreeViewRequest request)
        {
       
            return Create<TreeViewResponse>(observer =>
            {
                    CompositeDisposable disposables = new();
                Context.Post(_ =>
                {

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
                },default);
                return disposables;
            });
        }



        public IObservable<(int, int)> BuildTree(TreeView treeView, ValueNode property, out IDisposable disposable)
        {
            return PropertyExplorer.ExploreTree(treeView.Items, (items, prop) =>
            {
                if (prop.IsCollection == true || prop.IsDescendantOfCollection())
                {
                    TreeViewItem treeViewItem;

                    // root
                    if (prop.IsCollection == true)
                    {
                        dictionary.Clear();
                        treeViewItem = new TreeViewItem() { IsExpanded = true, Header = prop.Name };
                        treeViewItem.Items.Add(columnsTreeViewItem = new() { IsExpanded = true, ItemsPanel = horizontalTemplate, Style = FlatStyle, });
                        items.Add(treeViewItem);
                        return treeViewItem.Items;
                    }
                    else if (prop is ValueProperty)
                    {
                        if (prop.Name == "Type")
                        {

                        }
                        if (dictionary.ContainsKey(prop.Name) == false)
                        {
                            dictionary.Add(prop.Name, dictionary.Count);
                            columnsTreeViewItem.Items.Add(new TreeViewItem { Header = new Label { Content = prop.Name, Tag = prop.Name }, Width = 200 });
                            //columnsTreeViewItem.Items.SortDescriptions.Clear();
                            //columnsTreeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));

                        }

                        treeViewItem = new TreeViewItem { Header = prop, HeaderTemplateSelector = dataTemplateSelector, Width = 200 };

                        while (items.Count < dictionary[prop.Name])
                            items.Add(new TreeViewItem() { Header = "Remove", Width = 200 });                       

                        while (items.Count > dictionary[prop.Name])
                            items.RemoveAt(dictionary[prop.Name]);

                        items.Insert(dictionary[prop.Name], treeViewItem);

                        return items;
                    }
                    else if (prop.IsChildOfCollection())
                    {
                        treeViewItem = new TreeViewItem
                        {
                            Header = prop.Name,
                            Style = FlatStyle,
                            ItemsPanel = horizontalTemplate,
                            IsExpanded = true
                        };
                        items.Add(treeViewItem);
                        return treeViewItem.Items;
                    }
                }
                else
                {
                    var treeViewItem = MakeTreeViewItem(prop);
                    items.Add(treeViewItem);
                    return treeViewItem.Items;
                }
                return items;

            }, property, out disposable);

            TreeViewItem MakeTreeViewItem(PropertyBase node)
            {
                TreeViewItem treeViewItem;
                if (node is ValueProperty)
                {
                    var header = new HeaderedContentControl
                    {
                        Header = node.Name,
                        Style = HorizontalStyle,
                        ContentTemplateSelector = dataTemplateSelector,
                        Content = node,
                        Tag = node.Name
                    };

                    treeViewItem = new TreeViewItem()
                    {
                        Header = header, /*HeaderTemplate = emptyTemplate,*/
                        IsExpanded = true
                    };
                }
                else
                    treeViewItem = new TreeViewItem()
                    {
                        Header = node.Name, /*HeaderTemplate = emptyTemplate,*/
                        IsExpanded = true
                    };


                //_ = Observe<GetViewModelResponse, GetViewModelRequest>(new(node.Key))
                //    .Subscribe(x =>
                //    {
                //        foreach (var viewModel in x.ViewModels)
                //            try
                //            {
                //                panelTemplate = GetPanelsTemplate(panelTemplate, viewModel);
                //                treeViewItem.ItemsPanel = panelTemplate;
                //                if (viewModel.IsExpanded.HasValue)
                //                    treeViewItem.IsExpanded = viewModel.IsExpanded.Value;
                //                if (viewModel.GridRow.HasValue)
                //                    Grid.SetRow(treeViewItem, viewModel.GridRow.Value);
                //                if (viewModel.GridColumn.HasValue)
                //                    Grid.SetColumn(treeViewItem, viewModel.GridColumn.Value);
                //                if (viewModel.GridRowSpan.HasValue)
                //                    Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan.Value);
                //                if (viewModel.GridColumnSpan.HasValue)
                //                    Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan.Value);
                //                if (viewModel.Dock.HasValue)
                //                    DockPanel.SetDock(treeViewItem, (Dock)viewModel.Dock);

                //                treeViewItem.Margin = new Thickness(viewModel.Left ?? 0, viewModel.Top ?? 0, viewModel.Right ?? 0, viewModel.Bottom ?? 0);
                //                //treeViewItem.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.2 };
                //                if (string.IsNullOrEmpty(viewModel.DataTemplateKey) == false)
                //                {
                //                    node.DataTemplateKey = viewModel.DataTemplateKey;
                //                    //var headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.DataTemplateKey);
                //                    //treeViewItem.HeaderTemplate = headerTemplate;
                //                }
                //            }
                //            catch (Exception ex)
                //            {

                //            }
                //    });



                var adorner = new Button
                {
                    Content = "click",
                    Command = node.Command,
                    CommandParameter = new TreeClickEvent(node),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                // Utility.WPF.Adorners.Infrastructure.AdornerHelper.AddIfMissingAdorner(treeViewItem, adorner);

                treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);


                treeViewItem.MouseDoubleClick += (s, e) =>
                {
                    node.Command.Execute(new TreeMouseDoubleClickEvent(node));
                };

                return treeViewItem;
            }
        }

        private ItemsPanelTemplate GetPanelsTemplate(ItemsPanelTemplate panelTemplate, IViewModel viewModel)
        {
            if (viewModel.Arrangement != null)
                if (Enum.TryParse(typeof(Arrangement), viewModel.Arrangement, out var result))
                {
                    panelTemplate = panelsDictionary.GetValueOrCreate(new PanelKey(0, Orientation.Vertical, (Arrangement)result), () => LayOutHelper.ItemsPanelTemplate(0, Orientation.Vertical, (Arrangement)result));
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

        record PanelKey(int Index, Orientation Orientation, Arrangement Arrangement);

    }
}

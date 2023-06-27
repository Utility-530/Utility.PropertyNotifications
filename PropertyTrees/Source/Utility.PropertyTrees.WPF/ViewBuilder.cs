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
using Utility.WPF.Adorners.Infrastructure;
using System.Windows.Media;
using Orientation = System.Windows.Controls.Orientation;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Utility.Helpers.NonGeneric;
using NetFabric.Hyperlinq;
using System.Linq;

namespace Utility.PropertyTrees.WPF
{
    public class HorizontalPanel : UniformGrid
    {
        public HorizontalPanel()
        {
            Rows = 1;
        }
    }

    public class ViewBuilder : BaseObject
    {
        const int columnWidth = 90;
        private TreeViewItem columnsTreeViewItem, removeItem;
        private Dictionary<Type, Dictionary<object, int>> typeOrderDictionary = new();
        private Style itemsOnlyStyle, contentOnlyStyle;
        private Dictionary<int, object> cache = new();
        private static Style? horizontalStyle;
        private DataTemplate headeredContentTemplate;
        private readonly DataTemplateSelector dataTemplateSelector;

        readonly Dictionary<PanelKey, ItemsPanelTemplate> panelsDictionary = new() {
            { new(0, Orientation.Vertical, Arrangement.Stacked), defaultTemplate } ,
            { new(1, Orientation.Horizontal, Arrangement.Stacked), defaultTemplate }
        };

        static readonly DataTemplate emptyTemplate = TemplateGenerator.CreateDataTemplate(() =>
            {
                return new Control();
            });

        static DataTemplate headerTemplate(int count) => TemplateGenerator.CreateDataTemplate(() =>
        {
            Binding binding = new(nameof(ValueNode.Name))
            {
                Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter
            };
            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            textBlock.FontWeight = FontWeight.FromOpenTypeWeight(500 - count * 10);
            textBlock.FontSize = 32 - count * 1;
            return textBlock;
        });

        DataTemplate HeaderedContentTemplate
        {
            get
            {
                return headeredContentTemplate ??= TemplateGenerator.CreateDataTemplate(Create);

                HeaderedContentControl Create()
                {
                    var headeredContentControl = new HeaderedContentControl
                    {
                        Style = HorizontalStyle,
                        ContentTemplateSelector = dataTemplateSelector,
                        //Content = node,
                        //Tag = node.Name
                    };

                    Binding binding = new(nameof(ValueNode.Name))
                    {
                        Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter
                    };
                    headeredContentControl.SetBinding(HeaderedContentControl.HeaderProperty, binding);

                    Binding binding2 = new();
                    headeredContentControl.SetBinding(HeaderedContentControl.ContentProperty, binding2);

                    return headeredContentControl;
                }
            }
        }

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
        private Style ItemsOnlyStyle
        {
            get
            {
                if (itemsOnlyStyle != default)
                    return itemsOnlyStyle;
                itemsOnlyStyle = new Style { TargetType = typeof(TreeViewItem) };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                var factory = new FrameworkElementFactory(typeof(ItemsPresenter));
                factory.SetValue(Control.NameProperty, "ItemsHost");
                template.VisualTree = factory;
                var setter = new Setter { Property = Control.TemplateProperty, Value = template };
                itemsOnlyStyle.Setters.Add(setter);
                return itemsOnlyStyle;
            }
        }

        private Style ContentOnlyStyle
        {
            get
            {
                if (contentOnlyStyle != default)
                    return contentOnlyStyle;
                contentOnlyStyle = new Style { TargetType = typeof(TreeViewItem) };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                var factory = new FrameworkElementFactory(typeof(ContentPresenter));
                factory.SetValue(Control.NameProperty, "PART_Header");
                factory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
                factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                template.VisualTree = factory;
                var setter = new Setter { Property = Control.TemplateProperty, Value = template };
                contentOnlyStyle.Setters.Add(setter);
                return contentOnlyStyle;
            }
        }

        private static Style HorizontalStyle
        {
            get
            {
                horizontalStyle = new Style(typeof(HeaderedContentControl));
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
            FrameworkElementFactory gridFactory = new(typeof(CustomGrid));
            gridFactory.Name = "PART_StackPanel";
            FrameworkElementFactory headerPresenterFactory = new(typeof(ContentPresenter));
            headerPresenterFactory.Name = "PART_HeaderPresenter";
            headerPresenterFactory.SetValue(Grid.ColumnProperty, 0);
            headerPresenterFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            FrameworkElementFactory contentPresenterFactory = new(typeof(ContentPresenter));
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
                column2 = new() { Width = new GridLength(2, GridUnitType.Star) 
                //column3 = new() { Width = new GridLength(1, GridUnitType.Star)
                };

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
                    e => { },
                    () => { })
                    .DisposeWith(disposables);
                    disposables.Add(disposable);
                }, default);
                return disposables;
            });
        }

        public IObservable<(int, int)> BuildTree(TreeView treeView, ValueNode property, out IDisposable disposable)
        {
            typeOrderDictionary.Clear();
            return PropertyExplorer.ExploreTree(treeView.Items, (items, prop) =>
            {
                if (prop is PropertyBase propertyBase)
                {
                    if (propertyBase.IsCollection == true || propertyBase.IsDescendantOfCollection())
                    {
                        TreeViewItem treeViewItem = null;

                        // root
                        if (propertyBase.IsCollection == true)
                        {
                            treeViewItem = new TreeViewItem()
                            {
                                IsExpanded = true,
                                Header = prop,
                                HeaderTemplate = headerTemplate(prop.Ancestors.Count()),
                            };

                            treeViewItem.Items.Add(columnsTreeViewItem = new() { IsExpanded = true, ItemsPanel = horizontalTemplate, Style = ItemsOnlyStyle, });
                            items.Add(treeViewItem);
                            Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                            return treeViewItem.Items;
                        }
                        else if (propertyBase is ValueProperty valueProperty)
                        {
                            var type = (valueProperty.FindAncestor(new Predicate<Nodes.Abstractions.INode>(a => a is ICollectionItemProperty)) as ICollectionItemProperty).Type;
                            var dictionary = typeOrderDictionary.GetValueOrNew(type);

                            //if (items.Count == 0)
                            //{
                            //    dictionary["remove"] = 0;
                            //    columnsTreeViewItem.Items.Add(new TreeViewItem { Width = columnWidth, Style = ContentOnlyStyle });
                            //}

                            if (dictionary.ContainsKey(propertyBase.Name) == false)
                            {
                                dictionary.Add(propertyBase.Name, dictionary.Count);
                                columnsTreeViewItem.Items.Add(new TreeViewItem { Header = new Label { Content = propertyBase.Name, Tag = propertyBase.Name }, Width = columnWidth, Style = ContentOnlyStyle });
                                //columnsTreeViewItem.Items.SortDescriptions.Clear();
                                //columnsTreeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));

                            }

                            treeViewItem = new TreeViewItem { Header = prop, HeaderTemplateSelector = dataTemplateSelector, Width = columnWidth, Style = ContentOnlyStyle };

                            List<object> orphans = new();

                            if (items.Count > dictionary[propertyBase.Name])
                            {
                                orphans.Add(items[(dictionary[propertyBase.Name])]);
                                items.RemoveAt(dictionary[propertyBase.Name]);
                            }

                            items.Add(treeViewItem);

                            foreach (var orphan in orphans)
                            {
                                items.Add(orphan);
                            }
                            Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                            return items;
                        }
                        else if (propertyBase.IsChildOfCollection())
                        {
                            treeViewItem = new TreeViewItem
                            {
                                Header = prop,
                                HeaderTemplate = headerTemplate(prop.Ancestors.Count()),
                                Style = ItemsOnlyStyle,
                                ItemsPanel = horizontalTemplate,
                                IsExpanded = true
                            };

                            items.Add(treeViewItem);
                            Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                            return treeViewItem.Items;
                        }

                    }
                    else
                    {
                        var treeViewItem = MakeTreeViewItem(propertyBase);
                        Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                        items.Add(treeViewItem);
                        return treeViewItem.Items;
                    }
                }
                return items;

            },
            (items, node) =>
            {

                var item = items.Cast<TreeViewItem>().Where(a => a.Header == node).SingleOrDefault();
                if (item != default)
                    items.Remove(item);
            },
            property, out disposable);

            TreeViewItem MakeTreeViewItem(PropertyBase node)
            {
                TreeViewItem treeViewItem;
                if (node is ValueProperty)
                {
                    treeViewItem = new TreeViewItem()
                    {
                        Header = node,
                        HeaderTemplate = HeaderedContentTemplate,
                        IsExpanded = true
                    };
                }
                else
                    treeViewItem = new TreeViewItem()
                    {
                        Header = node,
                        HeaderTemplate = headerTemplate(node.Ancestors.Count()),
                        IsExpanded = true
                    };

                _ = Observe<GetViewModelResponse, GetViewModelRequest>(new(node.Key))
                    .Subscribe(async x =>
                    {
                        Context.Post(_ =>
                        {
                            foreach (var viewModel in x.ViewModels)
                                try
                                {
                                    treeViewItem.ItemsPanel = GetPanelsTemplate(treeViewItem.ItemsPanel, viewModel);
                                    //if (viewModel.IsExpanded)
                                    //treeViewItem.IsExpanded = viewModel.IsExpanded.Value;
                                    treeViewItem.IsExpanded = true; // viewModel.IsExpanded;
                                    //if (viewModel.GridRow)
                                    //Grid.SetRow(treeViewItem, viewModel.GridRow.Value);
                                    Grid.SetRow(treeViewItem, viewModel.GridRow);
                                    //if (viewModel.GridColumn.HasValue)
                                    //Grid.SetColumn(treeViewItem, viewModel.GridColumn.Value);
                                    Grid.SetColumn(treeViewItem, viewModel.GridColumn);
                                    //if (viewModel.GridRowSpan.HasValue)
                                    //Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan.Value);
                                    Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan);
                                    //if (viewModel.GridColumnSpan.HasValue)
                                    //Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan.Value);
                                    Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan);
                                    //if (viewModel.Dock.HasValue)
                                    DockPanel.SetDock(treeViewItem, (Dock)viewModel.Dock);
                                    treeViewItem.ToolTip = viewModel.Tooltip;
                                    //treeViewItem.Margin = new Thickness(viewModel.Left ?? 0, viewModel.Top ?? 0, viewModel.Right ?? 0, viewModel.Bottom ?? 0);
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
                        }, default);
                    });

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


            TreeViewItem MakeTreeViewMethodItem(MethodNode node)
            {
                TreeViewItem treeViewItem;

                treeViewItem = new TreeViewItem()
                {
                    Header = node,
                    HeaderTemplateSelector = dataTemplateSelector,
                    //HeaderTemplate = ContentTempl,
                    IsExpanded = true
                };

           

                //else
                //    treeViewItem = new TreeViewItem()
                //    {
                //        Header = node,
                //        HeaderTemplate = headerTemplate,
                //        IsExpanded = true
                //    };


                //_ = Observe<GetViewModelResponse, GetViewModelRequest>(new(node.Key))
                //    .Subscribe(async x =>
                //    {
                //        Context.Post(_ =>
                //        {
                //            foreach (var viewModel in x.ViewModels)
                //                try
                //                {
                //                    treeViewItem.ItemsPanel = GetPanelsTemplate(treeViewItem.ItemsPanel, viewModel);
                //                    //if (viewModel.IsExpanded)
                //                    //treeViewItem.IsExpanded = viewModel.IsExpanded.Value;
                //                    treeViewItem.IsExpanded = viewModel.IsExpanded;
                //                    //if (viewModel.GridRow)
                //                    //Grid.SetRow(treeViewItem, viewModel.GridRow.Value);
                //                    Grid.SetRow(treeViewItem, viewModel.GridRow);
                //                    //if (viewModel.GridColumn.HasValue)
                //                    //Grid.SetColumn(treeViewItem, viewModel.GridColumn.Value);
                //                    Grid.SetColumn(treeViewItem, viewModel.GridColumn);
                //                    //if (viewModel.GridRowSpan.HasValue)
                //                    //Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan.Value);
                //                    Grid.SetRowSpan(treeViewItem, viewModel.GridRowSpan);
                //                    //if (viewModel.GridColumnSpan.HasValue)
                //                    //Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan.Value);
                //                    Grid.SetColumnSpan(treeViewItem, viewModel.GridColumnSpan);
                //                    //if (viewModel.Dock.HasValue)
                //                    DockPanel.SetDock(treeViewItem, (Dock)viewModel.Dock);

                //                    //treeViewItem.Margin = new Thickness(viewModel.Left ?? 0, viewModel.Top ?? 0, viewModel.Right ?? 0, viewModel.Bottom ?? 0);
                //                    treeViewItem.Margin = new Thickness(viewModel.Left, viewModel.Top, viewModel.Right, viewModel.Bottom);

                //                    //treeViewItem.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.2 };
                //                    if (string.IsNullOrEmpty(viewModel.DataTemplateKey) == false)
                //                    {
                //                        node.DataTemplateKey = viewModel.DataTemplateKey;
                //                        //var headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.DataTemplateKey);
                //                        //treeViewItem.HeaderTemplate = headerTemplate;
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                }
                //        }, default);
                //    });

                //var adorner = new Button
                //{
                //    Content = "click",
                //    Command = node.Command,
                //    CommandParameter = new TreeClickEvent(node),
                //    HorizontalAlignment = HorizontalAlignment.Right,
                //    VerticalAlignment = VerticalAlignment.Top
                //};

                //// Utility.WPF.Adorners.Infrastructure.AdornerHelper.AddIfMissingAdorner(treeViewItem, adorner);

                //treeViewItem.SetValue(AdornerEx.IsEnabledProperty, true);


                //treeViewItem.MouseDoubleClick += (s, e) =>
                //{
                //    node.Command.Execute(new TreeMouseDoubleClickEvent(node));
                //};

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

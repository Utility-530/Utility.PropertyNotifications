using System.Windows.Controls;
using System.Windows;
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
using Orientation = System.Windows.Controls.Orientation;
using NetFabric.Hyperlinq;
using System.Linq;
using Utility.Nodes.Abstractions;

namespace Utility.PropertyTrees.WPF
{


    public partial class ViewBuilder : BaseObject
    {

        const int columnWidth = 70;
        private TreeViewItem columnsTreeViewItem, removeItem;
        private Dictionary<Type, Dictionary<object, int>> typeOrderDictionary = new();

        private Dictionary<int, object> cache = new();
        private readonly DataTemplateSelector dataTemplateSelector;
        private readonly StyleSelector styleSelector;
        private DataTemplate headeredContentTemplate;
        private static Style? horizontalStyle;
        Dictionary<PropertyBase, TreeViewItem> orphans = new();

        public override Key Key => new(Guids.ViewBuilder, nameof(ViewBuilder), typeof(ViewBuilder));

        public ViewBuilder(DataTemplateSelector dataTemplateSelector, StyleSelector styleSelector)
        {
            this.dataTemplateSelector = dataTemplateSelector;
            this.styleSelector = styleSelector;
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


                IObservable<(int, int)> BuildTree(TreeView treeView, ValueNode property, out IDisposable disposable)
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
                                    treeViewItem = Make(prop);
                                    treeViewItem.Items.Add(columnsTreeViewItem = new()
                                    { 
                                        IsExpanded = true,
                                        ItemsPanel = horizontalTemplate,
                                        Style = TreeStyleSelector.ItemsOnlyStyle
                                    });
                                    items.Add(treeViewItem);
                                    Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                                    return treeViewItem.Items;
                                }
                                else if (propertyBase is ValueProperty valueProperty)
                                {
                                    var type = (valueProperty.FindAncestor(new Predicate<INode>(a => a is ICollectionItemProperty)) as ICollectionItemProperty).Type;
                                    var dictionary = typeOrderDictionary.GetValueOrNew(type);

                                    if (dictionary.ContainsKey(propertyBase.Name) == false)
                                    {
                                        dictionary.Add(propertyBase.Name, dictionary.Count);
                                        columnsTreeViewItem.Items.Add(new TreeViewItem { Header =  propertyBase, HeaderTemplate = TreeTemplateSelector.HeaderTemplate(9), Tag = propertyBase.Name, Width = columnWidth, Style = TreeStyleSelector.HeaderOnlyStyle });
                                    }
                                    treeViewItem = Make(propertyBase);

                                    if (items.Count == dictionary[propertyBase.Name])
                                    {
                                        items.Add(treeViewItem);
                                    }
                                    else
                                    {
                                        orphans.Add(prop as PropertyBase, treeViewItem);
                                    }

                                    foreach (var orphan in orphans.OrderBy(a => a.Key.Name).ToArray())
                                    {
                                        if (items.Count == dictionary[orphan.Key.Name])
                                        {
                                            items.Add(orphan.Value);
                                            orphans.Remove(orphan.Key);
                                        }
                                    }

                                    Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                                    return items;
                                }
                                else if (propertyBase.IsChildOfCollection())
                                {
                                    treeViewItem = MakeChildOfCollection(prop);
                                    items.Add(treeViewItem);
                                    Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                                    return treeViewItem.Items;
                                }

                            }
                            else
                            {
                                var labelItem = new TreeViewItem { Header = propertyBase, HeaderTemplateSelector = dataTemplateSelector, Style = TreeStyleSelector.HeaderOnlyStyle };
                                var treeViewItem = MakeTreeViewItem(propertyBase);
                                Send(new TreeViewItemInitialised(treeViewItem, propertyBase));
                                //items.Add(labelItem);
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
                        else
                        {

                        }
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
                                IsExpanded = true,
                                //ItemContainerStyleSelector = styleSelector
                            };
                        }
                        else if (node is RootProperty)
                        {
                            treeViewItem = new TreeViewItem()
                            {
                                Header = node,
                                IsExpanded = true,
                                Style = TreeStyleSelector.ItemsOnlyStyle
                            };
                        }
                        else
                            treeViewItem = Make(node);               

                        SetProperties(node, treeViewItem);

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
                            ItemContainerStyleSelector = styleSelector,
                            IsExpanded = true
                        };

                        return treeViewItem;
                    }

                    TreeViewItem MakeChildOfCollection(INode prop)
                    {
                        TreeViewItem treeViewItem;

                        treeViewItem = new TreeViewItem
                        {
                            Header = prop,
                            //HeaderTemplate = headerTemplate(prop.Ancestors.Count()),
                            //Style = ItemsOnlyStyle,
                            ItemsPanel = horizontalTemplate,
                            IsExpanded = true,
                            ItemContainerStyleSelector = styleSelector
                        };
                        return treeViewItem;
                    }

                    TreeViewItem Make(INode prop)
                    {
                        return new TreeViewItem()
                        {
                            IsExpanded = true,
                            Header = prop,
                            HeaderTemplateSelector = dataTemplateSelector,
                            ItemContainerStyleSelector = styleSelector
                        };
                    }

                    void SetProperties(PropertyBase node, TreeViewItem treeViewItem)
                    {
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
                                            //treeViewItem.ToolTip = viewModel.Tooltip;
                                            //treeViewItem.Margin = new Thickness(viewModel.Left ?? 0, viewModel.Top ?? 0, viewModel.Right ?? 0, viewModel.Bottom ?? 0);
                                            //treeViewItem.Margin = new Thickness(viewModel.Left, viewModel.Top, viewModel.Right, viewModel.Bottom);

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

                        ItemsPanelTemplate GetPanelsTemplate(ItemsPanelTemplate panelTemplate, IViewModel viewModel)
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

                    }
                }
            });
        }


        record PanelKey(int Index, Orientation Orientation, Arrangement Arrangement);

    }
}

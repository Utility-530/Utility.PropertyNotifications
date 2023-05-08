using System.Windows.Controls;
using System.Windows;
using Utility.PropertyTrees.Abstractions;
using Utility.WPF.Panels;
using System;
using System.Collections.Generic;
using Utility.PropertyTrees.Infrastructure;
using Utility.Infrastructure;
using Utility.Models;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Utility.PropertyTrees.WPF
{
    public class ViewBuilder : BaseObject
    {

        Guid Guid = Guid.Parse("26bb7df3-9ae7-49d6-aabc-a492c6254718");

        public override Key Key => new(Guid, nameof(ViewBuilder), typeof(ViewBuilder));


        public override bool OnNext(object value)
        {
            if (value is not GuidValue { Guid: var guid, Value: TreeViewRequest { PropertyNode: var propertyNode, TreeView: var treeView } request })
            {
                return base.OnNext(value);
            }

            BuildTree(treeView, propertyNode)
                .Subscribe(a => { }, () =>
                {
                    Broadcast(new GuidValue(guid, new TreeViewResponse(treeView), 0));
                });
            return true;
        }

        public IObservable<double> BuildTree(TreeView treeView, PropertyNode property)
        {
            return PropertyHelper.ExploreTree(treeView.Items, (items, prop) =>
            {
                var treeViewItem = MakeTreeViewItem(prop);
                items.Add(treeViewItem);
                return items;
            }, property);

            TreeViewItem MakeTreeViewItem(PropertyBase node)
            {
                //try
                //{
                ItemsPanelTemplate? panelTemplate = default;
                DataTemplate? headerTemplate = default;

                //ViewModel viewModel = new() { CollectionPanel = new() { Grid = new() }, Panel = new() { Grid = new() { } }, Template = new() { } };
                //node.ViewModel = viewModel;

                var treeViewItem = new TreeViewItem() { Header = node/*, HeaderTemplate = headerTemplate*/, /*ItemsPanel = panelTemplate, */IsExpanded = true };

                _ = Observe<PropertyNode, ActivationRequest>(new(node.Guid, new RootDescriptor(node), node, PropertyType.Root))
                .Subscribe(propertyNode =>
                {
                    propertyNode.Predicates = new ViewModelPredicate();
                    PropertyHelper.ExploreTree(new List<object>(), (a, b) => a, propertyNode).Subscribe(a => { },
                    () =>
                    {
                        var viewModel = node.ViewModel as ViewModel;
                        panelTemplate = viewModel.Panel?.Type != null ? (ItemsPanelTemplate)Application.Current.TryFindResource(viewModel.Panel.Type) : DefaultItemsPanelTemplate();
                        if (viewModel.Template.DataTemplateKey != null)
                            headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.Template.DataTemplateKey);
                        else
                        {
                            var key = new DataTemplateKey(node.PropertyType);
                            headerTemplate = (DataTemplate)Application.Current.TryFindResource(key);
                        }
                        //treeViewItem.HeaderTemplate = headerTemplate;
                        treeViewItem.ItemsPanel = panelTemplate;
                    });
                });

                return treeViewItem;
                //}
                //catch (Exception ex)
                //{
                //    return new TreeViewItem();
                //}
            }
        }


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


    public record TreeViewRequest(TreeView TreeView, PropertyNode PropertyNode);

    public record TreeViewResponse(TreeView TreeView);
    //static IDisposable AddToTree(ItemCollection items, PropertyNode property, Subject<State> state)
    //{
    //    state.OnNext(State.Started);

    //    var disposable = property
    //        .Children
    //        .Subscribe(item =>
    //        {
    //            if (item is not NotifyCollectionChangedEventArgs args)
    //                throw new Exception("rev re");

    //            foreach (PropertyBase node in SelectNewItems<PropertyBase>(args))
    //            {
    //                TreeViewItem treeViewItem = MakeTreeViewItem(items, node);
    //                _ = AddToTree(treeViewItem.Items, node, state);
    //            }
    //        },
    //        () =>
    //        {
    //            state.OnNext(State.Completed);
    //        });
    //    return disposable;
    //}

    public class ViewModelPredicate : DescriptorFilters
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

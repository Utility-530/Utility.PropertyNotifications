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
using Utility.PropertyTrees.WPF.Meta;
using Utility.Nodes;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF
{
    public class ViewBuilder : BaseObject
    {
        public override Key Key => new(Guids.ViewBuilder, nameof(ViewBuilder), typeof(ViewBuilder));

        public Utility.Interfaces.Generic.IObservable<TreeViewResponse> OnNext(TreeViewRequest request)
        {
            return Create<TreeViewResponse>(observer =>
            {
                return BuildTree(request.TreeView, request.PropertyNode)
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

                });
            }); 
        }

        public IObservable<(int,int)> BuildTree(TreeView treeView, ValueNode property)
        {
            return PropertyExplorer.ExploreTree(treeView.Items, (items, prop) =>
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




                //_ = Observe<ActivationResponse, ActivationRequest>(new(node.Guid, new RootDescriptor(node), node, PropertyType.Root))
                //    .Select(a => a.PropertyNode)
                //.Subscribe(propertyNode =>
                //{
                //    propertyNode.Predicates = new ViewModelPredicate();
                //    PropertyExplorer.ExploreTree(new List<object>(), (a, b) => a, propertyNode)
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

using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Panels;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class PropertyView : UserControl
    {
        public PropertyView()
        {
            InitializeComponent();
            ViewModelTree.Engine = new Engine();
            this.Loaded += PropertyView_Loaded;
        }

        private void PropertyView_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyTree.SelectedObject = this.DataContext;
        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is IProperty property)
            {
                ViewModelTree.SelectedObject = property;
            }
        }

        public class Engine : IPropertyGridEngine
        {
            public Engine()
            {
            }

            public IPropertyNode Convert(object data)
            {
                if (data is IGuid guid)
                {
                    return new PropertyNode(guid.Guid) { Data = data, Predicates = new ViewModelPredicate() };
                }
                throw new Exception(" 4 wewfwe");
            }

            public static Engine Instance { get; } = new Engine();
        }

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

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            var treeView = new TreeView { };
            var property = PropertyTree.Source as PropertyNode;
            Create(treeView.Items, property);
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(treeView);
        }

        private static void Create(ItemCollection items, PropertyNode property)
        {
            foreach (var item in property.Children)
            {
                if (item is PropertyBase node)
                {
                    ItemsPanelTemplate? panelTemplate = default;
                    DataTemplate? headerTemplate = default;
                    if (node.ViewModel == null)
                    {
                    }

                    ViewModel viewModel = new ViewModel { CollectionPanel = new() { Grid = new() }, Panel = new() { Grid = new() { } }, Template = new() { } };
                    node.ViewModel = viewModel;
                    panelTemplate = viewModel.Panel?.Type != null ? (ItemsPanelTemplate)Application.Current.TryFindResource(viewModel.Panel.Type) : DefaultItemsPanelTemplate();
                    if (viewModel.Template.DataTemplateKey != null)
                        headerTemplate = (DataTemplate)Application.Current.TryFindResource(viewModel.Template.DataTemplateKey);
                    else
                    {
                        var key = new DataTemplateKey(node.PropertyType);
                        headerTemplate = (DataTemplate)Application.Current.TryFindResource(key);
                    }

                    var treeViewItem = new TreeViewItem() { Header = node/*, HeaderTemplate = headerTemplate*/, ItemsPanel = panelTemplate, IsExpanded = true };
                    //treeViewItem.ItemsPanel =
                    items.Add(treeViewItem);
                    Create(treeViewItem.Items, node);
                }
            }
        }

        static ItemsPanelTemplate DefaultItemsPanelTemplate()
        {
            FrameworkElementFactory factoryPanel = new FrameworkElementFactory(typeof(UniformStackPanel));
            factoryPanel.SetValue(Canvas.IsItemsHostProperty, true);
            ItemsPanelTemplate template = new ItemsPanelTemplate();
            template.VisualTree = factoryPanel;
            return template;

        }
    }
}
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Panels;
using Utility.Interfaces.NonGeneric;
using DryIoc;
using Utility.Infrastructure;
using Utility.Models;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class PropertyView : UserControl
    {
        public const string Key1 = nameof(PropertyView) + "1";
        //public const string Key2 = nameof(PropertyView) + "2";
        private readonly DryIoc.IContainer container;

        public PropertyView(DryIoc.IContainer container)
        {
            InitializeComponent();
            ViewModelTree.Engine = container.Resolve<ViewModelEngine>();
            PropertyTree.Engine = new Infrastructure.Engine(container.Resolve<PropertyNode>(Key1));
            this.Loaded += PropertyView_Loaded;
            this.container = container;
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
                    //if (node.ViewModel == null)
                    //{
                    //}

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

        private void initialise_click(object sender, RoutedEventArgs e)
        {
            AutoObject.Resolver.Initialise();
        }
    }

    public class ViewModelEngine : BaseObject, IPropertyGridEngine
    {
        Guid guid = Guid.Parse("78f35bd1-fc3c-44ca-8d86-f3a8a9d69d33");

        public ViewModelEngine()
        {
        }

        public override Key Key => new (guid, nameof(ViewModelEngine), typeof(ViewModelEngine));

        public async Task<IPropertyNode> Convert(object data)
        {
            if (data is IGuid guid)
            {
                var propertyNode = await Observe<PropertyNode, ActivationRequest>(new(guid.Guid, new RootDescriptor(data), data, PropertyType.Root)).ToTask();
                propertyNode.Data = data;
                propertyNode.Predicates = new ViewModelPredicate() ;
                return propertyNode;
            }
            throw new Exception(" 4 wewfwe");
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
    }
}
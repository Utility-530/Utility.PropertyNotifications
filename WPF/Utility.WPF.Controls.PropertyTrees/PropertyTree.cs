using Splat;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;
using Utility.Nodes.WPF;
using Utility.PropertyDescriptors;
using Utility.WPF.Controls.Trees;
using Utility.WPF.Templates;

namespace Utility.WPF.Controls.PropertyTrees
{
    public class PropertyTree : CustomTreeView
    {
        static HashSet<string> keys = new();
        private string _key;
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(nameof(Object), typeof(object), typeof(PropertyTree), new PropertyMetadata(null, changed));
        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register("Schema", typeof(Schema), typeof(PropertyTree), new PropertyMetadata());

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyTree tree && e.NewValue is { } value)
            {
                tree.ItemsSource = create(value).Items;
            }

            Node create(object obj)
            {
                var root = DescriptorFactory.CreateRoot(obj, tree._key);
                var reflectionNode = new Node(root) { IsExpanded = true };
                NodeEngine.Instance.Add(reflectionNode);
                return reflectionNode;
            }
        }

        static PropertyTree()
        {
            Style dynamicStyle = new(typeof(CustomTreeView)) { };
            dynamicStyle.Setters.Add(new Setter(TreeView.ItemTemplateSelectorProperty, Utility.Nodes.WPF.DataTemplateSelector.Instance));
            dynamicStyle.Setters.Add(new Setter(TreeView.ItemContainerStyleSelectorProperty, Utility.Nodes.WPF.StyleSelector.Instance));
        

            StyleProperty.OverrideMetadata(typeof(PropertyTree), new FrameworkPropertyMetadata(dynamicStyle));
        }

        static DataTemplate createEditTemplate()
        {
            return Factorys.TemplateGenerator.CreateDataTemplate(() =>
            {
                var propertyTree = new PropertyTree() { };
                BindingOperations.SetBinding(
                   propertyTree,
                   PropertyTree.ObjectProperty,
                   new Binding { Path = new PropertyPath(".") });
                return propertyTree;
            });
        }


        public PropertyTree()
        {
            string key = "obj";
            while (keys.Add(key) == false)
            {
                key = Utility.Randoms.Next.Instance.Word();
            }
            this._key = key;
       
            //Locator.CurrentMutable.RegisterConstant<IContext>(new Context());
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(Utility.Nodes.WPF.Expander.Instance);
            Locator.CurrentMutable.RegisterConstant<System.Windows.Controls.DataTemplateSelector>(CustomDataTemplateSelector.Instance);
        }



        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        public Schema Schema
        {
            get { return (Schema)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if(element is CustomTreeViewItem treeViewItem && item is Node{ Data: ICollectionDescriptor collectionDescriptor })
            {
                var innerType = collectionDescriptor.ElementType;
                treeViewItem.Edit = ActivateAnything.Activate.New(innerType);
            }

            base.PrepareContainerForItemOverride(element, item);
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = base.GetContainerForItemOverride();

            if(item is CustomTreeViewItem customTreeViewItem)
            {
                customTreeViewItem.EditTemplate = createEditTemplate();
   
            }
            return item;
        }
    }
}



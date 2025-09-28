using ReactiveUI;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class PropertySelectorBehavior : TreeSelectorBehavior
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(PropertySelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty PropertyInfoProperty = DependencyProperty.Register("PropertyInfo", typeof(PropertyInfo), typeof(PropertySelectorBehavior), new PropertyMetadata(PropertyInfoChanged));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject: { } ass } typeSelector)
            {
                if (e.NewValue is Type type)
                {
                    typeSelector.AssociatedObject.ItemsSource = type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(a => typeSelector.Filter?.Equals(a.PropertyType) != false)
                        .Select(a => a);
                }
            }
        }

        private static void PropertyInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector)
            {
                if (e.NewValue is PropertyInfo type)
                    typeSelector.ChangeInfo(tree, type);
            }
        }

        //private static void AssembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is PropertySelectorBehavior typeSelector && e.NewValue is IEnumerable enumerable)
        //    {
        //        Set(typeSelector, enumerable);
        //        if (enumerable is IReadOnlyTree tree && typeSelector.Type is Type type)
        //            typeSelector.ChangeType(tree, type);
        //    }
        //}

        protected override void OnAttached()
        {
            //AssociatedObject.SelectedItemTemplateSelector = CustomItemTemplateSelector.Instance;

            AssociatedObject.WhenAnyValue(a => a.SelectedNode)
                .OfType<IGetData>()
                .Select(a => a.Data)
                .Subscribe(a =>
                {
                    if (a is Type type)
                        Type = type;
                    else if (a is IType { Type: { } _type })
                    {
                        Type = _type;
                    }
                });

            //if (UseEntryAssembly)
            //{
            //    Assemblies = new List<Assembly>([Assembly.GetEntryAssembly()]);
            //}
            //if (Assemblies != null)
            //    Set(this, Assemblies);

            AssociatedObject.OnLoaded(a =>
            {
                if (Type is Type type)
                {
                    AssociatedObject.ItemsSource =
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(a => Filter?.Equals(a.PropertyType) != false)
                    .Select(a => a);
                }
                if (PropertyInfo is PropertyInfo propertyInfo && AssociatedObject.TreeView != null && AssociatedObject.ItemsSource is IReadOnlyTree _tree)
                {
                    ChangeInfo(_tree, propertyInfo);
                }
            });

            base.OnAttached();
        }

        void ChangeInfo(IReadOnlyTree tree, PropertyInfo _propertyInfo)
        {
            if (tree.Descendant(a => (a.tree as IGetData).Data is PropertyInfo type && type == _propertyInfo || (a.tree as IGetData).Data is IPropertyInfo itype && itype.PropertyInfo == _propertyInfo) is IReadOnlyTree { } innerTree)
            {
                AssociatedObject.IsError = false;
                AssociatedObject.UpdateSelectedItems(innerTree);
                if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                AssociatedObject.SelectedNode = innerTree;
            }
            else
            {
                AssociatedObject.IsError = true;
            }
        }



        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public PropertyInfo PropertyInfo
        {
            get { return (PropertyInfo)GetValue(PropertyInfoProperty); }
            set { SetValue(PropertyInfoProperty, value); }
        }



        public Type Filter
        {
            get { return (Type)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(Type), typeof(PropertySelectorBehavior), new PropertyMetadata(_changed));

        private static void _changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject: { } ass } typeSelector)
            {
                if (e.NewValue is Type type)
                {
                    typeSelector.AssociatedObject.ItemsSource = typeSelector.Type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(a => typeSelector.Filter?.Equals(a.PropertyType) != false)
                        .Select(a => a);
                }
            }
        }
    }
}

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
        //public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(TypeSelectorBehavior), new PropertyMetadata(AssembliesChanged));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(PropertySelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty PropertyInfoProperty = DependencyProperty.Register("PropertyInfo", typeof(PropertyInfo), typeof(PropertySelectorBehavior), new PropertyMetadata(PropertyInfoChanged));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject:{ } ass } typeSelector)
            {
                if (e.NewValue is Type type)
                {
                    typeSelector.AssociatedObject.ItemsSource = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
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
                .OfType<IReadOnlyTree>()
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
                if (Type is Type type )
                {
                    AssociatedObject.ItemsSource = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(a=> new ViewModelTree(a));
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
            if (tree.Descendant(a => a.tree.Data is PropertyInfo type && type == _propertyInfo || a.tree.Data is IPropertyInfo itype && itype.PropertyInfo == _propertyInfo) is IReadOnlyTree { } innerTree)
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

        //static void Set(PropertySelectorBehavior typeSelector, IEnumerable enumerable)
        //{
        //    var assemblyTree = enumerable.Cast<Assembly>().ToArray().ToViewModelTree();
        //    typeSelector.AssociatedObject.ItemsSource = assemblyTree;
        //}

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


        //public IEnumerable Assemblies
        //{
        //    get { return (IEnumerable)GetValue(AssembliesProperty); }
        //    set { SetValue(AssembliesProperty, value); }
        //}

        //public bool UseEntryAssembly
        //{
        //    get { return (bool)GetValue(UseEntryAssemblyProperty); }
        //    set { SetValue(UseEntryAssemblyProperty, value); }
        //}

        //class CustomItemTemplateSelector : DataTemplateSelector
        //{
        //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
        //    {
        //        if (item is IReadOnlyTree { Data: var data } tree)
        //        {
        //            if (data is Type || data is IType || data is Assembly || data is IGetAssembly)
        //                return TemplateGenerator.CreateDataTemplate(() =>
        //                {
        //                    var textBlock = new TextBlock { };
        //                    Binding binding = new() { Path = new PropertyPath(nameof(System.Type.Name)) };
        //                    Binding binding2 = new() { Path = new PropertyPath(nameof(IReadOnlyTree.Data)) };
        //                    textBlock.SetBinding(TextBlock.TextProperty, binding);
        //                    textBlock.SetBinding(FrameworkElement.DataContextProperty, binding2);
        //                    return textBlock;
        //                });
        //            return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = data }, Margin = new Thickness(4, 0, 4, 0) });
        //        }
        //        throw new Exception("d ss!$sd");
        //    }

        //    public static CustomItemTemplateSelector Instance { get; } = new();
        //}
    }
}

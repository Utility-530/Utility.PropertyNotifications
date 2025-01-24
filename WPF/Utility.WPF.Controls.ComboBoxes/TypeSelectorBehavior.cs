using MoreLinq;
using ReactiveUI;
using System;
using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Trees.Extensions;
using Utility.Extensions;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class TypeSelectorBehavior : TreeSelectorBehavior
    {
        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(TypeSelectorBehavior), new PropertyMetadata(AssembliesChanged));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeSelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty UseEntryAssemblyProperty = DependencyProperty.Register("UseEntryAssembly", typeof(bool), typeof(TypeSelectorBehavior), new PropertyMetadata(false));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector )
            {
                if (e.NewValue is Type type)
                    typeSelector.ChangeType(tree, type);
            }
        }

        private static void AssembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelectorBehavior typeSelector && e.NewValue is IEnumerable enumerable)
            {
                Set(typeSelector, enumerable);
                if (enumerable is IReadOnlyTree tree && typeSelector.Type is Type type)
                    typeSelector.ChangeType(tree, type);
            }
        }

        protected override void OnAttached()
        {

            this.AssociatedObject.SelectedItemTemplateSelector = CustomItemTemplateSelector.Instance;

            this.AssociatedObject.WhenAnyValue(a => a.SelectedNode)
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


            if (UseEntryAssembly)
            {
                Assemblies = new List<Assembly>([Assembly.GetEntryAssembly()]);
            }
            if (Assemblies != null)
                Set(this, Assemblies);

            if (Type is Type type && AssociatedObject.TreeView != null && this.AssociatedObject.ItemsSource is IReadOnlyTree tree)
            {
                ChangeType(tree, type);
            }

            base.OnAttached();
        }
        void ChangeType(IReadOnlyTree tree, Type _type)
        {
            if (tree.Descendant(a => (a.tree.Data is Type type && type == _type) ||(a.tree.Data is IType itype && itype.Type == _type)) is IReadOnlyTree { } innerTree)
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

        static void Set(TypeSelectorBehavior typeSelector, IEnumerable enumerable)
        {
            var assemblyTree = enumerable.Cast<Assembly>().ToArray().ToViewModelTree();
            typeSelector.AssociatedObject.ItemsSource = assemblyTree;
        }


        //public override void OnApplyTemplate()
        //{

        //    base.OnApplyTemplate();
        //    if (Assemblies != null)
        //        Set(this, Assemblies);

        //    if (Type is Type type && AssociatedObject.TreeView != null && this.AssociatedObject.ItemsSource is IReadOnlyTree tree)
        //    {
        //        ChangeType(tree, type);
        //    }
        //}


        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }


        public IEnumerable Assemblies
        {
            get { return (IEnumerable)GetValue(AssembliesProperty); }
            set { SetValue(AssembliesProperty, value); }
        }

        public bool UseEntryAssembly
        {
            get { return (bool)GetValue(UseEntryAssemblyProperty); }
            set { SetValue(UseEntryAssemblyProperty, value); }
        }


        class CustomItemTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is IReadOnlyTree { Data: var data } tree)
                {
                    if (data is Type || data is IType || data is Assembly || data is IGetAssembly)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {

                            var textBlock = new TextBlock { };
                            Binding binding = new() { Path = new PropertyPath(nameof(System.Type.Name)) };
                            Binding binding2 = new() { Path = new PropertyPath(nameof(IReadOnlyTree.Data)) };
                            textBlock.SetBinding(TextBlock.TextProperty, binding);
                            textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                            return textBlock;
                        });

                    return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = data }, Margin = new Thickness(4, 0, 4, 0) });

                }
                throw new Exception("d ss!$sd");
            }

            public static CustomItemTemplateSelector Instance { get; } = new();
        }

    }
}

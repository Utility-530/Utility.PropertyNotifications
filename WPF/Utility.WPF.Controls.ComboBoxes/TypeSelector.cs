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
using Utility.Extensions;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class TypeSelector : TreeSelector
    {
        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(TypeSelector), new PropertyMetadata(AssembliesChanged));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeSelector), new PropertyMetadata(TypeChanged));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelector { ItemsSource: IReadOnlyTree tree } typeSelector && e.NewValue is Type type)
            {
                typeSelector.ChangeType(tree, type);
            }
        }

        private static void AssembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelector typeSelector && e.NewValue is IEnumerable enumerable)
            {
                Set(typeSelector, enumerable);
                if (enumerable is IReadOnlyTree tree && typeSelector.Type is Type type)
                    typeSelector.ChangeType(tree, type);
            }
        }


        public TypeSelector()
        {
            this.SelectedItemTemplateSelector = CustomItemTemplateSelector.Instance;

            this.WhenAnyValue(a => a.SelectedNode)
                .OfType<IReadOnlyTree>()
                .Select(a => a.Data)
                .OfType<Type>()
                .Subscribe(a =>
                {
                    Type = a;
                });

            //this.WhenAnyValue(a => a.ItemsSource)
            //    .WhereNotNull()
            //    .Subscribe(a =>
            //    {
            //        if (a is IReadOnlyTree tree && Type is Type type)
            //            ChangeType(tree, type);
            //    });

        }

        void ChangeType(IReadOnlyTree tree, Type _type)
        {
            if (tree.MatchDescendant(a => a.Data is Type type && type == _type) is IReadOnlyTree { } innerTree)
            {
                IsError = false;
                UpdateSelectedItems(innerTree);
                if (_treeView?.ItemContainerGenerator.ContainerFromItem(_treeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                SelectedNode = innerTree;
            }
            else
            {
                IsError = true;
            }
        }


 
        static void Set(TypeSelector typeSelector, IEnumerable enumerable)
        {

            var assemblyTree = Ex.ToTree(enumerable.Cast<Assembly>().ToArray());

            typeSelector.ItemsSource = assemblyTree; 
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Assemblies != null)
                Set(this, Assemblies);

            if (Type is Type type && _treeView != null && this.ItemsSource is IReadOnlyTree tree)
            {
                ChangeType(tree, type);
            }
        }


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

        class CustomItemTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is IReadOnlyTree { Data: var data } tree)
                {
                    if (data is Type type)
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

        //private void ExpandAll(ItemsControl items, Predicate<object> predicate)
        //{
        //    bool b = false;
        //    foreach (object obj in items.Items)
        //    {
        //        if (b) break;
        //        ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
        //        if (childControl != null)
        //        {
        //            ExpandAll(childControl, predicate);
        //        }
        //        TreeViewItem item = childControl as TreeViewItem;
        //        if (item != null)
        //        {
        //            item.IsExpanded = true;
        //            item.IsSelected = true;
        //        }
        //        if (predicate(item))
        //        {
        //            item.IsSelected = true;
        //        }
        //        b = true;
        //    }
        //}
    }
}

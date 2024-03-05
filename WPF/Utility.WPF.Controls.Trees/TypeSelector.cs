using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;

namespace Utility.WPF.Controls.Trees
{
    public class TypeSelector : TreeSelector
    {
        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(TypeSelector), new PropertyMetadata(Changed));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeSelector), new PropertyMetadata(TypeChanged));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelector typeSelector && e.NewValue is Type type)
            {
                typeSelector.ChangeType();
            }
        }

        void ChangeType()
        {
            if (Type != null && _treeView != null && this.ItemsSource is Tree tree)
            {
                if (tree.MatchDescendant(a => a.Data is Type type && type == Type) is ViewModelTree { } innerTree)
                {
                    IsError = false;
                    innerTree.IsSelected = true;
                    innerTree.OnPropertyChanged(nameof(ViewModelTree.IsSelected));
                    //behavior.SelectedItem = innerTree;
                    UpdateSelectedItem(innerTree);

                }
                else
                {
                    IsError = true;
                }
            }
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelector typeSelector && e.NewValue is IEnumerable enumerable)
            {
                typeSelector.ItemsSource = Ex.ToTree(enumerable.Cast<Assembly>().ToArray());
            }
        }

        public TypeSelector()
        {
            if (Assemblies != null)
                this.ItemsSource = Ex.ToTree(Assemblies.Cast<Assembly>().ToArray());
            this.ItemTemplateSelector = CustomItemTemplateSelector.Instance;
            this.WhenAnyValue(a => a.SelectedItems)
                .Subscribe(a =>
                {
                    if (a is IEnumerable enumerable)
                        foreach (var x in enumerable)
                            if (x is IReadOnlyTree { Data: Type type })
                                Type = type;
                });

            this.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    ChangeType();
                });



            //behavior = new TreeViewSelectionBehavior();
            //behavior.HierarchyPredicate = Pred;
            //behavior.WhenAnyValue(a => a.SelectedItem)
            //    .Subscribe(a =>
            //    {

            //    });


        }

        //private bool Pred(object a, object b)
        //{
        //    return true;
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Interaction.GetBehaviors(_treeView).Add(behavior);
            ChangeType();
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
                if (item is IReadOnlyTree { Data: System.Type type })
                {
                    return TemplateGenerator.CreateDataTemplate(() =>
                    {

                        var textBlock = new TextBlock { };
                        Binding binding = new() { Path = new PropertyPath(nameof(System.Type.Name)) };
                        Binding binding2 = new() { Path = new PropertyPath(nameof(IReadOnlyTree.Data)) };
                        textBlock.SetBinding(TextBlock.TextProperty, binding);
                        textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                        return textBlock;
                    });
                }
                return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(2, 0, 2, 0) });

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

using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Ex;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class TypeSelectorBehavior : TreeSelectorBehavior
    {
        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(TypeSelectorBehavior), new PropertyMetadata(AssembliesChanged));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeSelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty UseEntryAssemblyProperty = DependencyProperty.Register("UseEntryAssembly", typeof(bool), typeof(TypeSelectorBehavior), new PropertyMetadata(false));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector)
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

            this.AssociatedObject.ValueCoercing += (s, e) =>
            {
                if (e.NewValue is not IGetValue { Value: Type { } type })
                {
                    e.Cancel = true;
                }
            };
            this.AssociatedObject
                .Observe(a => a.SelectedNode)
                .OfType<IGetValue>()
                .Select(a => a.Value)
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

            AssociatedObject.OnLoaded(a =>
            {
                if (Type is Type type && AssociatedObject.TreeView != null && this.AssociatedObject.ItemsSource is IReadOnlyTree tree)
                {
                    ChangeType(tree, type);
                }
            });

            base.OnAttached();
        }

        private void ChangeType(IReadOnlyTree tree, Type _type)
        {
            if (tree.Descendant(a => (a.tree.Value() is Type type && type == _type) || (a.tree.Value() is IType itype && itype.Type == _type)) is IReadOnlyTree { } innerTree)
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

        private static void Set(TypeSelectorBehavior typeSelector, IEnumerable enumerable)
        {
            var assemblyTree = enumerable.Cast<Assembly>().ToArray().ToNodeViewModel();
            typeSelector.AssociatedObject.ItemsSource = assemblyTree;
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

        public bool UseEntryAssembly
        {
            get { return (bool)GetValue(UseEntryAssemblyProperty); }
            set { SetValue(UseEntryAssemblyProperty, value); }
        }

        public override DataTemplateSelector SelectedTemplateSelector() =>
            DataTemplateHelper.CreateSelector((item, b) =>
            {
                if (item is IGetValue { Value: var value } tree)
                {
                    if (value is Type || value is IType)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var textBlock = new TextBlock { };
                            Binding binding = new() { Path = new PropertyPath(nameof(System.Type.Name)) };
                            Binding binding2 = new() { Path = new PropertyPath(nameof(IGetData.Data)) };
                            textBlock.SetBinding(TextBlock.TextProperty, binding);
                            textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                            return textBlock;
                        });
                    else if (value is Assembly assembly)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var textBlock = new TextBlock { Text = assembly.GetName().Name };
                            return textBlock;
                        });
                    return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = value }, Margin = new Thickness(4, 0, 4, 0) });
                }
                throw new Exception("d ss!$sd");
            });

    }
}
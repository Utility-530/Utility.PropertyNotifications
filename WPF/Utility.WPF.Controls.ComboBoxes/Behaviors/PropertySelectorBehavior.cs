using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.ComboBoxes
{


    public class PropertySelectorBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(PropertySelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty PropertyInfoProperty = DependencyProperty.Register("PropertyInfo", typeof(PropertyInfo), typeof(PropertySelectorBehavior), new PropertyMetadata(PropertyInfoChanged));
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(Type), typeof(PropertySelectorBehavior), new PropertyMetadata(_changed));

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject: { } ass } typeSelector)
            {
                if (e.NewValue is Type type)
                {
                    typeSelector.AssociatedObject.ItemsSource = typeSelector.toCollection(type, a => typeSelector.Filter?.Equals(a.PropertyType) != false);

                    //typeSelector.AssociatedObject.ItemTemplateSelector =
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

        protected override void OnAttached()
        {
            AssociatedObject.ItemTemplateSelector = DataTemplateSelector();

            AssociatedObject.Observe(a => a.SelectedItem)
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

            AssociatedObject.OnLoaded(a =>
            {
                if (Type is Type type)
                {
                    AssociatedObject.ItemsSource = toCollection(type, a => Filter?.Equals(a.PropertyType) != false);

                }
                if (PropertyInfo is PropertyInfo propertyInfo && AssociatedObject.ItemsSource is IReadOnlyTree _tree)
                {
                    ChangeInfo(_tree, propertyInfo);
                }
            });

            base.OnAttached();
        }

        private void ChangeInfo(IReadOnlyTree tree, PropertyInfo _propertyInfo)
        {
            if (tree.Descendant(a => (a.tree as IGetValue).Value is PropertyInfo type && type == _propertyInfo || (a.tree as IGetValue).Value is IPropertyInfo itype && itype.PropertyInfo == _propertyInfo) is IReadOnlyTree { } innerTree)
            {
                //AssociatedObject.IsError = false;
                //AssociatedObject.UpdateSelectedItems(innerTree);
                //if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                //    item.IsSelected = true;
                AssociatedObject.SelectedItem = innerTree;
            }
            else
            {
                //AssociatedObject.IsError = true;
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

        public DataTemplateSelector DataTemplateSelector() =>
                 DataTemplateHelper.CreateSelector((item, b) =>
                 {
                     if (item is IGetData { Data: var data } tree)
                     {
                         if (data is PropertyInfo propertyInfo)
                             return TemplateGenerator.CreateDataTemplate(() =>
                             {
                                 var textBlock = new TextBlock { };
                                 var wrapPanel = new WrapPanel { };
                                 wrapPanel.Children.Add(textBlock);
                                 Binding binding = new() { Path = new PropertyPath(nameof(PropertyInfo.Name)) };
                                 Binding binding2 = new() { Path = new PropertyPath(nameof(IGetData.Data)) };
                                 textBlock.SetBinding(TextBlock.TextProperty, binding);
                                 textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                                 return wrapPanel;
                             });

                         return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = data }, Margin = new Thickness(4, 0, 4, 0) });
                     }
                     throw new Exception("d ss!$sd");
                 });

        private IEnumerable toCollection(Type type, Predicate<PropertyInfo> predicate)
        {
            foreach (var nodeViewModel in type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(a => predicate(a))
                        .OrderBy(a => a.Name)
                        .Select(a => new NodeViewModel(a)))
            {
                nodeViewModel
                    .WhenReceivedFrom(a => a.IsChecked, includeInitialValue: false)
                    .Subscribe(a =>
                    {
                        if (AssociatedObject.DataContext is PendingConnectorViewModel viewModel)
                        {
                            if (a)
                            {
                                viewModel.ChangeConnectorsCommand.Execute(new CollectionChanges(new[] { nodeViewModel }, Array.Empty<object>()));
                            }
                            else
                            {
                                viewModel.ChangeConnectorsCommand.Execute(new CollectionChanges(Array.Empty<object>(), new[] { nodeViewModel }));
                            }
                        }
                    });
                yield return nodeViewModel;
            }
            //.GroupBy(a => a.PropertyType)
            //.Select(a => new Model(() => a.Select(a => new NodeViewModel(a))) { Data = a.Key, Name = a.Key.Name });
        }

        private static void _changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertySelectorBehavior { AssociatedObject: { } ass } typeSelector)
            {
                if (e.NewValue is Type type)
                {
                    typeSelector.AssociatedObject.ItemsSource = typeSelector.toCollection(type, a => typeSelector.Filter?.Equals(a.PropertyType) != false);
                }
            }
        }
    }
}
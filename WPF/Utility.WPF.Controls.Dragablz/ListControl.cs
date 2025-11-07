using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Dragablz;
using Utility.Helpers.Reflection;
using Utility.WPF.Attached;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Dragablz
{
    public class ListControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(ListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsRefreshablePathProperty = DependencyProperty.Register("IsRefreshablePath", typeof(string), typeof(ListControl), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandPathProperty = DependencyProperty.Register("CommandPath", typeof(string), typeof(ListControl), new PropertyMetadata(null));

        static ListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListControl), new FrameworkPropertyMetadata(typeof(ListControl)));
        }

        public string IsCheckedPath
        {
            get => (string)GetValue(IsCheckedPathProperty);
            set => SetValue(IsCheckedPathProperty, value);
        }

        public string IsRefreshablePath
        {
            get => (string)GetValue(IsRefreshablePathProperty);
            set => SetValue(IsRefreshablePathProperty, value);
        }

        public string CommandPath
        {
            get => (string)GetValue(CommandPathProperty);
            set => SetValue(CommandPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();

            CreateAndSetTextBinding();
            CreateAndSetCommandBinding();
            CreateAndSetIsCheckedBinding();
            CreateAndSetRefreshableBinding();

            base.PrepareContainerForItemOverride(element, item);

            void CreateAndSetTextBinding()
            {
                if (string.IsNullOrEmpty(DisplayMemberPath))
                    return;
                if (element.ChildOfType<TextBox>() is not TextBox textBox)
                    return;

                textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
                textBox.GotFocus += TextBox_GotFocus;
                var prop = item.GetType().GetProperty(DisplayMemberPath);
                var isReadOnly = prop.IsReadOnly() || prop.IsInitOnly();
                var binding = new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                textBox.IsReadOnly = isReadOnly;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }

            /// this will override the drag behavior
            void CreateAndSetIsCheckedBinding()
            {
                if (string.IsNullOrEmpty(IsCheckedPath) == false)
                {
                    BindingOperations.SetBinding(element, Ex.StateProperty, CreateBinding());
                    BindingOperations.SetBinding(element, Ex.IsCheckedProperty, CreateBinding2());
                }

                Binding CreateBinding()
                {
                    var prop = item.GetType().GetProperty(IsCheckedPath);
                    var isReadOnly = prop.IsReadOnly() || prop.IsInitOnly() || prop.GetSetMethod() == null;
                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(IsCheckedPath),
                        Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                        Converter = new ValueConverter(),
                        ConverterParameter = IsCheckedPathProperty,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    return binding;
                }

                Binding CreateBinding2()
                {
                    var prop = item.GetType().GetProperty(IsCheckedPath);
                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(IsCheckedPath),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    return binding;
                }
            }

            /// this will override the drag behavior
            void CreateAndSetRefreshableBinding()
            {
                if (string.IsNullOrEmpty(IsRefreshablePath) == false)
                    BindingOperations.SetBinding(element, Ex.StateProperty, CreateBinding());

                Binding CreateBinding()
                {
                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(IsRefreshablePath),
                        Mode = BindingMode.OneWay,
                        Converter = new ValueConverter(),
                        ConverterParameter = IsRefreshablePathProperty,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    return binding;
                }
            }

            void CreateAndSetCommandBinding()
            {
                if (element.ChildOfInterface<ICommandSource>() is not DependencyObject dependencyObjec || string.IsNullOrEmpty(CommandPath))
                    return;

                BindingOperations.SetBinding(dependencyObjec, ButtonBase.CommandProperty, CreateCommandBinding(item));

                Binding? CreateCommandBinding(object item)
                {
                    if (string.IsNullOrEmpty(CommandPath))
                        return null;

                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(CommandPath),
                        Mode = BindingMode.OneTime
                    };
                    return binding;
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void TextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }
    }

    internal class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool balue)
                throw new Exception("Expected boolean type.sdfsdf");
            if (parameter is not DependencyProperty { Name: { } name })
                throw new Exception("Expected DependencyProperty type.sdfsdf");

            if (name == ListControl.IsCheckedPathProperty.Name)
            {
                return (bool?)balue switch { true => State.Ticked, false => State.Crossed, null => State.None };
            }
            else if (name == ListControl.IsRefreshablePathProperty.Name)
            {
                return (bool?)balue switch { true => State.Refreshable, false => State.None, null => State.None };
            }

            throw new ArgumentOutOfRangeException("Expected boolean type.sdfsdf");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
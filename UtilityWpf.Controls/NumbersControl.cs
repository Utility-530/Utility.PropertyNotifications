using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Utility.WPF.Helper;
using Utility.WPF.Controls.Base;

namespace UtilityWpf.Controls
{
    public class NumberItem : ListBoxItem
    {
        static NumberItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberItem), new FrameworkPropertyMetadata(typeof(NumberItem)));
        }
    }

    public class NumbersControl : ListBox<NumberItem>
    {
        public static readonly DependencyProperty DisplayKeyPathProperty = DependencyProperty.Register("DisplayKeyPath", typeof(string), typeof(NumbersControl), new PropertyMetadata(null));

        static NumbersControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumbersControl), new FrameworkPropertyMetadata(typeof(NumbersControl)));
        }

        public string DisplayKeyPath
        {
            get { return (string)GetValue(DisplayKeyPathProperty); }
            set { SetValue(DisplayKeyPathProperty, value); }
        }

        protected override NumberItem InitialiseItem(NumberItem item, object viewModel)
        {
            if (item is not Control control)
                return item;
            if (VisualTreeHelper.GetChildrenCount(control) == 0)
                _ = control.ApplyTemplate();

            SetTextBinding(item, viewModel);
            SetValueBinding(item, viewModel);

            return item;
        }

        private void SetValueBinding(NumberItem item, object viewModel)
        {
            if (string.IsNullOrEmpty(DisplayKeyPath))
                return;
            if (item.ChildOfType<TextBlock>() is not TextBlock textBlock)
                return;

            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, CreateKeyBinding(viewModel));

            Binding CreateKeyBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayKeyPath),
                    Mode = BindingMode.OneTime
                };
            }
        }

        private void SetTextBinding(NumberItem item, object viewModel)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;

            if (item.ChildOfType<SpinnerControl>() is not SpinnerControl numberBox)
                return;

            BindingOperations.SetBinding(numberBox, SpinnerControl.ValueProperty, CreateBinding(viewModel));

            Binding CreateBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            }
        }
    }
}
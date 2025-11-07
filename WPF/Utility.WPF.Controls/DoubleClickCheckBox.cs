using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Evan.Wpf;

namespace Utility.WPF.Controls
{
    public class DoubleClickCheckBox : ContentControl
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyHelper.Register<bool>();

        private static readonly RoutedEvent IsCheckedChangedEvent = EventManager.RegisterRoutedEvent("IsCheckedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(DoubleClickCheckBox));

        static DoubleClickCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleClickCheckBox), new FrameworkPropertyMetadata(typeof(DoubleClickCheckBox)));
        }

        public DoubleClickCheckBox()
        {
            this.MouseDown += DoubleClickCheckBox_MouseDown;
        }

        public event RoutedEventHandler IsCheckedChanged
        {
            add { AddHandler(IsCheckedChangedEvent, value); }
            remove { RemoveHandler(IsCheckedChangedEvent, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private void DoubleClickCheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                this.IsChecked = !this.IsChecked;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedPropertyChangedEventArgs<bool>(false, true, IsCheckedChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedPropertyChangedEventArgs<bool>(true, false, IsCheckedChangedEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
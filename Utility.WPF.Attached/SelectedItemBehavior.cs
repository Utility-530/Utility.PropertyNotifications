using System.Windows;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Attached
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/18456836/getting-selected-item-in-itemscontrol"></a>
    /// </summary>
    public static class SelectedItemBehavior
    {
        public static readonly DependencyProperty BindingProperty =
           DependencyProperty.RegisterAttached("Binding", typeof(object), typeof(SelectedItemBehavior),
              new FrameworkPropertyMetadata(new object(),
                 FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                 SelectedItem_Changed));

        public static object GetBinding(FrameworkElement frameworkElement)
        {
            return frameworkElement.GetValue(BindingProperty);
        }

        public static void SetBinding(FrameworkElement frameworkElement, object value)
        {
            frameworkElement.SetValue(BindingProperty, value);
        }

        private static void SelectedItem_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            toggleButton.Checked -= ToggleButtonOnChecked;
            toggleButton.IsChecked = e.NewValue?.Equals(toggleButton.DataContext) ?? false;
            toggleButton.Checked += ToggleButtonOnChecked;
        }

        private static void ToggleButtonOnChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            SetBinding(toggleButton, toggleButton.DataContext);
        }
    }
}
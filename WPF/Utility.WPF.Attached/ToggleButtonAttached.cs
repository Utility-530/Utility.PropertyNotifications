using System.Windows;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Attached
{
    public static class ToggleButtonAttached
    {
        public static readonly DependencyProperty OnContentProperty = DependencyProperty.RegisterAttached(
            "OnContent", typeof(object), typeof(ToggleButtonAttached), new PropertyMetadata(default, OnContentPropertyChangedCallback));

        public static readonly DependencyProperty OffContentProperty = DependencyProperty.RegisterAttached(
            "OffContent", typeof(object), typeof(ToggleButtonAttached), new PropertyMetadata(default));

        private static void OnContentPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ToggleButton toggleButton)
            {
                SetOffContent(toggleButton, toggleButton.Content);
                SetOnContent(toggleButton, dependencyPropertyChangedEventArgs.NewValue);
                if (toggleButton.IsChecked == true)
                {
                    toggleButton.SetValue(System.Windows.Controls.ContentControl.ContentProperty, GetOnContent(toggleButton));
                }
                toggleButton.Checked += (_, _) => ToggleButton_Checked(toggleButton, dependencyPropertyChangedEventArgs.NewValue);
                toggleButton.Unchecked += (_, _) => ToggleButton_UnChecked(toggleButton, dependencyPropertyChangedEventArgs.NewValue);
                toggleButton.Unloaded -= (_, _) => ToggleButton_Checked(toggleButton, dependencyPropertyChangedEventArgs.NewValue);
                toggleButton.Unloaded -= (_, _) => ToggleButton_UnChecked(toggleButton, dependencyPropertyChangedEventArgs.NewValue);
            }
        }

        private static void ToggleButton_Checked(ToggleButton toggleButton, object value)
        {
            toggleButton.SetValue(System.Windows.Controls.ContentControl.ContentProperty, value);
        }

        private static void ToggleButton_UnChecked(ToggleButton toggleButton, object value)
        {
            toggleButton.SetValue(System.Windows.Controls.ContentControl.ContentProperty, GetOffContent(toggleButton));
        }

        public static void SetOnContent(DependencyObject element, object value)
        {
            element.SetValue(OnContentProperty, value);
        }

        public static object GetOnContent(DependencyObject element)
        {
            return element.GetValue(OnContentProperty);
        }

        public static void SetOffContent(DependencyObject element, object value)
        {
            element.SetValue(OffContentProperty, value);
        }

        public static object GetOffContent(DependencyObject element)
        {
            return element.GetValue(OffContentProperty);
        }

        public static readonly DependencyProperty OnContentTemplateProperty = DependencyProperty.RegisterAttached(
            "OnContentTemplate", typeof(DataTemplate), typeof(ToggleButtonAttached), new PropertyMetadata(default(DataTemplate)));

        public static void SetOnContentTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(OnContentTemplateProperty, value);
        }

        public static DataTemplate GetOnContentTemplate(DependencyObject element)
        {
            return (DataTemplate)element.GetValue(OnContentTemplateProperty);
        }
    }
}
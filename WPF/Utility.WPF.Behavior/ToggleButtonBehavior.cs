using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Behavior
{
    public class ToggleButtonContentBehavior : Behavior<ToggleButton>
    {
        public static readonly DependencyProperty UnCheckedContentProperty =
            DependencyProperty.Register(nameof(UnCheckedContent), typeof(object), typeof(ToggleButtonContentBehavior), new FrameworkPropertyMetadata(null));

        protected override void OnAttached()
        {
            object? checkedContent = null;
            base.OnAttached();
            ConfigureContent(AssociatedObject, this.WhenAnyValue(a => a.UnCheckedContent), checkedContent);
        }

        public object UnCheckedContent
        {
            get { return (object)GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }

        public static void ConfigureContent(ToggleButton toggleButton, IObservable<object> uncheckedContentObservable, object? checkedContent)
        {
            object? uncheckedContent = null;
            uncheckedContentObservable.WhereNotNull().Subscribe(a =>
            {
                uncheckedContent = a;
                if (checkedContent == null)
                {
                    checkedContent = toggleButton.Content;
                    toggleButton.Content = toggleButton.IsChecked == true ? toggleButton.Content : uncheckedContent;
                }
            });

            toggleButton.Unchecked += Change;
            toggleButton.Checked += Change;

            void Change(object sender, RoutedEventArgs e)
            {
                if (checkedContent == null)
                    checkedContent = toggleButton.Content;
                toggleButton.Content = toggleButton.IsChecked == true ? checkedContent : uncheckedContent;
            }
        }
    }
}
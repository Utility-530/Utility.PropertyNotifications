using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors
{
    public class SelectorSelectedItemBehavior : Behavior<Selector>
    {
        #region SelectedItem Property

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
              nameof(SelectedItem),
              typeof(object),
              typeof(SelectorSelectedItemBehavior),
              new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender,
          DependencyPropertyChangedEventArgs e)
        {
            if (sender is SelectorSelectedItemBehavior { AssociatedObject: { } obj } b)
            {
                obj.SelectedItem = obj.ItemsSource.Cast<object>().First(a => a.Equals(e.NewValue));
            }
        }

        #endregion SelectedItem Property

        protected override void OnAttached()
        {
            base.OnAttached();
            if (SelectedItem is { } item)
                AssociatedObject.SelectedItem = AssociatedObject.ItemsSource.Cast<object>().First(a => a == item || a.Equals(item));

            AssociatedObject.SelectionChanged += OnSelectorSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
                AssociatedObject.SelectionChanged -= OnSelectorSelectedItemChanged;
        }

        private void OnSelectorSelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItem = AssociatedObject.SelectedItem;
        }
    }
}
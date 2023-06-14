using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Behavior
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>
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
              typeof(BindableSelectedItemBehavior),
              new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender,
          DependencyPropertyChangedEventArgs e)
        {
            var behavior = (BindableSelectedItemBehavior)sender;
            var generator = behavior.AssociatedObject.ItemContainerGenerator;
            if (generator.ContainerFromItem(e.NewValue) is TreeViewItem item)
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
        }

        #endregion SelectedItem Property

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        private void OnTreeViewSelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e) =>
          SelectedItem = e.NewValue;
    }
}
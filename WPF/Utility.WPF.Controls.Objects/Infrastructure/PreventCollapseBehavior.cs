using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Controls.Objects
{
    public class PreventCollapseBehavior : Behavior<TreeViewItem>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Collapsed += AssociatedObject_Collapsed;

            MethodInfo methodInfo = typeof(TreeViewItem).GetMethod("GetTemplateChild", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            _ = AssociatedObject.Dispatcher.BeginInvoke(() =>
            {
                if (methodInfo.Invoke(AssociatedObject, new string[] { "Expander" }) is ToggleButton expander)
                    expander.Visibility = System.Windows.Visibility.Collapsed;
            }, System.Windows.Threading.DispatcherPriority.Background);

            base.OnAttached();
        }

        private void AssociatedObject_Collapsed(object sender, System.Windows.RoutedEventArgs e)
        {
            AssociatedObject.IsExpanded = true;
            e.Handled = true;
        }
    }
}
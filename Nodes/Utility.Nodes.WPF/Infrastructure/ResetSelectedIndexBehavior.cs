using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.Nodes.WPF
{
    public class ResetSelectedIndexBehavior : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedIndex == -1)
            {
                AssociatedObject.SelectedIndex = 0;
            }
        }
    }

}

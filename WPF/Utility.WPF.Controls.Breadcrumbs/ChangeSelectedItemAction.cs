using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Controls.Breadcrumbs
{
    internal class ChangeSelectedItemAction : TargetedTriggerAction<object>
    {
        protected override void Invoke(object parameter)
        {
            TreeViewEx.SetCurrentItem(this.Target as ItemsControl, null);
        }
    }
}
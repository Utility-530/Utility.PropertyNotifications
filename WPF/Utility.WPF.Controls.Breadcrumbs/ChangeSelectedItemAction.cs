using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

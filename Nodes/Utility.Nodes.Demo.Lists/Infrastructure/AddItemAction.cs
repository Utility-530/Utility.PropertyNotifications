using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.WPF;
using Utility.WPF.Controls.Lists;

namespace Utility.Nodes.Demo.Lists
{
    internal class AddItemAction : TriggerAction<CustomSelector>
    {
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject.ItemsSource is IList list)
            {
                if (parameter is EditRoutedEventArgs { IsAccepted: true, Edit: { } edit })
                {
                    list.Add(edit);
                }
            }
        }
    }
}

using Microsoft.Xaml.Behaviors;
using System.Collections;
using Utility.WPF;
using Utility.WPF.Controls.Lists;

namespace Utility.Nodes.WPF
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

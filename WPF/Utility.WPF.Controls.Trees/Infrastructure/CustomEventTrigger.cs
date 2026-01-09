using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Trees;

namespace Utility.WPF.Controls.Trees.Infrastructure
{
    public class CustomEventTrigger : Microsoft.Xaml.Behaviors.EventTrigger
    {
        public bool IsHandled { get; set; } = true;

        protected override void OnEvent(EventArgs eventArgs)
        {
            var routedEventArgs = eventArgs as RoutedEventArgs;
            if (routedEventArgs != null)
                routedEventArgs.Handled = IsHandled;

            base.OnEvent(eventArgs);
        }
    }

    public class AddObjectAction : Microsoft.Xaml.Behaviors.TriggerAction<FrameworkElement>
    {

        protected override void Invoke(object parameter)
        {
            if (parameter is Utility.WPF.EditRoutedEventArgs { IsAccepted: true, Edit: { } instance } value)
            {
                value.Handled = true;
                var x = AssociatedObject;
                ;
                if (x.DataContext is Tree tree)
                {
                    tree.Add(instance);
                }
            }
        }

    }
}

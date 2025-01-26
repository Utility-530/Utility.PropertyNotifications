using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.Collections;
using Utility.Descriptors;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.Trees;

namespace Utility.Trees.Demo.Filters
{
    public class NewObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ICollectionDescriptor collectionDescriptor)
            {
                var instance = Activator.CreateInstance(collectionDescriptor.ElementType);
                return instance;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AddObjectAction : TriggerAction<FrameworkElement>
    {

        protected override void Invoke(object parameter)
        {
            if(parameter is NewObjectRoutedEventArgs { IsAccepted:true, NewObject:{ } instance } value)
            {
                var x = AssociatedObject;
                ;
                if(x.DataContext is IReadOnlyTree{ Data: ICollectionDescriptor descriptor })
                {
                    (descriptor.Collection as IList).Add(instance);
                    descriptor.Refresh();
                }

            }
        }
    }

    public class CustomEventTrigger : Microsoft.Xaml.Behaviors.EventTrigger
    {

        public bool IsHandled { get; set; } = true;

        protected override void OnEvent(System.EventArgs eventArgs)
        {
            var routedEventArgs = eventArgs as RoutedEventArgs;
            if (routedEventArgs != null)
                routedEventArgs.Handled = IsHandled;

            base.OnEvent(eventArgs);
        }
    }
}

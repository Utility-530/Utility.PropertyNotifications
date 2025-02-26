using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.WPF
{
    public class NewObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ICollectionDescriptor collectionDescriptor)
            {
                var instance = ActivateAnything.Activate.New(collectionDescriptor.ElementType);
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
            if(parameter is Utility.WPF.NewObjectRoutedEventArgs { IsAccepted:true, NewObject:{ } instance } value)
            {
                var x = AssociatedObject;
                ;
                if(x.DataContext is IReadOnlyTree{ Data: ICollectionDescriptor descriptor })
                {
                    if (descriptor.Collection is IList list)
                    {
                        list?.Add(instance);
                        descriptor.OnNext(new System.ComponentModel.RefreshEventArgs(instance));
                    }
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

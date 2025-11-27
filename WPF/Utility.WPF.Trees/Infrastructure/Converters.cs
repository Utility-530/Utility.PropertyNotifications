using System.Collections;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using NodaTime;
using Utility;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
{
    //public class EditConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if(value is ICollectionDescriptor collectionDescriptor)
    //        {
    //            var instance = ActivateAnything.Activate.New(collectionDescriptor.ElementType);
    //            return instance;
    //        }
    //        return DependencyProperty.UnsetValue;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class AddObjectAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is EditRoutedEventArgs { IsAccepted: true, Edit: { } instance } value)
            {
                var x = AssociatedObject;
                ;
                if (x.DataContext is IChildren descriptor)
                {
                    if (descriptor.Children is IList list)
                    {
                        if (instance is ISetKey setKey)
                            setKey.Key = (GuidKey)Guid.NewGuid();
                        list?.Add(instance);
                        //descriptor.OnNext(new System.ComponentModel.RefreshEventArgs(instance));
                    }
                }
            }
        }
    }

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
}
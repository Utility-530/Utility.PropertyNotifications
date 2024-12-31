
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors
{



    /// <summary>
    /// Sets the designated property to the supplied value. TargetObject
    /// optionally designates the object on which to set the property. If
    /// TargetObject is not supplied then the property is set on the object
    /// to which the trigger is attached.
    /// </summary>
    //public abstract class SetPropertyAction<T> : TriggerAction<T> where T:DependencyObject
    //{
    //    /// <summary>
    //    /// name of property to be set
    //    /// </summary>
    //    public string PropertyName
    //    {
    //        get { return (string)GetValue(PropertyNameProperty); }
    //        set { SetValue(PropertyNameProperty, value); }
    //    }

    //    public static readonly DependencyProperty PropertyNameProperty
    //        = DependencyProperty.Register("PropertyName", typeof(string),
    //        typeof(SetPropertyAction<T>));


    //    /// <summary>
    //    /// value to set the property to
    //    /// </summary>
    //    public object PropertyValue
    //    {
    //        get { return GetValue(PropertyValueProperty); }
    //        set { SetValue(PropertyValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty PropertyValueProperty
    //        = DependencyProperty.Register("PropertyValue", typeof(object),
    //        typeof(SetPropertyAction<T>));

    //    /// <summary>
    //    /// target object
    //    /// </summary>
    //    public object TargetObject
    //    {
    //        get { return GetValue(TargetObjectProperty); }
    //        set { SetValue(TargetObjectProperty, value); }
    //    }

    //    public static readonly DependencyProperty TargetObjectProperty
    //        = DependencyProperty.Register("TargetObject", typeof(object),
    //        typeof(SetPropertyAction<T>));


    //}

    public class SetSelectedItemAction : SetterAction
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is not RoutedPropertyChangedEventArgs<object> { NewValue: { } value })
            {
                return;
            }
            object target = TargetObject ?? AssociatedObject;
            PropertyInfo propertyInfo = target.GetType().GetProperty(
                PropertyName,
                BindingFlags.Instance | BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            propertyInfo.SetValue(target, value);
        }
    }
}
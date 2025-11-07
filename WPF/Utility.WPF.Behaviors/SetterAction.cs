namespace Utility.WPF.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    //https://stackoverflow.com/questions/942548/setting-a-property-with-an-eventtrigger
    // Neutrino
    //FocusedWolf
    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// Sets a specified property to a value when invoked.
    /// </summary>
    public class SetterAction : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(SetterAction),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SetterAction),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(SetterAction), new PropertyMetadata());

        #region properties

        /// <summary>
        /// Property that is being set by this setter.
        /// </summary>
        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        /// Property value that is being set by this setter.
        /// </summary>
        public object? Value
        {
            get => (object)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        #endregion properties

        protected override void Invoke(object parameter)
        {
            var target = TargetObject ?? AssociatedObject;

            var targetType = target.GetType();

            var property = targetType.GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property == null)
                throw new ArgumentException($"Property not found: {PropertyName}");

            if (property.CanWrite == false)
                throw new ArgumentException($"Property is not settable: {PropertyName}");

            object? convertedValue;

            if (Converter != null)
            {
                convertedValue = Converter.Convert(parameter, default, default, default);
            }
            else if (Value == null)
                convertedValue = null;
            else
            {
                var valueType = Value.GetType();
                var propertyType = property.PropertyType;

                if (valueType == propertyType)
                    convertedValue = Value;
                else
                {
                    var propertyConverter = TypeDescriptor.GetConverter(propertyType);

                    if (propertyConverter.CanConvertFrom(valueType))
                        convertedValue = propertyConverter.ConvertFrom(Value);
                    else if (valueType.IsSubclassOf(propertyType))
                        convertedValue = Value;
                    else
                        throw new ArgumentException($"Cannot convert type '{valueType}' to '{propertyType}'.");
                }
            }

            if (convertedValue == DependencyProperty.UnsetValue)
                return;
            property.SetValue(target, convertedValue);
        }
    }
}
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Controls.Lists
{
    public class ItemsSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.SingleOrDefault(a => a is not IEnumerable and not null) is { } x)
                return x;

            if (values.SingleOrDefault(a => a is IEnumerable) is IEnumerable enumerable)
            {
                if (enumerable.Count() > 0)
                {
                    return create(enumerable.CommonBaseClass());///.InnerType();
                }
                else if (convert(enumerable.GetType()) is { } _object)
                {
                    return _object;
                }
                else if (enumerable is ICollectionView { SourceCollection: { } sourceCollection })
                {
                    return convert(sourceCollection.GetType());
                }
                else
                {
                    throw new NotImplementedException("dv33 322lk2");
                }
            }
            return DependencyProperty.UnsetValue;

            object convert(Type type)
            {
                if (TypeHelper.InnerType(type) is Type _type)
                {
                    return create(_type);
                }
                return null;
            }
        }

        private static object create(Type _type)
        {
            return ActivateAnything.Activate.New(_type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullTo40Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return new Thickness(0);
            }
            return new Thickness(0, 0, 40, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class WidthConverter : IMultiValueConverter
    {
        //public object[] Convert(object value, Type[] targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is ListBox frameworkElement)
        //    {
        //        var container = (ListBoxItem)frameworkElement.ItemContainerGenerator.ContainerFromItem(frameworkElement.Items.First());
        //        return container.ActualWidth;// frameworkElement.ActualWidth;
        //    }
        //    return  40;
        //}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            return values[0];// 500.0;
        }



        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}


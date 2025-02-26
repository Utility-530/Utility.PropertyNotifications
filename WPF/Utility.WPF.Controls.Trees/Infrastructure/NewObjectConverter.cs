using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers;

namespace Utility.WPF.Controls.Trees
{
    //public class MultiNewObjectConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        IEnumerable enumerable = null;
    //        IValueConverter valueConverter = null;
    //        if(values.SingleOrDefault(a=>a is IEnumerable) is IEnumerable a)
    //        {

    //        }
    //        if (values.SingleOrDefault(a => a is IEnumerable) is IEnumerable a)
    //        {

    //        }
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class NewObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable enumerable)
            {
                var type = value.GetType();
                Type elementType = null;
                if (type.GenericTypeArguments().Length == 1)
                {
                    elementType = type.InnerType();
                    if(elementType==typeof(object))
                    {
                        elementType = TypeHelper.CommonBaseClass(enumerable);
                    }
                }
                else if (type.GenericTypeArguments().Length == 0)
                {
                    elementType = TypeHelper.CommonBaseClass(enumerable);
                }
                else
                {
                    throw new Exception("rg r433333o");
                }
                return ActivateAnything.Activate.New(elementType);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

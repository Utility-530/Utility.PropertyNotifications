using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.PatternMatchings;
using Utility.Trees;

namespace Utility.Nodify.Views.Infrastructure
{
    public class NodeEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelectionChangedEventArgs { AddedItems: { } x } args)
            {
                foreach(var item in x)
                {
                    if(item is Result { Symbol:{ Item: PropertyInfo propertyInfo } })
                    {
                        return propertyInfo;
                    }
                }
            
            }
            throw new NotImplementedException("SD");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CheckedNodeEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WPF.Controls.ComboBoxes.ComboBoxTreeView.CheckedItemsEventArgs { CheckedItems: var x, UnCheckedItems: var y } args)
            {
                List<object> list = new();
                if (x != null)
                    foreach (var item in x)
                    {
                        if (item is IGetData { Data: { } data })
                            list.Add(data);
                    }

                List<object> list2 = new();
                if (y != null)
                    foreach (var item in y)
                    {
                        if (item is IGetData { Data: { } data })
                            list2.Add(data);
                    }
                return (list, list2);
            }
            throw new NotImplementedException("SD");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

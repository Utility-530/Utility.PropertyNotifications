using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls
{
    public class BreadCrumbs : CustomItemsControl
    {

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BreadCrumbs), new PropertyMetadata());

        static BreadCrumbs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbs), new FrameworkPropertyMetadata(typeof(BreadCrumbs)));
        }



        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }



    }

    public class LastItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ItemsControl itemscontrol && (values[1] is FrameworkElement fe))
            {
                int count = itemscontrol.Items.Count;

                if (values != null && values.Length >= 3 && count > 0)
                {
                    var lastItem = itemscontrol.Items[count - 1];
                    if (Equals(values[2], false) && Equals(lastItem, fe.DataContext) == false)
                    {
                        return true;
                    }
                    return false;

                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

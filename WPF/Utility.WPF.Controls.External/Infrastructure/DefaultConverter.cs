using Custom.Controls;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Utility.WPF.Controls.External
{
    public class DefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ChangeKeyboardFocus : TargetedTriggerAction<object>
    {
        protected override void Invoke(object parameter)
        {
            if(TargetObject is EditableTextBlock e)
            {
                e.Text = e.OldText;
                e.IsInEditMode = false;
            }
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using Custom.Controls;
using Microsoft.Xaml.Behaviors;

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
            if (TargetObject is EditableTextBlock e)
            {
                e.Text = e.OldText;
                e.IsInEditMode = false;
            }
        }
    }
}
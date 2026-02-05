using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.ComboBoxes
{
    public class ComboBoxEx
    {
        public static readonly DependencyProperty ToggleButtonStyleProperty =
    DependencyProperty.RegisterAttached(
        "ToggleButtonStyle",
        typeof(Style),
        typeof(ComboBoxEx),
        new PropertyMetadata());


        public static Style GetToggleButtonStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(ToggleButtonStyleProperty);
        }

        private static void SetToggleButtonStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ToggleButtonStyleProperty, value);
        }
      

    }
}

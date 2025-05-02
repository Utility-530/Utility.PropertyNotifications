using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Leepfrog.WpfFramework.Behaviors
{
    public static class ButtonOptions
    {
        public static readonly DependencyProperty FixedCornerRadiusProperty =
            DependencyProperty.RegisterAttached(
            "FixedCornerRadius",
            typeof(double),
            typeof(ButtonOptions));

        public static double GetFixedCornerRadius(DependencyObject d)
        {
            return (double)d.GetValue(FixedCornerRadiusProperty);
        }

        public static void SetFixedCornerRadius(DependencyObject d, double value)
        {
            d.SetValue(FixedCornerRadiusProperty, value);
        }
    }
}

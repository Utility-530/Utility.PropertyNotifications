using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class PointControl : Control
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(PointControl), new PropertyMetadata(0d));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(PointControl), new PropertyMetadata(0d));

        static PointControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointControl), new FrameworkPropertyMetadata(typeof(PointControl)));
        }

        public double X
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
    }
}
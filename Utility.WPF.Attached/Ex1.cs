using System.Windows;

namespace Utility.WPF.Attached
{
    public partial class Ex
    {
        public static GridLength GetIndent(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(IndentProperty);
        }

        public static void SetIndent(DependencyObject obj, GridLength value)
        {
            obj.SetValue(IndentProperty, value);
        }

        // Using a DependencyProperty as the backing store for Indent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.RegisterAttached("Indent", typeof(GridLength), typeof(Ex), new PropertyMetadata(new GridLength(0)));
    }
}
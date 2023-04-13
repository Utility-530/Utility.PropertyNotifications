using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls
{
    public class ValidationForm : ContentControl
    {
        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool), typeof(ValidationForm), new PropertyMetadata(false));

        static ValidationForm()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationForm), new FrameworkPropertyMetadata(typeof(ValidationForm)));
        }

        public ValidationForm()
        {
            Content = new Rectangle { Height = 300, Width = 300, Fill = Brushes.Gainsboro }; ;
        }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
    }
}
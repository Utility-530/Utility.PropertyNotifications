using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class IconContentControl : ContentControl
    {
        public static readonly DependencyProperty KindProperty = DependencyProperty.Register("Kind", typeof(PackIconKind), typeof(IconContentControl));

        static IconContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconContentControl), new FrameworkPropertyMetadata(typeof(IconContentControl)));
        }

        public PackIconKind Kind
        {
            get => (PackIconKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }
    }
}
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.FileSystem
{
    public class PathControl : Control
    {
        public static readonly DependencyProperty PathNameProperty = DependencyProperty.Register("PathName", typeof(string), typeof(PathControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(PackIconKind), typeof(PathControl), new PropertyMetadata(PackIconKind.ArrowLeft));

        static PathControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathControl), new FrameworkPropertyMetadata(typeof(PathControl)));
        }

        public PackIconKind Icon
        {
            get { return (PackIconKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string PathName
        {
            get { return (string)GetValue(PathNameProperty); }
            set { SetValue(PathNameProperty, value); }
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Controls.Buttons
{
    public class GeometryButton : Button
    {
        public const string InitialData = "M 34,40 L0,20 L0,60 Z";

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(GeometryButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));

        static GeometryButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(typeof(GeometryButton)));
        }

        public GeometryButton()
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Geometry));
            Data = (Geometry)(converter.ConvertFrom(InitialData) ?? new EllipseGeometry(new Rect(new Size(30, 30))));
        }

        #region properties

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public Brush HoverBackground
        {
            get => (Brush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }

        #endregion properties
    }
}
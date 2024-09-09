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
        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Transparent));

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

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }




        #endregion properties
    }


    public class GeometryToggleButton : ToggleButton
    {
        public const string InitialData = "M 34,40 L0,20 L0,60 Z";

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(GeometryToggleButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Color), typeof(GeometryToggleButton), new PropertyMetadata(Color.FromArgb(255, 255, 139, 0)));
        public static readonly DependencyProperty CheckedBackgroundProperty =  DependencyProperty.Register("CheckedBackground", typeof(Color), typeof(GeometryToggleButton), new PropertyMetadata(Color.FromArgb(55, 55, 139, 0)));
        public static readonly DependencyProperty FillProperty =
    DependencyProperty.Register("Fill", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Transparent));

        static GeometryToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryToggleButton), new FrameworkPropertyMetadata(typeof(GeometryToggleButton)));
        }

        public GeometryToggleButton()
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

        public Color HoverBackground
        {
            get => (Color)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }


        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Color CheckedBackground
        {
            get { return (Color)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        #endregion properties
    }
}
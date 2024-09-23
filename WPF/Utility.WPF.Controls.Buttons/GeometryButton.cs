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
        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Silver));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Transparent));

        static GeometryButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(typeof(GeometryButton)));

        }

        public GeometryButton()
        { 
        }
         public override void OnApplyTemplate()
        {
            Data ??= Geometry.Parse(InitialData) ?? new EllipseGeometry(new Rect(new Size(30, 30)));
            base.OnApplyTemplate();
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

        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public Brush PressedForeground
        {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
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

        static GeometryToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryToggleButton), new FrameworkPropertyMetadata(typeof(GeometryToggleButton)));
        }

        public GeometryToggleButton()
        {
        }

        public override void OnApplyTemplate()
        {
            Data ??= Geometry.Parse(InitialData) ?? new EllipseGeometry(new Rect(new Size(30, 30)));
            base.OnApplyTemplate();
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



        public Color CheckedBackground
        {
            get { return (Color)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        #endregion properties
    }
}
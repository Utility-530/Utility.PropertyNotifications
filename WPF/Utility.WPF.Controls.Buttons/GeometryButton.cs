using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.Buttons
{
    public class GeometryButton : Button
    {
        public const string InitialData = "M 34,40 L0,20 L0,60 Z";

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(GeometryButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundBrushProperty = DependencyProperty.Register("HoverBackgroundBrush", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));
        public static readonly DependencyProperty PressedForegroundBrushProperty = DependencyProperty.Register("PressedForegroundBrush", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Silver));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(GeometryButton), new PropertyMetadata(12.0));
        public static readonly DependencyProperty HoverForegroundBrushProperty = DependencyProperty.Register("HoverForegroundBrush", typeof(Brush), typeof(GeometryButton), new PropertyMetadata(Brushes.Green));

        static GeometryButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(typeof(GeometryButton)));
            BackgroundProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(Brushes.Transparent));
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

        public Brush HoverBackgroundBrush
        {
            get => (Brush)GetValue(HoverBackgroundBrushProperty);
            set => SetValue(HoverBackgroundBrushProperty, value);
        }

        public Brush HoverForegroundBrush
        {
            get { return (Brush)GetValue(HoverForegroundBrushProperty); }
            set { SetValue(HoverForegroundBrushProperty, value); }
        }

        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        public Brush PressedBackgroundBrush
        {
            get { return (Brush)GetValue(PressedBackgroundBrushProperty); }
            set { SetValue(PressedBackgroundBrushProperty, value); }
        }

        public Brush PressedForegroundBrush
        {
            get { return (Brush)GetValue(PressedForegroundBrushProperty); }
            set { SetValue(PressedForegroundBrushProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        #endregion properties
    }

    public class GeometryToggleButton : ToggleButton
    {
        public const string InitialData = "M 34,40 L0,20 L0,60 Z";

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(GeometryToggleButton), new PropertyMetadata());
        public static readonly DependencyProperty HoverBackgroundBrushProperty = DependencyProperty.Register("HoverBackgroundBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 139, 0))));
        public static readonly DependencyProperty CheckedBackgroundBrushProperty = DependencyProperty.Register("CheckedBackgroundBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(55, 55, 139, 0))));
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(GeometryToggleButton), new PropertyMetadata(4.0));
        public static readonly DependencyProperty FillProperty = Shape.FillProperty.AddOwner(typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty PressedForegroundBrushProperty = DependencyProperty.Register("PressedForegroundBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.Register("PressedBackgroundBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Silver));
        public static readonly DependencyProperty HoverForegroundBrushProperty = DependencyProperty.Register("HoverForegroundBrush", typeof(Brush), typeof(GeometryToggleButton), new PropertyMetadata(Brushes.Green));

        static GeometryToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryToggleButton), new FrameworkPropertyMetadata(typeof(GeometryToggleButton)));
            BackgroundProperty.OverrideMetadata(typeof(GeometryToggleButton), new FrameworkPropertyMetadata(Brushes.Transparent));
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

        public Brush HoverBackgroundBrush
        {
            get { return (Brush)GetValue(HoverBackgroundBrushProperty); }
            set { SetValue(HoverBackgroundBrushProperty, value); }
        }

        public Brush HoverForegroundBrush
        {
            get { return (Brush)GetValue(HoverForegroundBrushProperty); }
            set { SetValue(HoverForegroundBrushProperty, value); }
        }

        public Brush CheckedBackgroundBrush
        {
            get { return (Brush)GetValue(CheckedBackgroundBrushProperty); }
            set { SetValue(CheckedBackgroundBrushProperty, value); }
        }

        public Brush PressedBorderBrush
        {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
        }

        public Brush PressedBackgroundBrush
        {
            get { return (Brush)GetValue(PressedBackgroundBrushProperty); }
            set { SetValue(PressedBackgroundBrushProperty, value); }
        }

        public Brush PressedForegroundBrush
        {
            get { return (Brush)GetValue(PressedForegroundBrushProperty); }
            set { SetValue(PressedForegroundBrushProperty, value); }
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        #endregion properties
    }
}
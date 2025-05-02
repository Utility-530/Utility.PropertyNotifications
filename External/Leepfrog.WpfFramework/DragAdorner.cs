using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leepfrog.WpfFramework
{
    public class DragAdorner : Adorner
    {
        private UIElement _owner;
        private Rect _rect;
        private Point _offset;
        private Brush _brush;
        private double _opacity;

        public DragAdorner(UIElement owner, FrameworkElement adornElement, Point offset, double opacity)
            : base(owner)
        {
            _owner = owner;
            _offset = offset;
            _offset.X += adornElement.Margin.Left;
            _offset.Y += adornElement.Margin.Top;
            _opacity = opacity;
            _rect.Location = new Point(0, 0);
            _rect.Width = adornElement.ActualWidth;
            _rect.Height = adornElement.ActualHeight;
            if (adornElement.Effect as DropShadowEffect != null)
            {
                var shadowSize = ((adornElement.Effect as DropShadowEffect).BlurRadius) * 2;
                _rect.Width += shadowSize;
                _rect.Height += shadowSize;
            }
            RenderTargetBitmap rtb = renderBitmap(adornElement);
            _brush = new ImageBrush(BitmapFrame.Create(rtb));
        }

        private RenderTargetBitmap renderBitmap(FrameworkElement element) 
        { 
           double left = 0; 
           double top = 0; 
           int width = (int)_rect.Width;
           int height = (int)_rect.Height;
           double dpiX = 96; // this is the magic number 
           double dpiY = 96; // this is the magic number 
           PixelFormat pixelFormat = PixelFormats.Default; 
           VisualBrush elementBrush = new VisualBrush(element);
           elementBrush.Opacity = _opacity;
           elementBrush.TileMode = TileMode.None;
           elementBrush.Stretch = Stretch.None;
           elementBrush.AlignmentX = AlignmentX.Left;
           elementBrush.AlignmentY = AlignmentY.Top;
           DrawingVisual visual = new DrawingVisual(); 
           DrawingContext dc = visual.RenderOpen(); 
           dc.DrawRectangle(elementBrush, null, new Rect(left, top, width, height)); 
           dc.Close(); 
           RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, dpiX, dpiY, pixelFormat); 
           bitmap.Render(visual); 
           return bitmap; 
        }



        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(DragAdorner), new PropertyMetadata(new Point(),_point_Changed));

        private static void _point_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DragAdorner).positionUpdated();
        }

        private void positionUpdated()
        {
            _rect.Location = Position;
            InvalidateVisual();
        }

        public Point Offset
        {
            get
            {
                return _offset;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var rect = _rect;
            rect.Offset(-_offset.X, -_offset.Y);
            drawingContext.DrawRectangle(_brush,null,rect);
        }

    }
}

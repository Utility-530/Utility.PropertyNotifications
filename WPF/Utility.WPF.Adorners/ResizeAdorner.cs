using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Adorners
{
    /// <summary>
    /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/14a41db4-c08c-43e5-bd09-caf45fbeb038/textbox-adorner-similar-to-ms-paint-application-in-wpf-c?forum=wpf"></a>
    /// </summary>
    public class ResizeAdorner : Adorner
    {
        private const double THUMB_SIZE = 10;
        private const double MINIMAL_SIZE = 20;
        private const double MOVE_OFFSET = 20;

        //9 thumbs
        /*                        moveAndRotateThumb
         *                              *
         *                              *
         * topLeftThumb*************topMiddleThumb**************topRightThumb
         *      *                                                    *
         *      *                                                    *
         *      *                                                    *
         * middleLeftThumb                                     middleRightThumb
         *      *                                                    *
         *      *                                                    *
         *      *                                                    *
         * bottomLeftThumb*********bottomMiddleThumb**************bottomRightThumb
         *
         * */
        private Thumb moveAndRotateThumb, topLeftThumb, middleLeftThumb, bottomLeftThumb, topMiddleThumb, topRightThumb, middleRightThumb, bottomRightThumb, bottomMiddleThumb;

        private Rectangle thumbRectangle;

        private VisualCollection visualCollection;

        public ResizeAdorner(UIElement adorned) : base(adorned)
        {
            visualCollection = new VisualCollection(this)
            {
                (thumbRectangle = GetResizeRectangle(adorned)),

                (topLeftThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top)),
                (middleLeftThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center)),
                (bottomLeftThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom)),

                (topRightThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top)),
                (middleRightThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center)),
                (bottomRightThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom)),

                (topMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top)),
                (bottomMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom)),

                (moveAndRotateThumb = GetMoveAndRotateThumb(adorned))
            };

            static Thumb GetMoveAndRotateThumb(UIElement adornedElement)
            {
                var thumb = new Thumb()
                {
                    Width = THUMB_SIZE,
                    Height = THUMB_SIZE,
                    Cursor = Cursors.Hand,
                    Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = GetThumbTemplate(GetMoveEllipseBack())
                    }
                };
                thumb.DragDelta += (s, e) =>
                {
                    Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + e.HorizontalChange);
                    Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + e.VerticalChange);
                };
                return thumb;

                static Brush GetMoveEllipseBack()
                {
                    const string lan = "M841.142857 570.514286c0 168.228571-153.6 336.457143-329.142857 336.457143s-329.142857-153.6-329.142857-336.457143c0-182.857143 153.6-336.457143 329.142857-336.457143v117.028571l277.942857-168.228571L512 0v117.028571c-241.371429 0-438.857143 197.485714-438.857143 453.485715S270.628571 1024 512 1024s438.857143-168.228571 438.857143-453.485714h-109.714286z m0 0";
                    var geometry = (Geometry?)TypeDescriptor.GetConverter(typeof(Geometry)).ConvertFrom(lan);
                    TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry))
                    {
                        Stretch = Stretch.Fill
                    };
                    return bsh;
                }
            }

            static Rectangle GetResizeRectangle(UIElement adornedElement) => new Rectangle()
            {
                Width = adornedElement.RenderSize.Width,
                Height = adornedElement.RenderSize.Height,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };
        }

        private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            Thumb thumb = GetThumb(cur, horizontal, vertical);

            thumb.DragDelta += (s, e) =>
            {
                if (!(AdornedElement is FrameworkElement element))
                    return;

                ElementResize(element);

                switch (thumb.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom when element.Height + e.VerticalChange > MINIMAL_SIZE:
                        element.Height += e.VerticalChange;
                        thumbRectangle.Height += e.VerticalChange;
                        break;
                    //case VerticalAlignment.Center:
                    //    if ()
                    //    {
                    //    }
                    //    break;
                    case VerticalAlignment.Top when element.Height - e.VerticalChange > MINIMAL_SIZE:
                        element.Height -= e.VerticalChange;
                        thumbRectangle.Height -= e.VerticalChange;
                        Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
                        break;
                }
                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left when element.Width - e.HorizontalChange > MINIMAL_SIZE:
                        element.Width -= e.HorizontalChange;
                        thumbRectangle.Width -= e.HorizontalChange;
                        Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                        break;
                    //case HorizontalAlignment.Center:
                    //    if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
                    //    {
                    //        element.Width += e.HorizontalChange;
                    //    }
                    //    break;
                    case HorizontalAlignment.Right when element.Width + e.HorizontalChange > MINIMAL_SIZE:
                        element.Width += e.HorizontalChange;
                        thumbRectangle.Width += e.HorizontalChange;
                        break;
                }

                e.Handled = true;
            };
            return thumb;

            static Thumb GetThumb(Cursor cur, HorizontalAlignment horizontal, VerticalAlignment vertical) => new Thumb
            {
                //Background = Brushes.Red,
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Cursor = cur,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetThumbTemplate(new SolidColorBrush(Colors.White))
                }
            };

            static void ElementResize(FrameworkElement frameworkElement)
            {
                if (double.IsNaN(frameworkElement.Width))
                    frameworkElement.Width = frameworkElement.RenderSize.Width;
                if (double.IsNaN(frameworkElement.Height))
                    frameworkElement.Height = frameworkElement.RenderSize.Height;
            }

            // get Thumb Temple
        }

        private static FrameworkElementFactory GetThumbTemplate(Brush back)
        {
            back.Opacity = 1;
            var fef = new FrameworkElementFactory(typeof(Ellipse));
            fef.SetValue(Shape.FillProperty, back);
            fef.SetValue(Shape.StrokeProperty, Brushes.Green);
            fef.SetValue(Shape.StrokeThicknessProperty, (double)1);
            return fef;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = THUMB_SIZE / 2;
            Size sz = new Size(THUMB_SIZE, THUMB_SIZE);

            topLeftThumb.Arrange(new Rect(new Point(-offset, -offset), sz));
            topMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -offset), sz));
            topRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));

            bottomLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
            bottomMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, AdornedElement.RenderSize.Height - offset), sz));
            bottomRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));

            middleLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));
            middleRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));

            moveAndRotateThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -MOVE_OFFSET), sz));

            thumbRectangle.Arrange(new Rect(new Point(-offset, -offset), new Size(Width = AdornedElement.RenderSize.Width + THUMB_SIZE, Height = AdornedElement.RenderSize.Height + THUMB_SIZE)));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => visualCollection[index];

        protected override int VisualChildrenCount => visualCollection.Count;
    }
}
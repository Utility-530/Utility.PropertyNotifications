using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.WPF.Helpers;

namespace Utility.WPF.Adorners;

public class EllipseAdorner : Adorner
{
    private ICommand? command;

    public EllipseAdorner(UIElement adornedElement, ICommand? command = null) : base(adornedElement)
    {
        MouseDown += EllipseAdorner_MouseDown;
        this.command = command;
    }

    private void EllipseAdorner_MouseDown(object sender, MouseButtonEventArgs e)
    {
        command?.Execute(this);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        SolidColorBrush renderBrush = new SolidColorBrush(Colors.LightCoral);

        Pen renderPen = new Pen(new SolidColorBrush(Colors.DarkBlue), 1.0);

        if (AdornedElement is Path path)
        {
            foreach (var v in path.RenderedGeometry.Point())
            {
                //double fraction = 0.5;  //the relative point of the curve
                //Point pt;               //the absolute point of the curve
                //Point tg;               //the tangent point of the curve
                //(path.RenderedGeometry as PathGeometry).GetPointAtFractionLength(
                //    fraction,
                //    out pt,
                //    out tg);
                //drawingContext.DrawEllipse(renderBrush, renderPen, new Point(pt.X, pt.Y), 10, 10);

                drawingContext.DrawEllipse(renderBrush, renderPen, new Point(v.X, v.Y), 10, 10);
            }
        }
        else if (AdornedElement is Control control)
        {
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(0, 0), 10, 10);
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(control.Width, control.Height), 10, 10);
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(0, control.Height), 10, 10);
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(control.Width, 0), 10, 10);
        }
    }
}

public class PlusAdorner : Adorner
{
    private ICommand? command;

    public PlusAdorner(UIElement adornedElement, ICommand? command = null) : base(adornedElement)
    {
        MouseDown += EllipseAdorner_MouseDown;
        this.command = command;
    }

    private void EllipseAdorner_MouseDown(object sender, MouseButtonEventArgs e)
    {
        command?.Execute(this);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        SolidColorBrush renderBrush = new SolidColorBrush(Colors.LightCoral);

        Pen renderPen = new Pen(new SolidColorBrush(Colors.DarkBlue), 1.0);

        if (AdornedElement is FrameworkElement control)
        {
            // Draw the ellipse
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(control.Width, control.Height), 10, 10);

            // Draw the plus symbol inside the ellipse
            Point center = new(control.Width, control.Height);
            double plusSize = 6; // Adjust size to fit nicely within the 10-radius ellipse

            // Draw horizontal line of the plus
            drawingContext.DrawLine(renderPen,
                new Point(center.X - plusSize, center.Y),
                new Point(center.X + plusSize, center.Y));

            // Draw vertical line of the plus
            drawingContext.DrawLine(renderPen,
                new Point(center.X, center.Y - plusSize),
                new Point(center.X, center.Y + plusSize));
        }
    }
}
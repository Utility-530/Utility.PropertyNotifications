using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Adorners;

public class CancelAdorner : Adorner
{
    private ICommand? command;

    public CancelAdorner(UIElement adornedElement, ICommand? command = null) : base(adornedElement)
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
        SolidColorBrush renderBrush2 = new SolidColorBrush(Colors.WhiteSmoke) { Opacity = 0.7 };

        if (AdornedElement is Control control)
        {
            var width = control.ActualWidth / 16d;
            var height = control.ActualHeight / 16d;
            var size = width / 3d;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.BlueViolet), width / 6d);

            drawingContext.DrawEllipse(renderBrush2, renderPen, new Point(control.ActualWidth, 0), width, width);
            drawingContext.DrawLine(renderPen, new Point(control.ActualWidth - size, 0 - size), new Point(control.ActualWidth + size, 0 + size));
            drawingContext.DrawLine(renderPen, new Point(control.ActualWidth - size, 0 + size), new Point(control.ActualWidth + size, 0 - size));
        }
    }
}
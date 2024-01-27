using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Adorners;

public class MaskAdorner : Adorner
{
    private ICommand? command;


    public MaskAdorner(UIElement adornedElement, ICommand? command = null) : base(adornedElement)
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
        SolidColorBrush renderBrush = new SolidColorBrush(Colors.WhiteSmoke) { Opacity = 0.7 };

        if (AdornedElement is Control control)
        {
            var width = control.ActualWidth / 16d;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.BlueViolet), width / 6d);
            Rect adornedElementRect = new Rect(new Size(control.ActualWidth, control.ActualHeight));
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

        }
    }


}

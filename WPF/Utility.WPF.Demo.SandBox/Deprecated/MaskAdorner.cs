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

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MaskAdorner), new PropertyMetadata());

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
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            FormattedText ft = new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                control.ActualHeight/2d,    // 36 pt type
                Brushes.DarkGray);

            // Draw the text at a location
            drawingContext.DrawText(ft, new Point(10.0, 10.0));
        }
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
}

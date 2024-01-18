using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VisualJsonEditor.Test
{
    public class ButtonAdornerController : IObservable<object>
    {
        ReplaySubject<object> _replaySubject = new(1);

        private Button button;
        double left_margin = -15;
        private readonly UIElement uiElement;

        public ButtonAdornerController(UIElement uiElement)
        {

            button = new Button { Margin = new Thickness(left_margin, 0, 0, 0), Width = 15, Height = 15, Background = new SolidColorBrush { Color = Color.FromRgb(250, 250, 250) }, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            button.Click += Button_Click;
            //this.SetValue(Gu.Wpf.Adorners.Overlay.ContentProperty, button);


            var adorners = AdornerBehavior.AdornerBehavior.GetAdorners(uiElement);
            adorners.Add(button);

            uiElement.MouseLeave += _MouseLeave;
            button.MouseLeave += Button_MouseLeave;
            uiElement.MouseEnter += _MouseEnter;
            this.uiElement = uiElement;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            AdornerBehavior.AdornerBehavior.SetIsEnabled(uiElement, false);
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            AdornerBehavior.AdornerBehavior.SetIsEnabled(uiElement, true);
            //button.SetValue(Gu.Wpf.Adorners.Overlay.VisibilityProperty, Visibility.Visible);
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            Point? point = e.GetPosition(button);
            if (point.HasValue)
                if (point.Value.X <= button.ActualWidth && point.Value.Y <= button.ActualHeight && point.Value.X >= 0 && point.Value.Y >= 0)
                {
                    // ignore as mouse if still over either the element or over button
                }
                else
                {
                    AdornerBehavior.AdornerBehavior.SetIsEnabled(uiElement, false);
                }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _replaySubject.OnNext(null);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return _replaySubject.Subscribe(observer);
        }
    }
}

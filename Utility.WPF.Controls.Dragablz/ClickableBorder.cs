using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Utility.WPF.Controls.Dragablz
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/45608326/c-sharp-wpf-border-style-animations"></a>
    /// </summary>
    public class ClickableBorder : Border
    {
        public static readonly RoutedEvent ClickEvent;
        //public static readonly DependencyProperty PressedColorProperty = DependencyProperty.Register("PressedColor", typeof(Color), typeof(ClickableBorder), new PropertyMetadata(Colors.Gray));
        //public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register("HoverColor", typeof(Color), typeof(ClickableBorder), new PropertyMetadata(Colors.LightBlue));
        //public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(ClickableBorder), new PropertyMetadata(false));

        static ClickableBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClickableBorder), new FrameworkPropertyMetadata(typeof(ClickableBorder)));
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(ClickableBorder));
        }

        public ClickableBorder()
        {
        }

        //public Color PressedColor
        //{
        //    get { return (Color)GetValue(PressedColorProperty); }
        //    set { SetValue(PressedColorProperty, value); }
        //}

        //public Color HoverColor
        //{
        //    get { return (Color)GetValue(HoverColorProperty); }
        //    set { SetValue(HoverColorProperty, value); }
        //}

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                if (IsMouseOver)
                {
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                }
            }
        }
    }
}
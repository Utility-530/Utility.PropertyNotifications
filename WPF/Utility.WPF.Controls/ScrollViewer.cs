using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls
{
    public enum WheelOrientation
    {
        None = 0,
        Auto = 1,
        Horizontal = 2,
        Vertical = 3,
    }

    public class ScrollViewer : System.Windows.Controls.ScrollViewer
    {
        public static readonly DependencyProperty CanContentScrollUpProperty = DependencyProperty.Register("CanContentScrollUp", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static readonly DependencyProperty CanContentScrollRightProperty = DependencyProperty.Register("CanContentScrollRight", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static readonly DependencyProperty CanContentScrollDownProperty = DependencyProperty.Register("CanContentScrollDown", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static readonly DependencyProperty CanContentScrollLeftProperty = DependencyProperty.Register("CanContentScrollLeft", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));
        public static readonly DependencyProperty WheelOrientationProperty = DependencyProperty.Register("WheelOrientation", typeof(WheelOrientation), typeof(ScrollViewer), new PropertyMetadata(WheelOrientation.Vertical));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(ScrollViewer), new PropertyMetadata(1d));

        private MouseWheelEventArgs _lastMouseWheelEventArgs;

        // Constructor
        public ScrollViewer() : base()
        {
            WheelOrientation = WheelOrientation.Auto;
            AddHandler(MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheelEx), true);
        }

        public bool CanContentScrollUp
        {
            get { return (bool)GetValue(CanContentScrollUpProperty); }
            set { SetValue(CanContentScrollUpProperty, value); }
        }

        public bool CanContentScrollRight
        {
            get { return (bool)GetValue(CanContentScrollRightProperty); }
            set { SetValue(CanContentScrollRightProperty, value); }
        }

        public bool CanContentScrollDown
        {
            get { return (bool)GetValue(CanContentScrollDownProperty); }
            set { SetValue(CanContentScrollDownProperty, value); }
        }

        public bool CanContentScrollLeft
        {
            get { return (bool)GetValue(CanContentScrollLeftProperty); }
            set { SetValue(CanContentScrollLeftProperty, value); }
        }

        public WheelOrientation WheelOrientation
        {
            get { return (WheelOrientation)GetValue(WheelOrientationProperty); }
            set { SetValue(WheelOrientationProperty, value); }
        }

        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            CanContentScrollUp = e.VerticalOffset > 0;
            CanContentScrollDown = e.VerticalOffset < ScrollableHeight;
            CanContentScrollLeft = e.HorizontalOffset > 0;
            CanContentScrollRight = e.HorizontalOffset < ScrollableWidth;
            base.OnScrollChanged(e);
        }

        // Lock standard wheel event
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            e.Handled = true;
            //base.OnMouseWheel(e);
        }

        private void OnMouseWheelEx(object sender, MouseWheelEventArgs e)
        {
            if (e == _lastMouseWheelEventArgs)
                return;

            if ((WheelOrientation == WheelOrientation.Vertical || WheelOrientation == WheelOrientation.Auto)
                && VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                if (ScrollableHeight > 0)
                    ScrollToVerticalOffset(VerticalOffset - e.Delta * Speed / 4);
                _lastMouseWheelEventArgs = e;
                e.Handled = true;
            }
            else if ((WheelOrientation == WheelOrientation.Horizontal || WheelOrientation == WheelOrientation.Auto)
                && HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                if (ScrollableWidth > 0)
                    ScrollToHorizontalOffset(HorizontalOffset - e.Delta * Speed / 4);
                //_inertiaManager.StopInertia();
                _lastMouseWheelEventArgs = e;
                e.Handled = true;
            }
        }
    }
}
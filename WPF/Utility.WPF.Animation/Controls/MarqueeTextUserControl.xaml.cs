using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Utility.Enums;

namespace Utility.WPF.Animations
{
    /// <summary>
    /// Interaction logic for MarqueeTextUserControl.xaml
    /// </summary>
    public partial class MarqueeTextUserControl : UserControl
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(MarqueeTextUserControl), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));
        public static readonly DependencyProperty MovementProperty = DependencyProperty.Register("Movement", typeof(XYTraversal), typeof(MarqueeTextUserControl), new PropertyMetadata(XYTraversal.RightToLeft, Changed));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MarqueeTextUserControl), new PropertyMetadata(Changed2));

        public RepeatBehavior Repeat
        {
            get { return (RepeatBehavior)GetValue(RepeatProperty); }
            set { SetValue(RepeatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Repeat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepeatProperty =
            DependencyProperty.Register("Repeat", typeof(RepeatBehavior), typeof(MarqueeTextUserControl), new PropertyMetadata(RepeatBehavior.Forever));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MarqueeTextUserControl cntrl && e.NewValue is XYTraversal movement)
            {
                switch (movement)
                {
                    case XYTraversal.LeftToRight:
                        cntrl.LeftToRightMarquee();
                        break;

                    case XYTraversal.RightToLeft:
                        cntrl.RightToLeftMarquee();
                        break;

                    case XYTraversal.BottomToTop:
                        cntrl.BottomToTopMarquee();
                        break;

                    case XYTraversal.TopToBottom:
                        cntrl.TopToBottomMarquee();
                        break;
                }
            }
        }

        private static void Changed2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MarqueeTextUserControl cntrl && e.NewValue is string)
            {
                switch (cntrl.Movement)
                {
                    case XYTraversal.LeftToRight:
                        cntrl.LeftToRightMarquee();
                        break;

                    case XYTraversal.RightToLeft:
                        cntrl.RightToLeftMarquee();
                        break;

                    case XYTraversal.BottomToTop:
                        cntrl.BottomToTopMarquee();
                        break;

                    case XYTraversal.TopToBottom:
                        cntrl.TopToBottomMarquee();
                        break;
                }
            }
        }

        public MarqueeTextUserControl()
        {
            InitializeComponent();
            this.Loaded += MarqueeTextUserControl_Loaded;
        }

        private void MarqueeTextUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Movement)
            {
                case XYTraversal.LeftToRight:
                    LeftToRightMarquee();
                    break;

                case XYTraversal.RightToLeft:
                    RightToLeftMarquee();
                    break;

                case XYTraversal.BottomToTop:
                    BottomToTopMarquee();
                    break;

                case XYTraversal.TopToBottom:
                    TopToBottomMarquee();
                    break;
            }
        }

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public XYTraversal Movement
        {
            get { return (XYTraversal)GetValue(MovementProperty); }
            set { SetValue(MovementProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void LeftToRightMarquee()
        {
            tbmarquee.BeginAnimation(Canvas.LeftProperty, Horizontal());
        }

        private void RightToLeftMarquee()
        {
            tbmarquee.BeginAnimation(Canvas.RightProperty, Horizontal());
        }

        private void TopToBottomMarquee()
        {
            tbmarquee.BeginAnimation(Canvas.TopProperty, Vertical());
        }

        private void BottomToTopMarquee()
        {
            tbmarquee.BeginAnimation(Canvas.BottomProperty, Vertical());
        }

        private DoubleAnimation Vertical()
        {
            double width = canMain.ActualWidth - tbmarquee.ActualWidth;
            tbmarquee.Margin = new Thickness(width / 2, 0, 0, 0);
            return new DoubleAnimation
            {
                From = -tbmarquee.ActualHeight,
                To = canMain.ActualHeight,
                RepeatBehavior = Repeat,
                Duration = Duration
            };
        }

        private DoubleAnimation Horizontal()
        {
            double height = canMain.ActualHeight - tbmarquee.ActualHeight;
            tbmarquee.Margin = new Thickness(0, height / 2, 0, 0);
            return new DoubleAnimation
            {
                From = -tbmarquee.ActualWidth,
                To = canMain.ActualWidth,
                RepeatBehavior = Repeat,
                Duration = Duration
            };
        }
    }
}
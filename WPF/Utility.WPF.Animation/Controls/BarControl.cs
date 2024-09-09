using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility.WPF.Animations
{
    public class BarControl : Control
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(BarControl), new PropertyMetadata(1000, DurationChanged));

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(BarControl), new PropertyMetadata(30d, _SizeChanged));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BarControl), new PropertyMetadata(Orientation.Vertical, OrientationChanged));

        private Rectangle rctMovingObject;

        public override void OnApplyTemplate()
        {
            rctMovingObject = GetTemplateChild("PART_MovingObject") as Rectangle;
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        static BarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BarControl), new FrameworkPropertyMetadata(typeof(BarControl)));
        }

        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public BarControl()
        {
        }

        private static void _SizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarControl bc = d as BarControl;
            DoubleAnimation animation = new DoubleAnimation((double)e.NewValue, TimeSpan.FromMilliseconds(bc.Duration));
            animation.From = 0;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            if (bc.Orientation == Orientation.Horizontal)
            {
                bc.rctMovingObject?.BeginAnimation(WidthProperty, animation);
            }
            if (bc.Orientation == Orientation.Vertical)
            {
                bc.rctMovingObject?.BeginAnimation(HeightProperty, animation);
            }
        }

        private static void DurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarControl bc = d as BarControl;
            DoubleAnimation animation = new DoubleAnimation(bc.Size, TimeSpan.FromMilliseconds(Convert.ToInt32(e.NewValue)));
            animation.From = 0;
            animation.RepeatBehavior = RepeatBehavior.Forever;

            if (bc.Orientation == Orientation.Horizontal)
            {
                bc.rctMovingObject?.BeginAnimation(WidthProperty, animation);
            }
            if (bc.Orientation == Orientation.Vertical)
            {
                bc.rctMovingObject?.BeginAnimation(HeightProperty, animation);
            }
        }

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as BarControl)
        }
    }
}
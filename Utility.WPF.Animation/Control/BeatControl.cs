using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility.WPF.Animation
{
    public class BeatControl : Control
    {
        static BeatControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BeatControl), new FrameworkPropertyMetadata(typeof(BeatControl)));
        }

        public BeatControl()
        {
        }

        private Rectangle rctMovingObject;

        public override void OnApplyTemplate()
        {
            rctMovingObject = this.GetTemplateChild("PART_MovingObject") as Rectangle;
        }

        public object Beat
        {
            get { return (object)GetValue(BeatProperty); }
            set { SetValue(BeatProperty, value); }
        }

        public static readonly DependencyProperty BeatProperty = DependencyProperty.Register("Beat", typeof(object), typeof(BeatControl), new PropertyMetadata(null, BeatChanged));

        private static void BeatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BeatControl bc = d as BeatControl;
            DoubleAnimation animation = new DoubleAnimation(50, TimeSpan.FromMilliseconds(50));
            animation.From = 0;

            bc.rctMovingObject?.BeginAnimation(Rectangle.WidthProperty, animation);
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility.WPF.Animation
{
    public class BlinkControl : Control
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(BlinkControl), new PropertyMetadata(1000, DurationChanged));

        private Ellipse ellipse;

        public override void OnApplyTemplate()
        {
            ellipse = this.GetTemplateChild("PART_Ellipse") as Ellipse;
        }

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        static BlinkControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlinkControl), new FrameworkPropertyMetadata(typeof(BlinkControl)));
        }

        public BlinkControl()
        {
        }

        private static void DurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BlinkControl bc = d as BlinkControl;
            if (bc.ellipse != null)
            {
                int seconds = Convert.ToInt32(e.NewValue);
                var blinkAnimation = new DoubleAnimationUsingKeyFrames();
                blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
                blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(seconds))));

                var blinkStoryboard = new Storyboard
                {
                    Duration = TimeSpan.FromMilliseconds(seconds * 2),
                    RepeatBehavior = RepeatBehavior.Forever,
                };

                Storyboard.SetTarget(blinkAnimation, bc.ellipse);
                Storyboard.SetTargetProperty(blinkAnimation, new PropertyPath(OpacityProperty));

                blinkStoryboard.Children.Add(blinkAnimation);
                blinkStoryboard.Begin();
            }
        }
    }
}
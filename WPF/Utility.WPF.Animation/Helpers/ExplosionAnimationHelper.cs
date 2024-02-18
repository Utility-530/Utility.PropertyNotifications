using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility.WPF.Animation.Infrastructure
{
    public class ExplosionAnimationHelper
    {
        public static IEnumerable<Timeline> CreateTargetAnimation(Ellipse toEll, double pointTime)
        {
            double particleTime = pointTime / 2d;

            yield return SetBlinkAnimation(toEll, particleTime);
            yield return SetColorAnimation(toEll, particleTime);

            ApplyOpacityMask(toEll);
        }

        public static void ApplyOpacityMask(UIElement element)
        {
            RadialGradientBrush rgBrush = new();
            GradientStop gStop0 = new(Color.FromArgb(255, 0, 0, 0), 0);
            GradientStop gStopT = new(Color.FromArgb(255, 0, 0, 0), 0);
            GradientStop gStop1 = new(Color.FromArgb(255, 0, 0, 0), 1);
            rgBrush.GradientStops.Add(gStop0);
            rgBrush.GradientStops.Add(gStopT);
            rgBrush.GradientStops.Add(gStop1);
            element.OpacityMask = rgBrush;
        }

        public static DoubleAnimation SetBlinkAnimation(UIElement ellipse, double particleTimeInSeconds)
        {
            var animation = BlinkAnimation(particleTimeInSeconds);
            SetBlinkAnimationStoryboard(animation, ellipse);
            return animation;
        }

        public static ColorAnimation SetColorAnimation(UIElement ellipse, double particleTimeInSeconds)
        {
            var animation = ColorAnimation(particleTimeInSeconds);
            SetColorAnimationStoryboard(animation, ellipse);
            return animation;
        }

        public static DoubleAnimation BlinkAnimation(double particleTime) => new()
        {
            From = 0.2,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(particleTime)),
            BeginTime = TimeSpan.FromSeconds(particleTime),
            FillBehavior = FillBehavior.HoldEnd
        };

        public static void SetBlinkAnimationStoryboard(DependencyObject animation, UIElement ellipse)
        {
            Storyboard.SetTarget(animation, ellipse);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Ellipse.OpacityProperty));
        }

        public static ColorAnimation ColorAnimation(double particleTime) => new()
        {
            To = Color.FromArgb(0, 0, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(particleTime)),
            BeginTime = TimeSpan.FromSeconds(particleTime),
            FillBehavior = FillBehavior.HoldEnd
        };

        public static void SetColorAnimationStoryboard(DependencyObject animation, UIElement element)
        {
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Ellipse.OpacityMask).(GradientBrush.GradientStops)[1].(GradientStop.Color)"));
        }
    }
}
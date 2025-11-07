using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Utility.WPF.Panels
{
    /// <example>
    ///
    /// </summary>
    public class AnimationHelper
    {
        public static void Animate(UIElement parent, UIElement child, Rect rect)
        {
            if (child.RenderTransform is not TranslateTransform translateTransform)
            {
                child.RenderTransform = translateTransform = new TranslateTransform();
            }
            var translationPoint = child.TranslatePoint(new Point(), parent);
            child.RenderTransformOrigin = translationPoint;

            child.Arrange(new Rect(new Point(translationPoint.X, translationPoint.Y), rect.Size));

            Animate(translateTransform, translationPoint, rect.Location);
        }

        public static void Animate(TranslateTransform trans, Point translationPoint, Point combinedPoint)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000))
            {
                IsCumulative = false,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
            };

            animation.From = translationPoint.X;
            animation.To = combinedPoint.X - translationPoint.X;
            trans.BeginAnimation(TranslateTransform.XProperty, animation, HandoffBehavior.SnapshotAndReplace);

            animation.From = translationPoint.Y;
            animation.To = combinedPoint.Y - translationPoint.Y;
            trans.BeginAnimation(TranslateTransform.YProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        public static void AnimatePosition(UIElement child, double x, double y, Duration duration)
        {
            //if (child.RenderTransform is not TranslateTransform translateTransform)
            //{
            child.RenderTransformOrigin = new Point(0, 0);
            var translateTransform = new TranslateTransform();
            child.RenderTransform = translateTransform;
            //}

            translateTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(x, duration), HandoffBehavior.Compose);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(y, duration), HandoffBehavior.Compose);
        }

        public static void AnimateSizeToZero(FrameworkElement element, double x, double y)
        {
            //ScaleTransform trans = new();
            //element.RenderTransform = trans;
            // if you use the same animation for X & Y you don't need anim1, anim2
            DoubleAnimation animHeight = new DoubleAnimation(y, 0, TimeSpan.FromMilliseconds(1000));
            DoubleAnimation animWidth = new DoubleAnimation(x, 0, TimeSpan.FromMilliseconds(1000));
            element.BeginAnimation(FrameworkElement.WidthProperty, animWidth);
            element.BeginAnimation(FrameworkElement.HeightProperty, animHeight);
        }

        public static void AnimateOpacityToZero(UIElement element)
        {
            DoubleAnimation animWidth = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            element.BeginAnimation(FrameworkElement.OpacityProperty, animWidth, HandoffBehavior.Compose);
        }

        public static void AnimateOpacityFromZero(UIElement element)
        {
            DoubleAnimation animWidth = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            element.BeginAnimation(FrameworkElement.OpacityProperty, animWidth, HandoffBehavior.Compose);
        }
    }
}
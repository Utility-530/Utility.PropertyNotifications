using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Animations.Helpers;

namespace Utility.WPF.Behaviors
{
    public class FadeBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty FadeInTimeProperty =
            DependencyProperty.Register("FadeInTime", typeof(double), typeof(FadeBehavior), new FrameworkPropertyMetadata(0.25));

        public static readonly DependencyProperty FadeOutTimeProperty =
            DependencyProperty.Register("FadeOutTime", typeof(double), typeof(FadeBehavior), new FrameworkPropertyMetadata(1.0));

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(FadeBehavior), new PropertyMetadata(true, FadeChanged));

        private static void FadeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FadeBehavior fadeBehavior && e.NewValue is bool b)
            {
                if (b)
                    fadeBehavior.AssociatedObject.FadeIn(TimeSpan.FromSeconds(fadeBehavior.FadeInTime));
                else
                    fadeBehavior.AssociatedObject.FadeOut(TimeSpan.FromSeconds(fadeBehavior.FadeOutTime));
            }
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        /// Specifies the time (in seconds) it takes to fade in the adorner.
        /// </summary>
        public double FadeInTime
        {
            get => (double)GetValue(FadeInTimeProperty);
            set => SetValue(FadeInTimeProperty, value);
        }

        /// <summary>
        /// Specifies the time (in seconds) it takes to fade out the adorner.
        /// </summary>
        public double FadeOutTime
        {
            get => (double)GetValue(FadeOutTimeProperty);
            set => SetValue(FadeOutTimeProperty, value);
        }
    }
}
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Utility.WPF.Controls.Base
{
    public class Animator : FrameworkElement
    {
        // --- Dependency Properties ---

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(Animator),
                new PropertyMetadata(OnIsExpandedChanged));

        public static readonly DependencyProperty AnimatedValueProperty =
            DependencyProperty.Register(
                nameof(AnimatedValue),
                typeof(double),
                typeof(Animator),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(
                nameof(AnimationDuration),
                typeof(Duration),
                typeof(Animator),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.3))));

        public static readonly DependencyProperty FromValueProperty =
            DependencyProperty.Register(
                nameof(FromValue),
                typeof(double),
                typeof(Animator),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty ToValueProperty =
            DependencyProperty.Register(
                nameof(ToValue),
                typeof(double),
                typeof(Animator),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty AutoReverseProperty =
            DependencyProperty.Register(
                nameof(AutoReverse),
                typeof(bool),
                typeof(Animator),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ForeverProperty =
            DependencyProperty.Register(
                nameof(Forever),
                typeof(bool),
                typeof(Animator),
                new PropertyMetadata(false));

        #region properties

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public double AnimatedValue
        {
            get => (double)GetValue(AnimatedValueProperty);
            set => SetValue(AnimatedValueProperty, value);
        }

        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public double FromValue
        {
            get => (double)GetValue(FromValueProperty);
            set => SetValue(FromValueProperty, value);
        }

        public double ToValue
        {
            get => (double)GetValue(ToValueProperty);
            set => SetValue(ToValueProperty, value);
        }

        public bool AutoReverse
        {
            get => (bool)GetValue(AutoReverseProperty);
            set => SetValue(AutoReverseProperty, value);
        }

        public bool Forever
        {
            get => (bool)GetValue(ForeverProperty);
            set => SetValue(ForeverProperty, value);
        }

        #endregion properties

        // --- Routed Event Declaration ---
        public static readonly RoutedEvent AnimationCompletedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(AnimationCompleted),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(Animator));

        public event RoutedEventHandler AnimationCompleted
        {
            add => AddHandler(AnimationCompletedEvent, value);
            remove => RemoveHandler(AnimationCompletedEvent, value);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Animator)d).RunAnimation((bool)e.NewValue);
        }

        private void RunAnimation(bool expand)
        {
            var duration = AnimationDuration;

            var animation = new DoubleAnimation
            {
                From = expand ? FromValue : ToValue,
                To = expand ? ToValue : FromValue,
                Duration = duration,
                AutoReverse = AutoReverse,
                RepeatBehavior = Forever ? RepeatBehavior.Forever : new RepeatBehavior(1)
            };

            if (!Forever)
            {
                animation.Completed += (s, e) =>
                {
                    RaiseEvent(new RoutedEventArgs(AnimationCompletedEvent, this));
                };
            }

            BeginAnimation(AnimatedValueProperty, animation);
        }
    }
}
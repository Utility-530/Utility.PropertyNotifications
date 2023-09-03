using MaterialDesignThemes.Wpf;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Utility.WPF.Reactive;

namespace Utility.WPF.Controls
{
    public class AnimatedPackIcon : ContentControl
    {
        private bool flag = false;
        private readonly Storyboard storyboardOld = new();
        private readonly Storyboard storyboardNew = new();
        private readonly ReplaySubject<Unit> completions = new();

        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(int), typeof(AnimatedPackIcon), new PropertyMetadata(3000));
        public static readonly DependencyProperty KindProperty = DependencyProperty.Register("Kind", typeof(PackIconKind), typeof(AnimatedPackIcon), new PropertyMetadata(PackIconKind.None, Changed));

        static AnimatedPackIcon()
        {
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedPackIcon { flag: false } icon && e.NewValue is PackIconKind kind && e.OldValue is PackIconKind old)
            {
                icon.StartStories(old, kind);
            }
        }

        public AnimatedPackIcon()
        {
            completions
                .Subscribe(a =>
                {
                    if (flag == true)
                    {
                        //Opacity = 0;
                        //this.SetValue(KindProperty, a);
                        this.Content = new PackIcon
                        {
                            Width = 70,
                            Height = 70,
                            Margin = new Thickness(4),
                            Kind = Kind,
                        };
                        StartStory(this.Content as UIElement, storyboardNew, true, (int)(AnimationTime / 2d));
                    }
                    flag = false;
                });

            storyboardOld
                .Completions()
                .Select(a => Unit.Default)
                .Subscribe(completions.OnNext);
        }

        public int AnimationTime
        {
            get { return (int)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }

        public PackIconKind Kind
        {
            get { return (PackIconKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public void StartStories(PackIconKind oldElement, PackIconKind newElement)
        {
            if (flag)
                return;
            flag = true;

            if (this.Content is UIElement element)
                StartStory(element, storyboardOld, false, (int)(AnimationTime / 2d));
            else
                completions.OnNext(Unit.Default);
        }

        public static void StartStory(UIElement element, Storyboard storyboard, bool addRemove = true, int animationTime = 500)
        {
            storyboard.Children.Clear();
            TimeSpan duration = TimeSpan.FromMilliseconds(animationTime);
            DoubleAnimation fadeAnimation = new DoubleAnimation
            { From = addRemove ? 0 : 1, To = addRemove ? 1 : 0, Duration = new Duration(duration), EasingFunction = new CubicEase { } };
            Storyboard.SetTarget(fadeAnimation, element);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(Control.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Begin();
        }
    }
}
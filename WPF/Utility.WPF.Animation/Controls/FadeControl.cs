using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Utility.WPF.Animations.Helpers;

namespace Utility.WPF.Animation
{
    public class FadeControl : ContentControl
    {
        private bool animatingFlag;
        private readonly Storyboard storyboard = new();
        private readonly ReplaySubject<EventArgs?> completions = new(1);
        //private UIElement? newElement = null;

        public static readonly DependencyProperty FadeCommandProperty = DependencyProperty.Register(nameof(FadeCommand), typeof(ICommand), typeof(FadeControl), new PropertyMetadata(default(ICommand), Changed));

        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(int), typeof(FadeControl), new PropertyMetadata(1000, Changed));

        public static readonly DependencyProperty IsInvertFadeProperty = DependencyProperty.Register("IsInvertFade", typeof(bool), typeof(FadeControl), new PropertyMetadata(false, Changed));

        public static readonly DependencyProperty ReverseProperty = DependencyProperty.Register("Reverse", typeof(bool), typeof(FadeControl), new PropertyMetadata(true, Changed));

        static FadeControl()
        {
            //  FadeControl.ContentProperty.OverrideMetadata(typeof(FadeControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, Changed2, null, true));
        }

        public FadeControl()
        {
            FadeCommand = new FadeCommand(this);
        }

        #region properties

        public bool Reverse
        {
            get => (bool)GetValue(ReverseProperty);
            set => SetValue(ReverseProperty, value);
        }

        public bool IsInvertFade
        {
            get => (bool)GetValue(IsInvertFadeProperty);
            set => SetValue(IsInvertFadeProperty, value);
        }

        public ICommand FadeCommand
        {
            get => (ICommand)GetValue(FadeCommandProperty);
            set => SetValue(FadeCommandProperty, value);
        }

        public int AnimationTime
        {
            get => (int)GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            storyboard.SelectCompletions().Subscribe(completions.OnNext);
            completions
                .Subscribe(_ =>
                {
                    if (Content is not FrameworkElement element)
                        return;

                    if (animatingFlag)
                    {
                        //element.Opacity = 0;
                        if (storyboard.Children.Count == 0)
                            AnimationHelper.CreateFadeStoryboard(element, storyboard, IsInvertFade, Reverse, AnimationTime);

                        storyboard.Begin();
                    }
                    // because FillBehavior is Stop is Reverse not true have to set opacity at end of animation
                    else if (Reverse == false)
                    {
                        element.Opacity = IsInvertFade ? 1 : 0;
                    }

                    animatingFlag = false;
                }, _ => animatingFlag = false);

            base.OnApplyTemplate();
        }

        public void SwapElementsAndAnimate(UIElement? newElement)
        {
            //if (newElement != null)
            //{
            //    Content = newElement;
            //    completions.OnNext(null);
            //}

            if (animatingFlag)
                return;

            animatingFlag = true;
            completions.OnNext(null);
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FadeControl).storyboard.Children.Clear();
        }
    }

    public static class AnimationHelper
    {
        public static void CreateFadeStoryboard(UIElement element, Storyboard storyboard, bool isInvertFade = false, bool reverse = true, int animationTime = 500)
        {
            storyboard.Children.Clear();
            TimeSpan duration = TimeSpan.FromMilliseconds(animationTime);

            DoubleAnimation fadeOutAnimation = new(element.Opacity > 0 ? element.Opacity : isInvertFade ? 0 : 1, toValue: isInvertFade ? 1 : 0, duration: new Duration(duration))
            { EasingFunction = new SineEase(), AutoReverse = reverse, FillBehavior = reverse ? FillBehavior.HoldEnd : FillBehavior.Stop };

            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(nameof(FrameworkElement.Opacity)));
            storyboard.Children.Add(fadeOutAnimation);
        }
    }

    internal class FadeCommand : ICommand
    {
        private readonly FadeControl fadeControl;

        public event EventHandler? CanExecuteChanged;

        public FadeCommand(FadeControl fadeControl) => this.fadeControl = fadeControl;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            fadeControl.SwapElementsAndAnimate(/*fadeControl.Content as UIElement,*/ parameter as UIElement);
        }
    }
}
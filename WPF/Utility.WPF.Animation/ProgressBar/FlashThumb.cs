using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Utility.Commands;

namespace Utility.WPF.Animation
{
    public class FlashThumb : Thumb
    {
        private Storyboard storyBoard;
        private Grid grid;

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration), typeof(FlashThumb), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(1)), Changed));

        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register("Seconds", typeof(int), typeof(FlashThumb), new PropertyMetadata(1, Changed));

        public static readonly DependencyProperty FlashProperty =
           DependencyProperty.Register("Flash", typeof(ICommand), typeof(FlashThumb), new PropertyMetadata(default));

        public static readonly DependencyProperty RepeatProperty =
           DependencyProperty.Register("Repeat", typeof(bool), typeof(FlashThumb), new PropertyMetadata(true, RepeatChanged));

        static FlashThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlashThumb), new FrameworkPropertyMetadata(typeof(FlashThumb)));
        }

        public bool Repeat
        {
            get { return (bool)GetValue(RepeatProperty); }
            set { SetValue(RepeatProperty, value); }
        }

        public ICommand Flash
        {
            get { return (ICommand)GetValue(FlashProperty); }
            set { SetValue(FlashProperty, value); }
        }

        public int Seconds
        {
            get { return (int)((Duration)GetValue(DurationProperty)).TimeSpan.TotalSeconds; }
            set { SetValue(DurationProperty, new Duration(TimeSpan.FromSeconds(value))); }
        }

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Duration duration;
            if (d is FlashThumb flashView)
                if (!(e.NewValue is int newValue))
                    if (e.NewValue is Duration durationValue)
                        duration = durationValue;
                    else
                        return;
                else
                    duration = new Duration(TimeSpan.FromSeconds(newValue));
            else
                return;

            flashView.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var clone = flashView.storyBoard.Clone();
                flashView.storyBoard.Stop();
                clone.Duration = duration;
                foreach (var child in clone.Children)
                {
                    child.Duration = flashView.Duration;
                }
                clone.Begin(flashView.grid);
            }));
        }

        private static void RepeatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FlashThumb flashView && e.NewValue is bool newValue))
                return;

            flashView.storyBoard.Stop();
            flashView.storyBoard.RepeatBehavior = newValue ? RepeatBehavior.Forever : new RepeatBehavior(1);

            var clone = flashView.storyBoard.Clone();
            flashView.storyBoard.Stop();
            clone.RepeatBehavior = newValue ? RepeatBehavior.Forever : new RepeatBehavior(1);
            clone.Begin(flashView.grid);
        }

        public FlashThumb()
        {
            Flash = new Command(() =>
            {
                Repeat = false;
            });

            Loaded += FlashView_Loaded;
        }

        public override void OnApplyTemplate()
        {
            grid = GetTemplateChild("thumbGrid") as Grid;
            ; storyBoard = Template.Resources["MainStoryboard"] as Storyboard;
            //this.ellipse = this.GetTemplateChild("focusedHalo") as Ellipse;
            //Storyboard.SetTargetName(storyBoard, "focusedHalo");
            base.OnApplyTemplate();
        }

        private void FlashView_Loaded(object sender, RoutedEventArgs e)
        {
            storyBoard.Begin(grid);
        }
    }
}
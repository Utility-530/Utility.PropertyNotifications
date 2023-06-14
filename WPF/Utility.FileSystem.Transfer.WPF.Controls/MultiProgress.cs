using System;
using System.Collections;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Utility.FileSystem.Transfer.WPF.Controls.Abstract;
using UtilityWpf;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class MultiProgress : Control
    {
        public static readonly DependencyProperty ProgressViewsProperty = DependencyProperty.Register(nameof(ProgressViews), typeof(IEnumerable), typeof(MultiProgress), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty IsCompleteProperty = DependencyProperty.Register(nameof(IsComplete), typeof(bool), typeof(MultiProgress), new PropertyMetadata((object)false));
        public static readonly RoutedEvent CompleteEvent = EventManager.RegisterRoutedEvent("Complete", RoutingStrategy.Bubble, typeof(MultiProgress.TimeSpanRoutedEventHandler), typeof(MultiProgress));
        private ContentControl contentControl;
        private TextBlock textBlock;

        static MultiProgress() => FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiProgress), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(MultiProgress)));

        public MultiProgress() => this.Loaded += new RoutedEventHandler(this.MultiStageProgressView_Loaded);

        public int CountDown { get; set; } = 3;

        public IEnumerable ProgressViews
        {
            get => (IEnumerable)this.GetValue(MultiProgress.ProgressViewsProperty);
            set => this.SetValue(MultiProgress.ProgressViewsProperty, (object)value);
        }

        public bool IsComplete
        {
            get => (bool)this.GetValue(MultiProgress.IsCompleteProperty);
            set => this.SetValue(MultiProgress.IsCompleteProperty, (object)value);
        }

        private void MultiStageProgressView_Loaded(object sender, RoutedEventArgs e)
        {
            this.contentControl = this.ChildOfType<ContentControl>();
            TextBlock textBlock = new TextBlock();
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.FontSize = 40.0;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            this.textBlock = textBlock;
            this.contentControl.Content = (object)this.textBlock;
            int countDown = this.CountDown;
            Observable.Interval(TimeSpan.FromSeconds(1.0)).Scan<long, int>(countDown, (Func<int, long, int>)((a, _) => a - 1)).Take<int>(countDown).ObserveOnDispatcher<int>().Subscribe<int>(new Action<int>(this.CountDownChange), (Action)(async () =>
            {
                IProgressView[] arr = this.ProgressViews.Cast<IProgressView>().ToArray<IProgressView>();
                IProgressView[] progressViewArray = arr;
                for (int index = 0; index < progressViewArray.Length; ++index)
                {
                    IProgressView progressView = progressViewArray[index];
                    TimeSpan timeSpan = await this.StepChange(progressView);
                    progressView = (IProgressView)null;
                }
                progressViewArray = (IProgressView[])null;
                this.RaiseCompleteEvent();
                arr = (IProgressView[])null;
            }));
        }

        protected virtual async Task<TimeSpan> StepChange(IProgressView progressView)
        {
            Task task1 = await this.Dispatcher.InvokeAsync(Callback);
            TimeSpan task = await progressView.CompleteEvents.Take<TimeSpan>(1).ToTask<TimeSpan>();
            return task;

            static DoubleAnimation GetAnimation()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.To = new double?(0.0);
                doubleAnimation.Duration = (Duration)TimeSpan.FromSeconds(2.0);
                doubleAnimation.FillBehavior = FillBehavior.Stop;
                return doubleAnimation;
            }

            static DoubleAnimation GetAnimation2()
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.To = new double?(1.0);
                doubleAnimation.Duration = (Duration)TimeSpan.FromSeconds(2.0);
                doubleAnimation.FillBehavior = FillBehavior.Stop;
                return doubleAnimation;
            }

            static Task<EventPattern<object>> ToTask(DoubleAnimation animation) => Observable.FromEventPattern((Action<EventHandler>)(a => animation.Completed += a), (Action<EventHandler>)(a => animation.Completed -= a)).Take<EventPattern<object>>(1).ToTask<EventPattern<object>>();

            async Task Callback()
            {
                DoubleAnimation animation = GetAnimation();
                Task<EventPattern<object>> task = ToTask(animation);
                this.contentControl.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)animation);
                EventPattern<object> eventPattern1 = await task;
                this.contentControl.Content = (object)progressView;
                DoubleAnimation animation2 = GetAnimation2();
                task = ToTask(animation2);
                this.contentControl.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)animation2);
                progressView.RunCommand.Execute((object)null);
                EventPattern<object> eventPattern2 = await task;
                animation = (DoubleAnimation)null;
                task = (Task<EventPattern<object>>)null;
                animation2 = (DoubleAnimation)null;
            }
        }

        protected virtual void CountDownChange(int a) => this.textBlock.Text = a.ToString();

        public event RoutedEventHandler Complete
        {
            add => this.AddHandler(MultiProgress.CompleteEvent, (Delegate)value);
            remove => this.RemoveHandler(MultiProgress.CompleteEvent, (Delegate)value);
        }

        private void RaiseCompleteEvent() => this.RaiseEvent(new RoutedEventArgs(MultiProgress.CompleteEvent));

        public delegate void TimeSpanRoutedEventHandler(object sender, TimeSpanRoutedEventArgs e);
    }
}

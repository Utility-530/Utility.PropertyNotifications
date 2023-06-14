using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Utility.WPF.Controls.Clocks
{
    [TemplateVisualState(Name = "Day", GroupName = "TimeStates")]
    [TemplateVisualState(Name = "Night", GroupName = "TimeStates")]
    [TemplateVisualState(Name = "Christmas", GroupName = "TimeStates")]
    public class Clock : Control
    {
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(DateTime), typeof(Clock), new PropertyMetadata(DateTime.Now, TimePropertyChanged));
        public static readonly DependencyProperty ShowSecondsProperty = DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(Clock), new PropertyMetadata(true));
        public static readonly RoutedEvent TimeChangedEvent = EventManager.RegisterRoutedEvent("TimeChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(Clock));

        private static void TimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Clock clock && e is { NewValue: DateTime dateTime, OldValue: DateTime oDateTime })
            {
                clock.RaiseEvent(new RoutedPropertyChangedEventArgs<DateTime>(oDateTime, dateTime, TimeChangedEvent));
            }
        }

        public DateTime Time
        {
            get { return (DateTime)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public bool ShowSeconds
        {
            get { return (bool)GetValue(ShowSecondsProperty); }
            set { SetValue(ShowSecondsProperty, value); }
        }

        public event RoutedPropertyChangedEventHandler<DateTime> TimeChanged
        {
            add
            {
                AddHandler(TimeChangedEvent, value);
            }
            remove
            {
                RemoveHandler(TimeChangedEvent, value);
            }
        }

        public override void OnApplyTemplate()
        {
            OnTimeChanged(DateTime.Now);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += (s, e) => OnTimeChanged(DateTime.Now.AddHours(-1));
            timer.Start();

            base.OnApplyTemplate();
        }

        protected virtual void OnTimeChanged(DateTime newTime)
        {
            UpdateTimeState(newTime);
            Time = newTime;
        }

        private void UpdateTimeState(DateTime time)
        {
            if (time.Day == 25 && time.Month == 12)
            {
                VisualStateManager.GoToState(this, "Christmas", false);
            }
            else
            {
                if (time.Hour > 6 && time.Hour < 18)
                {
                    VisualStateManager.GoToState(this, "Day", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Night", false);
                }
            }
        }
    }
}
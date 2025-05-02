using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Threading;

namespace Leepfrog.WpfFramework.Behaviors
{
    /// <summary>
    /// Behaviour to be applied to Label controls
    /// When label's target is bound to a required field, IsTimeout = true
    /// </summary>
    public static class TimeoutBehavior
    {
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.RegisterAttached(
            "Duration",
            typeof(double),
            typeof(TimeoutBehavior),
            new UIPropertyMetadata(0.0, OnDurationChanged));

        public static double GetDuration(DependencyObject d)
        {
            return (double)d.GetValue(DurationProperty);
        }

        public static void SetDuration(DependencyObject d, double value)
        {
            d.SetValue(DurationProperty, value);
        }


        #region IsTimeoutSuspended - global/static property
        public static bool IsTimeoutSuspended = false;

        public static bool GetIsTimeoutSuspended(DependencyObject obj)
        {
            return IsTimeoutSuspended;
        }

        public static void SetIsTimeoutSuspended(DependencyObject obj, bool value)
        {
            IsTimeoutSuspended = value;
        }

        // Using a DependencyProperty as the backing store for IsTimeoutSuspended.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTimeoutSuspendedProperty =
            DependencyProperty.RegisterAttached("IsTimeoutSuspended", typeof(bool), typeof(TimeoutBehavior), new PropertyMetadata(false,isTimeoutSuspended_PropertyChanged));

        private static void isTimeoutSuspended_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsTimeoutSuspended = (bool)e.NewValue;
        }

        #endregion



        public static double GetTimeRemaining(DependencyObject obj)
        {
            return (double)obj.GetValue(TimeRemainingProperty);
        }

        public static void SetTimeRemaining(DependencyObject obj, double value)
        {
            obj.SetValue(TimeRemainingProperty, value);
        }

        // Using a DependencyProperty as the backing store for TimeRemaining.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeRemainingProperty =
            DependencyProperty.RegisterAttached("TimeRemaining", typeof(double), typeof(TimeoutBehavior), new PropertyMetadata(0.0));


        private static void OnDurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as Button;
            // IF SENDER IS NOT A BUTTON, JUST EXIT
            if (button == null)
            {
                return;
            }

            double duration = (double)(e.NewValue);

            // GET DESCRIPTOR OF BUTTON'S ISVISIBLE PROPERTY
            hookControl(button, duration);
        }

        private static void hookControl(Button button, double duration)
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(Button.IsVisibleProperty, typeof(Button));
            if (duration != 0)
            {
                // REGISTER FOR UPDATES WHENEVER THE TARGET PROPERTY CHANGES...
                dpd.AddValueChanged(button, button_IsVisibleChanged);
                button.AddHandler(Button.UnloadedEvent, new RoutedEventHandler(button_Unloaded), true);
            }
            else
            {
                // UNREGISTER
                dpd.RemoveValueChanged(button, button_IsVisibleChanged);
                button.RemoveHandler(Button.UnloadedEvent, new RoutedEventHandler(button_Unloaded));
            }

        }
        private static void button_Unloaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            hookControl(button, 0);
        }



        static object _timerLock = new object();
        static DispatcherTimer _timer;
        static List<Button> _buttonsWithTimerRunning = new List<Button>();

        private static void button_IsVisibleChanged(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button.IsVisible)
            {
                // set currentDuration to Duration
                button.SetValue(TimeRemainingProperty, button.GetValue(DurationProperty));
                // start a timer to run at 100ms intervals
                startTimer(button);
            }
            else
            {
                // stop the existing timer
                stopTimer(button);
            }
        }

        private static void startTimer(Button button)
        {
            lock (_timerLock)
            {
                while (_buttonsWithTimerRunning.Remove(button)) ;
                _buttonsWithTimerRunning.Add(button);
                if (_timer == null)
                {
                    _timer = new DispatcherTimer(DispatcherPriority.Send);
                    _timer.Interval = TimeSpan.FromSeconds(0.1);
                    _timer.Tick += _timer_Tick;
                }
                _timer.Start();
            }
        }

        private static void _timer_Tick(object sender, EventArgs e)
        {
            if (IsTimeoutSuspended)
            {
                return;
            }
            List<Button> buttons;
            lock (_timerLock)
            {
                buttons = new List<Button>(_buttonsWithTimerRunning);
            }
            foreach (var button in buttons)
            {
                var timeRemaining = (double)button.GetValue(TimeRemainingProperty);
                timeRemaining -= 0.1;
                if (timeRemaining < 0)
                {
                    timeRemaining = 0;
                }
                button.SetValue(TimeRemainingProperty, timeRemaining);
                if (timeRemaining == 0)
                {
                    stopTimer(button);
                    button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }


        private static void stopTimer(Button button)
        {

            lock (_timerLock)
            {
                while (_buttonsWithTimerRunning.Remove(button)) ;
                if (!_buttonsWithTimerRunning.Any())
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Tick -= _timer_Tick;
                        _timer = null;
                    }
                }
            }

        }

    }
}

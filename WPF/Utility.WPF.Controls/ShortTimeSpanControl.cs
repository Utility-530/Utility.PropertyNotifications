using System;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;

namespace Utility.WPF.Controls
{
    public class ShortTimeSpanControl : Control
    {
        protected static readonly DependencyProperty SecondsProperty = DependencyHelper.Register<int>(new PropertyMetadata(0, OnSecondsChanged));
        protected static readonly DependencyProperty MinutesProperty = DependencyHelper.Register<int>(new PropertyMetadata(0, OnMinutesChanged));
        protected static readonly DependencyProperty HoursProperty = DependencyHelper.Register<int>(new PropertyMetadata(0, OnHoursChanged));
        protected static readonly DependencyProperty ValueProperty = DependencyHelper.Register<TimeSpan>(new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<TimeSpan>), typeof(ShortTimeSpanControl));
        private static readonly RoutedEvent HoursChangedEvent = EventManager.RegisterRoutedEvent("HoursChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));
        private static readonly RoutedEvent MinutesChangedEvent = EventManager.RegisterRoutedEvent("MinutesChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));
        private static readonly RoutedEvent SecondsChangedEvent = EventManager.RegisterRoutedEvent("SecondsChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(ShortTimeSpanControl));

        static ShortTimeSpanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShortTimeSpanControl), new FrameworkPropertyMetadata(typeof(ShortTimeSpanControl)));
        }

        public ShortTimeSpanControl()
        {
        }

        #region properties

        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion properties

        #region events

        public event RoutedPropertyChangedEventHandler<TimeSpan> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> HoursChanged
        {
            add { AddHandler(HoursChangedEvent, value); }
            remove { RemoveHandler(HoursChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> MinutesChanged
        {
            add { AddHandler(MinutesChangedEvent, value); }
            remove { RemoveHandler(MinutesChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> SecondsChanged
        {
            add { AddHandler(SecondsChangedEvent, value); }
            remove { RemoveHandler(SecondsChangedEvent, value); }
        }

        #endregion events

        private static void OnHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeComponentChanged(d, args, (control, newValue) => new TimeSpan(newValue, control.Value.Minutes, control.Value.Seconds), HoursChangedEvent);
        }

        private static void OnMinutesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeComponentChanged(d, args, (control, newValue) => new TimeSpan(control.Value.Hours, newValue, control.Value.Seconds), MinutesChangedEvent);
        }

        private static void OnSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeComponentChanged(d, args, (control, newValue) => new TimeSpan(control.Value.Hours, control.Value.Minutes, newValue), SecondsChangedEvent);
        }

        private static void TimeComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args, Func<ShortTimeSpanControl, int, TimeSpan> func, RoutedEvent routedEvent)
        {
            if (d is not ShortTimeSpanControl control)
                return;

            int oldValue = (int)args.OldValue;
            int newValue = (int)args.NewValue;

            if (oldValue != newValue)
            {
                control.Value = func(control, newValue);
            }

            control.RaiseEvent(new RoutedPropertyChangedEventArgs<int>(oldValue, newValue, routedEvent));
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is not ShortTimeSpanControl control)
                return;

            var oldValue = (TimeSpan)args.OldValue;
            var newValue = (TimeSpan)args.NewValue;

            if (oldValue != newValue)
            {
                control.Hours = newValue.Hours;
                control.Minutes = newValue.Minutes;
                control.Seconds = newValue.Seconds;
            }

            control.RaiseEvent(new RoutedPropertyChangedEventArgs<TimeSpan>(oldValue, newValue, ValueChangedEvent));
        }
    }
}
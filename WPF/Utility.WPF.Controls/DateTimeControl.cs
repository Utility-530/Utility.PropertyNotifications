using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.Structs;

namespace Utility.WPF.Controls
{
    public class DateTimeControl : Control
    {
        public delegate void DateTimeRangeChangedEventHandler(object source, DateRangeChangeRoutedEventArgs rangeChange);

        private readonly ISubject<DateTime> startChanges = new Subject<DateTime>();
        private readonly ISubject<DateTime> endChanges = new Subject<DateTime>();

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(DateTime), typeof(DateTimeControl), new PropertyMetadata(default(DateTime), StartChanged));

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(DateTime), typeof(DateTimeControl), new PropertyMetadata(default(DateTime), EndChanged));

        public static readonly RoutedEvent DateRangeChangeEvent = EventManager.RegisterRoutedEvent("DateRangeChange", RoutingStrategy.Bubble, typeof(DateTimeRangeChangedEventHandler), typeof(DateTimeControl));

        static DateTimeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeControl), new FrameworkPropertyMetadata(typeof(DateTimeControl)));
        }

        public DateTimeControl()
        {
            var aa = startChanges.Where(a => a != default).DistinctUntilChanged();
            var bb = endChanges.Where(a => a != default).DistinctUntilChanged();
            _ = aa
                .CombineLatest(bb, (s, e) => (s, e)).Subscribe(a => RaiseDateRangeChangeEvent(a.s, a.e));
        }

        public override void OnApplyTemplate()
        {
            startChanges.OnNext(Start);
            endChanges.OnNext(End);
        }

        public DateTime Start
        {
            get { return (DateTime)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public DateTime End
        {
            get { return (DateTime)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public event DateTimeRangeChangedEventHandler DateRangeChange
        {
            add { AddHandler(DateRangeChangeEvent, value); }
            remove { RemoveHandler(DateRangeChangeEvent, value); }
        }

        private static void StartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeControl).startChanges.OnNext((DateTime)e.NewValue);
        }

        private static void EndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DateTimeControl).endChanges.OnNext((DateTime)e.NewValue);
        }

        private void RaiseDateRangeChangeEvent(DateTime start, DateTime end)
        {
            DateRangeChangeRoutedEventArgs newEventArgs = new DateRangeChangeRoutedEventArgs(DateRangeChangeEvent, start, end);
            RaiseEvent(newEventArgs);
        }

        public class DateRangeChangeRoutedEventArgs : RoutedEventArgs
        {
            public DateRange Range;

            public DateRangeChangeRoutedEventArgs(RoutedEvent @event, DateTime start, DateTime end) : base(@event)
            {
                Range = new DateRange(start, end);
            }
        }
    }
}
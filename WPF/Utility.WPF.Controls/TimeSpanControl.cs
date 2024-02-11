using Evan.Wpf;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Enums;
using Utility.WPF.Controls.Base;
using Utility.WPF.Controls.Infrastructure;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls
{
    using Utility.WPF.Abstract;

    public class TimeSpanControl : Controlx
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<(decimal value, TimeInterval timeInterval)>), typeof(TimeSpanControl));

        public static DependencyProperty TimeIntervalProperty = DependencyHelper.Register<TimeInterval>();
        public static DependencyProperty ValueProperty = DependencyHelper.Register<decimal>();

        static TimeSpanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanControl), new FrameworkPropertyMetadata(typeof(TimeSpanControl)));
        }

        public TimeSpanControl()
        {
            this.Control<ComboBox>().Subscribe(a =>
            {
                a.ItemsSource = Enum.GetValues(typeof(TimeInterval));
            });
        }

        public event RoutedEventHandler<(decimal value, TimeInterval timeInterval)> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            this.Control<ComboBox>().Select(a => a.Changes<TimeInterval>().StartWith((TimeInterval)this.GetValue(TimeIntervalProperty))
                      .CombineLatest(this.Control<SpinnerControl>().SelectMany(a => a.ValueChanges()).StartWith((decimal)this.GetValue(ValueProperty)), (a, b) => (b, a))
                      .DistinctUntilChanged()
                .Subscribe(a =>
                {
                    this.SetValue(TimeIntervalProperty, a.a);
                    this.SetValue(ValueProperty, a.b);
                    RaiseEvent(new RoutedEventArgs<(decimal value, TimeInterval timeInterval)>(a, ValueChangedEvent));
                }));

            this.Control<SpinnerControl>()
                .CombineLatest(this.Observable(ValueProperty.Name).Cast<decimal>().StartWith(1M).DistinctUntilChanged(), (a, b) => (a, b))
    .Subscribe(a =>
    {
        a.a.Value = a.b;
    });

            this.Control<ComboBox>().CombineLatest(this.Observable(TimeIntervalProperty.Name).Cast<TimeInterval>().StartWith(TimeInterval.Second).DistinctUntilChanged(), (a, b) => (a, b))
                .Subscribe(a =>
                {
                    a.a.SelectedItem = a.b;
                });

            base.OnApplyTemplate();
        }
    }
}
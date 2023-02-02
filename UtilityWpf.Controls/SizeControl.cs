using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Utility.WPF.Controls.Base;

namespace UtilityWpf.Controls
{
    using Utility.WPF.Reactive;

    using static DependencyPropertyFactory<SizeControl>;

    public class SizeControl : Controlx
    {
        public static readonly DependencyProperty SizeProperty = Register(nameof(Size), 25, CoerceSize);
        public static readonly DependencyProperty TotalSizeProperty = Register(nameof(TotalSize), 100, CoerceTotalSize);
        public static readonly RoutedEvent SelectedSizeChangedEvent = EventManager.RegisterRoutedEvent("SelectedSizeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SizeControl));

        private static object CoerceSize(DependencyObject d, object baseValue)
        {
            return Math.Max(1, Math.Min((int)baseValue, (d as SizeControl).TotalSize));
        }

        private static object CoerceTotalSize(DependencyObject d, object baseValue)
        {
            return Math.Max((int)baseValue, 1);
        }

        static SizeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SizeControl), new FrameworkPropertyMetadata(typeof(SizeControl)));
        }

        public SizeControl()
        {
            this.Observable(nameof(Size)).Select(_ => (int)_).Subscribe(RaiseSelectedSizeEvent);
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int TotalSize
        {
            get { return (int)GetValue(TotalSizeProperty); }
            set { SetValue(TotalSizeProperty, value); }
        }

        public event RoutedEventHandler SelectedSizeChanged
        {
            add { AddHandler(SelectedSizeChangedEvent, value); }
            remove { RemoveHandler(SelectedSizeChangedEvent, value); }
        }

        private void RaiseSelectedSizeEvent(int Size)
        {
            SelectedSizeChangedRoutedEventArgs newEventArgs = new SelectedSizeChangedRoutedEventArgs(SizeControl.SelectedSizeChangedEvent) { Size = Size };
            this.Dispatcher.Invoke(() =>
            RaiseEvent(newEventArgs));
        }

        public class SelectedSizeChangedRoutedEventArgs : RoutedEventArgs
        {
            public int Size { get; set; }

            public SelectedSizeChangedRoutedEventArgs(RoutedEvent @event) : base(@event)
            {
            }
        }
    }
}
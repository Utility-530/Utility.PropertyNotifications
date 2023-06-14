using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls
{
    public class SkipControl : ContentControl
    {
        public static readonly DependencyProperty PreviousCommandProperty = DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(SkipControl));

        public static readonly DependencyProperty NextCommandProperty = DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(SkipControl));

        public static readonly DependencyProperty CanMoveToNextProperty = DependencyProperty.Register("CanMoveToNext", typeof(bool), typeof(SkipControl), new PropertyMetadata(true, CanMoveToNextChanged));

        public static readonly DependencyProperty CanMoveToPreviousProperty = DependencyProperty.Register("CanMoveToPrevious", typeof(bool), typeof(SkipControl), new PropertyMetadata(true, CanMoveToPreviousChanged));

        private static void CanMoveToNextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SkipControl).CanMoveToNextChanges.OnNext((bool)e.NewValue);
        }

        private static void CanMoveToPreviousChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SkipControl).CanMoveToPreviousChanges.OnNext((bool)e.NewValue);
        }

        public bool CanMoveToNext
        {
            get { return (bool)GetValue(CanMoveToNextProperty); }
            set { SetValue(CanMoveToNextProperty, value); }
        }

        public bool CanMoveToPrevious
        {
            get { return (bool)GetValue(CanMoveToPreviousProperty); }
            set { SetValue(CanMoveToPreviousProperty, value); }
        }

        //public object Output
        //{
        //    get { return (object)GetValue(OutputProperty); }
        //    set { SetValue(OutputProperty, value); }
        //}

        static SkipControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SkipControl), new FrameworkPropertyMetadata(typeof(SkipControl)));
        }

        private ISubject<bool> CanMoveToNextChanges = new Subject<bool>();
        private ISubject<bool> CanMoveToPreviousChanges = new Subject<bool>();

        public SkipControl()
        {
            var nextCommand = ReactiveCommand.Create<Unit>(a => CanMoveToNextChanges.OnNext(true));
            var previousCommand = ReactiveCommand.Create<Unit>(a => CanMoveToPreviousChanges.OnNext(true));
            this.SetValue(NextCommandProperty, nextCommand);
            this.SetValue(PreviousCommandProperty, previousCommand);

            (nextCommand as ReactiveCommand<Unit, Unit>).Select(_ => Utility.Enums.Direction.Forward).Merge(previousCommand.Select(_ => Utility.Enums.Direction.Backward)).Subscribe(direction =>
             {
                 this.Dispatcher.InvokeAsync(() => RaiseSkipEvent(direction), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
             });
        }

        public static readonly RoutedEvent SkipEvent = EventManager.RegisterRoutedEvent("Skip", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SkipControl));

        public event RoutedEventHandler Skip
        {
            add { AddHandler(SkipEvent, value); }
            remove { RemoveHandler(SkipEvent, value); }
        }

        private void RaiseSkipEvent(Utility.Enums.Direction direction)
        {
            SkipRoutedEventArgs newEventArgs = new SkipRoutedEventArgs(SkipControl.SkipEvent) { Direction = direction };
            RaiseEvent(newEventArgs);
        }

        public class SkipRoutedEventArgs : RoutedEventArgs
        {
            public Utility.Enums.Direction Direction { get; set; }

            public SkipRoutedEventArgs(RoutedEvent @event) : base(@event)
            {
            }
        }
    }
}
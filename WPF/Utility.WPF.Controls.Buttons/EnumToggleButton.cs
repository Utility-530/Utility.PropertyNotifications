using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using ReactiveUI;

namespace Utility.WPF.Controls.Buttons
{
    public class EnumToggleButton : ToggleButton
    {
        private readonly ISubject<Array> enumSubject = new Subject<Array>();

        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register("Enum", typeof(Type), typeof(EnumToggleButton), new PropertyMetadata(typeof(Switch), null, EnumCoerce));
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EnumToggleButton));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(EnumToggleButton));

        static EnumToggleButton()
        {
            //  DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(typeof(EnumToggleButton)));
            IsCheckedProperty.OverrideMetadata(typeof(EnumToggleButton), new FrameworkPropertyMetadata(false, null, IsCheckedChanged));
        }

        public EnumToggleButton()
        {
            enumSubject
                .WhereNotNull()
                .Subscribe(c =>
                {
                    if (c.Length > 0)
                    {
                        this.Content = c.Cast<Enum>().First();
                        this.UnCheckedContent = c.Cast<Enum>().Last();
                    }
                });

            base.Checked += EnumToggleButton_Checked;
            base.Unchecked += EnumToggleButton_Unchecked;

            void EnumToggleButton_Unchecked(object sender, RoutedEventArgs e)
            {
                RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, false));
            }

            void EnumToggleButton_Checked(object sender, RoutedEventArgs e)
            {
                RaiseEvent(new EnumEventArgs(EnumToggleButton.SelectedEvent, true));
            }
        }

        public Type Enum
        {
            get { return (Type)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, Enum); }
        }

        public object Output
        {
            get { return (object)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        private static object? IsCheckedChanged(DependencyObject d, object baseValue)
        {
            if (d is not EnumToggleButton tb)
                return null;

            var vals = System.Enum.GetValues(tb.Enum);
            if ((bool)baseValue)
                tb.Output = vals.Cast<object>().First();
            else
                tb.Output = vals.Cast<object>().Last();

            return baseValue;
        }

        private static object EnumCoerce(DependencyObject d, object baseValue)
        {
            var vals = System.Enum.GetValues((Type)baseValue);

            if (vals.Length == 2)
            {
                (d as EnumToggleButton).enumSubject.OnNext(vals);
                //(d as ToggleSwitch.ToggleSwitchBase).CheckedContent = vals.Cast<object>().First();
                //(d as ToggleSwitch.ToggleSwitchBase).UncheckedContent = vals.Cast<object>().Last();
            }
            else
            {
                (d as EnumToggleButton).enumSubject.OnNext(new[] { vals.Cast<Enum>().First(), vals.Cast<Enum>().Last() });
            }
            return baseValue;
        }

        // Raise the routed event "selected"
        public class EnumEventArgs : RoutedEventArgs
        {
            public bool IsChecked;

            public EnumEventArgs(RoutedEvent routedEvent, bool isChecked) : base(routedEvent)
            {
                IsChecked = isChecked;
            }
        }

        public enum Switch
        {
            On, Off
        }
    }
}
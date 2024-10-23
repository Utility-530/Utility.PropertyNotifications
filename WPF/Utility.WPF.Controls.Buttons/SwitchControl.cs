using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static Utility.WPF.Controls.Buttons.SwitchControl;
using Evan.Wpf;

namespace Utility.WPF.Controls.Buttons
{
    public class SwitchControl : Control
    {
        public delegate void ToggleEventHandler(object sender, ToggleEventArgs size);

        private readonly ReactiveCommand<object, object> setValueCommand;
        protected ButtonBase EditButton;

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText", typeof(string), typeof(SwitchControl), new PropertyMetadata(null));

        public static readonly DependencyProperty MainProperty = DependencyHelper.Register();

        public static readonly DependencyProperty AlternateProperty = DependencyHelper.Register();

        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register(new PropertyMetadata(true));

        public static readonly DependencyProperty KeyValueProperty = DependencyProperty.Register("KeyValue", typeof(object), typeof(SwitchControl));

        public static readonly DependencyProperty ButtonWidthProperty = DependencyHelper.Register(new PropertyMetadata(120d));

        public static readonly RoutedEvent ButtonToggleEvent = EventManager.RegisterRoutedEvent("ButtonToggle", RoutingStrategy.Bubble, typeof(ToggleEventHandler), typeof(SwitchControl));

        public static readonly DependencyProperty SelectedValueProperty = DependencyHelper.Register();

        static SwitchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchControl), new FrameworkPropertyMetadata(typeof(SwitchControl)));
        }

        public SwitchControl()
        {
            setValueCommand = ReactiveCommand.Create<object, object>(a => a);
            //KeyValue = ValueToKey();
        }

        public override void OnApplyTemplate()
        {
            EditButton = GetTemplateChild("EditButton") as ButtonBase;

            setValueCommand.DistinctUntilChanged().Subscribe(a =>
            {
                Value = Main.Equals(a);
                RaiseToggleEvent(Value ? Main: Alternate, Value);
            });
            base.OnApplyTemplate();
        }

        protected virtual void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            RaiseToggleEvent(ValueToKey(), this.Value);
        }

        public virtual object ValueToKey()
        {
            return Value ? Main : Alternate;
        }

        public virtual bool KeyToValue(object key)
        {
            return KeyToValue(key, Main, Alternate);
        }

        public static bool KeyToValue(object key, object main, object alternative)
        {
            return key.Equals(main) ? true : false;
        }

        protected void Change(bool value)
        {
            if (value)
            {
                SetValueCommand.Execute(SelectedValue = Main);
            }
            if (!value)
            {
                SetValueCommand.Execute(SelectedValue = Alternate);
            }
        }

        public event ToggleEventHandler ButtonToggle
        {
            add => AddHandler(ButtonToggleEvent, value);
            remove => RemoveHandler(ButtonToggleEvent, value);
        }

        protected void RaiseToggleEvent(object key, bool? isChecked)
        {
            RoutedEventArgs newEventArgs = new ToggleEventArgs(ButtonToggleEvent, key, isChecked);
            RaiseEvent(newEventArgs);
        }

        #region properties

        public object Main
        {
            get => GetValue(MainProperty);
            set => SetValue(MainProperty, value);
        }

        public object Alternate
        {
            get => GetValue(AlternateProperty);
            set => SetValue(AlternateProperty, value);
        }

        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public bool Value
        {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        #endregion properties

        public ICommand SetValueCommand => setValueCommand;

        public class ToggleEventArgs : RoutedEventArgs
        {
            public ToggleEventArgs(RoutedEvent routedEvent, object key, bool? isChecked) : base(routedEvent)
            {
                Key = key;
                IsChecked = isChecked;
            }

            public bool? IsChecked { get; }

            public object Key { get; }
        }
    }

    public static class SwitchControlHelper
    {
        public static IObservable<ToggleEventArgs> Toggles(this SwitchControl selector) =>
            from x in Observable.FromEventPattern<ToggleEventHandler, ToggleEventArgs>(a => selector.ButtonToggle += a, a => selector.ButtonToggle -= a)
            select x.EventArgs;
    }
}
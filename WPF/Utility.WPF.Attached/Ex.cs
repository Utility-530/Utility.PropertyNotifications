using System;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

namespace Utility.WPF.Attached
{
    public enum State
    {
        None,
        Ticked,
        Crossed,
        Refreshable,
        Point
    }

    public record AttachedPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e);

    public partial class Ex : DependencyObject, IObservable<AttachedPropertyChange>
    {
        private static ReplaySubject<AttachedPropertyChange> singleReplaySubject = new(1);

        public static readonly DependencyProperty SecurityIdProperty = DependencyProperty.RegisterAttached("SecurityId", typeof(object), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            singleReplaySubject.OnNext(new(d, e));
        }

        public static string GetSecurityId(DependencyObject d)
        {
            return (string)d.GetValue(SecurityIdProperty);
        }

        public static void SetSecurityId(DependencyObject d, object value)
        {
            d.SetValue(SecurityIdProperty, value);
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached("Type", typeof(object), typeof(Ex), new PropertyMetadata(null));

        public static string GetType(DependencyObject d)
        {
            return d.GetValue(TypeProperty).GetType().ToString();
        }

        public static void SetType(DependencyObject d, object value)
        {
            d.SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(object), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        public static string GetKey(DependencyObject d)
        {
            return (string)d.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject d, object value)
        {
            d.SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsReadOnly(DependencyObject d)
        {
            return (bool)d.GetValue(IsReadOnlyProperty);
        }

        public static void SetIsReadOnly(DependencyObject d, object value)
        {
            d.SetValue(IsReadOnlyProperty, (bool)value);
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached("State", typeof(State), typeof(Ex), new PropertyMetadata(State.None, PropertyChanged));

        public static State GetState(DependencyObject d)
        {
            return (State)d.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject d, object value)
        {
            d.SetValue(StateProperty, (State)value);
        }

        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsPressed(DependencyObject d)
        {
            return (bool)d.GetValue(IsPressedProperty);
        }

        public static void SetIsPressed(DependencyObject d, object value)
        {
            d.SetValue(IsPressedProperty, (bool)value);
        }

        public static readonly DependencyProperty IsMouseOverProperty = DependencyProperty.RegisterAttached("IsMouseOver", typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsMouseOver(DependencyObject d)
        {
            return (bool)d.GetValue(IsMouseOverProperty);
        }

        public static void SetIsMouseOver(DependencyObject d, object value)
        {
            d.SetValue(IsMouseOverProperty, (bool)value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject d, object value)
        {
            d.SetValue(CommandProperty, (ICommand)value);
        }

        public const string IsChecked = "IsChecked";
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached(IsChecked, typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetIsChecked(DependencyObject d)
        {
            return (bool)d.GetValue(IsCheckedProperty);
        }

        public static void SetIsChecked(DependencyObject d, object value)
        {
            d.SetValue(IsCheckedProperty, (bool)value);
        }

        public IDisposable Subscribe(IObserver<AttachedPropertyChange> observer)
        {
            return singleReplaySubject.Subscribe(observer);
        }

        public static IObservable<T> Observable<T>(Predicate<DependencyObject> predicateObject, Predicate<DependencyProperty> predicateProperty)
        {
            return (from a in singleReplaySubject
                    where predicateObject(a.d)
                    where predicateProperty(a.e.Property)
                    select a.e.NewValue)
                .Cast<T>();
        }

        public static IObservable<bool> Checked(Predicate<DependencyObject> predicateObject) =>
            Observable<bool>(predicateObject, a => a == IsCheckedProperty);



        public const string Reverse = "Reverse";
        public static readonly DependencyProperty ReverseProperty = DependencyProperty.RegisterAttached(Reverse, typeof(bool), typeof(Ex), new PropertyMetadata(false, PropertyChanged));

        public static bool GetReverse(DependencyObject d)
        {
            return (bool)d.GetValue(ReverseProperty);
        }

        public static void SetReverse(DependencyObject d, object value)
        {
            d.SetValue(ReverseProperty, (bool)value);
        }


        public const string ToggleStyle = nameof(ToggleStyle);
        public static readonly DependencyProperty ToggleStyleProperty = DependencyProperty.RegisterAttached(ToggleStyle, typeof(Style), typeof(Ex), new PropertyMetadata(null, PropertyChanged));

        public static Style GetToggleStyle(DependencyObject d)
        {
            return (Style)d.GetValue(ToggleStyleProperty);
        }

        public static void SetToggleStyle(DependencyObject d, object value)
        {
            d.SetValue(ToggleStyleProperty, (Style)value);
        }


    }
}
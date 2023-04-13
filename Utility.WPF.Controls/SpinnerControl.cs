// Copyright 2012 lapthorn.net.
//
// This software is provided "as is" without a warranty of any kind. All
// express or implied conditions, representations and warranties, including
// any implied warranty of merchantability, fitness for a particular purpose
// or non-infringement, are hereby excluded. lapthorn.net and its licensors
// shall not be liable for any damages suffered by licensee as a result of
// using the software. In no event will lapthorn.net be liable for any
// lost revenue, profit or data, or for direct, indirect, special,
// consequential, incidental or punitive damages, however caused and regardless
// of the theory of liability, arising out of the use of or inability to use
// software, even if lapthorn.net has been advised of the possibility of
// such damages.

using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SpinnerControl.xaml
    /// </summary>
    public class SpinnerControl : UserControl
    {
        private static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(SpinnerControl), new PropertyMetadata(DefaultMinimumValue));
        private static readonly DependencyPropertyKey FormattedValuePropertyKey = DependencyProperty.RegisterAttachedReadOnly("FormattedValue", typeof(string), typeof(SpinnerControl), new PropertyMetadata(DefaultValue.ToString()));
        private static readonly DependencyProperty FormattedValueProperty = FormattedValuePropertyKey.DependencyProperty;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(SpinnerControl), new FrameworkPropertyMetadata(DefaultValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));
        private static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(SpinnerControl), new PropertyMetadata(DefaultMaximumValue));
        private static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(SpinnerControl), new PropertyMetadata(DefaultDecimalPlaces));
        private static readonly DependencyProperty ChangeProperty = DependencyProperty.Register("Change", typeof(decimal), typeof(SpinnerControl), new PropertyMetadata(DefaultChange));
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat", typeof(string), typeof(SpinnerControl), new PropertyMetadata("f"));

        public static RoutedCommand IncreaseCommand { get; set; } = new RoutedCommand("IncreaseCommand", typeof(SpinnerControl));
        public static RoutedCommand DecreaseCommand { get; set; } = new RoutedCommand("DecreaseCommand", typeof(SpinnerControl));

        private const decimal DefaultMinimumValue = 0, DefaultMaximumValue = 100, DefaultValue = DefaultMinimumValue, DefaultChange = 1;

        private const int DefaultDecimalPlaces = 0;

        /// <summary>
        /// The ValueChangedEvent, raised if  the value changes.
        /// </summary>
        private static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(SpinnerControl));

        static SpinnerControl()
        {
            InitializeCommands();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinnerControl), new FrameworkPropertyMetadata(typeof(SpinnerControl)));
        }

        public SpinnerControl()
        {
        }

        #region properties

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public decimal Change
        {
            get { return (decimal)GetValue(ChangeProperty); }
            set { SetValue(ChangeProperty, value); }
        }

        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion properties

        protected static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SpinnerControl control = sender as SpinnerControl;

            if (control != null)
            {
                control.OnIncrease();
            }
        }

        protected void OnIncrease()
        {
            //  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
            //  for why we do this.
            Value = LimitValueByBounds(Value + Change, this);
        }

        protected static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is SpinnerControl control)
            {
                control.OnDecrease();
            }
        }

        protected void OnDecrease()
        {
            //  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
            //  for why we do this.
            Value = LimitValueByBounds(Value - Change, this);
        }

        /// <summary>
        /// Returns the formatted version of the value, with the specified
        /// number of DecimalPlaces.
        /// </summary>
        public string FormattedValue => (string)GetValue(FormattedValueProperty);

        /// <summary>
        /// Update the formatted value.
        /// </summary>
        /// <param name="newValue"></param>
        protected void UpdateFormattedValue(decimal newValue)
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo() { NumberDecimalDigits = DecimalPlaces };
            //  use fixed point, and the built-in NumberFormatInfo
            //  implementation of IFormatProvider
            var formattedValue = newValue.ToString(StringFormat, numberFormatInfo);

            //  Set the value of the FormattedValue property via its property key
            SetValue(FormattedValuePropertyKey, formattedValue);
        }

        /// <summary>
        /// If the value changes, update the text box that displays the Value
        /// property to the consumer.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is SpinnerControl control)
            {
                var newValue = (decimal)args.NewValue;
                var oldValue = (decimal)args.OldValue;

                control.UpdateFormattedValue(newValue);

                RoutedPropertyChangedEventArgs<decimal> e =
                    new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent);

                control.RaiseEvent(e);
            }
        }

        private static decimal LimitValueByBounds(decimal newValue, SpinnerControl control)
        {
            newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
            //  then ensure the number of decimal places is correct.
            newValue = decimal.Round(newValue, control.DecimalPlaces);
            return newValue;
        }

        private static object CoerceValue(DependencyObject obj, object value)
        {
            decimal newValue = (decimal)value;

            if (obj is SpinnerControl control)
            {
                //  ensure that the value stays within the bounds of the minimum and
                //  maximum values that we define.
                newValue = LimitValueByBounds(newValue, control);
            }

            return newValue;
        }

        /// <summary>
        /// Since we're using RoutedCommands for the up/down buttons, we need to
        /// register them with the command manager so we can tie the events
        /// to callbacks in the control.
        /// </summary>
        private static void InitializeCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(SpinnerControl), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(typeof(SpinnerControl), new CommandBinding(DecreaseCommand, OnDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Right)));
            CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
            CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Left)));
        }
    }

    public static class SpinnerControlExtensions
    {
        public static IObservable<double> SelectValueChanges(SpinnerControl spinnerControl)
        {
            return Observable.FromEventPattern<
                RoutedPropertyChangedEventHandler<decimal>,
                RoutedPropertyChangedEventArgs<double>>(a => spinnerControl.ValueChanged += a, a => spinnerControl.ValueChanged += a)
                .Select(a => a.EventArgs.NewValue);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leepfrog.WpfFramework.Controls
{
    public class DatePicker : Control, INotifyPropertyChanged
    {
        static DatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));
        }




        public Brush ForegroundInvalid
        {
            get { return (Brush)GetValue(ForegroundInvalidProperty); }
            set { SetValue(ForegroundInvalidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundInvalid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundInvalidProperty =
            DependencyProperty.Register("ForegroundInvalid", typeof(Brush), typeof(DatePicker), new PropertyMetadata(new SolidColorBrush(Colors.Red)));



        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime?), typeof(DatePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Value_PropertyChanged));

        private static void Value_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (d as DatePicker);
            var handler = datePicker?.PropertyChanged;
            if (handler != null)
            {
                var newValue = (DateTime?)e.NewValue;
                if (newValue == null)
                {
                    if (!datePicker._preserveValues)
                    {
                        datePicker._daySelected = 0;
                        datePicker._monthSelected = 0;
                        datePicker._yearSelected = 0;
                    }
                }
                else
                {
                    datePicker._daySelected = newValue.Value.Day;
                    datePicker._monthSelected = newValue.Value.Month;
                    datePicker._yearSelected = newValue.Value.Year;
                }
                handler(d, new PropertyChangedEventArgs(nameof(Day)));
                handler(d, new PropertyChangedEventArgs(nameof(Month)));
                handler(d, new PropertyChangedEventArgs(nameof(MonthDisplay)));
                handler(d, new PropertyChangedEventArgs(nameof(Year)));
                handler(d, new PropertyChangedEventArgs(nameof(IsValid)));
            }
        }

        public DateTime? Maximum
        {
            get { return (DateTime?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(DateTime?), typeof(DatePicker), new PropertyMetadata(DateTime.Now, Maximum_Changed));

        private static void Maximum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // check current value fits in the new range
            if (d is DatePicker picker)
            {
                var currentValue = picker.Value;
                var newMaximum = (DateTime?)e.NewValue;
                if (
                    (newMaximum != null)
                 && (currentValue != null)
                 && (currentValue.Value < newMaximum.Value)
                   )
                {
                    picker.Value = newMaximum.Value;
                }
            }
        }
        public DateTime? Minimum
        {
            get { return (DateTime?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(DateTime?), typeof(DatePicker), new PropertyMetadata(DateTime.Now.AddYears(-120), Minimum_Changed));

        private static void Minimum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // check current value fits in the new range
            if (d is DatePicker picker)
            {
                var currentValue = picker.Value;
                var newMinimum = (DateTime?)e.NewValue;
                if (
                    (newMinimum != null)
                 && (currentValue != null)
                 && (currentValue.Value > newMinimum.Value)
                   )
                {
                    picker.Value = newMinimum.Value;
                }
            }
        }

        private int _yearSelected;
        public int Year
        {
            get => (_yearSelected == 0) ? (Value?.Year ?? 0) : _yearSelected;
            set
            {
                if ((Day == 0) || (Month == 0))
                {
                    value = _defaultYear;
                }
                _yearSelected = value;
                buildDate();
            }
        }
        private int _monthSelected;
        public int Month
        {
            get => (_monthSelected == 0) ? (Value?.Month ?? 0) : _monthSelected;
            set
            {
                _monthSelected = value;
                buildDate();
            }
        }
        private int _daySelected;
        public int Day
        {
            get => (_daySelected == 0) ? (Value?.Day ?? 0) : _daySelected;
            set
            {
                _daySelected = value;
                buildDate();
            }
        }
        public string MonthDisplay => (_monthSelected == 0 ? null : new DateTime(1, _monthSelected, 1).ToString("MMM"));

        public int YearMin => Minimum?.Year ?? 1900;
        public int YearMax => Maximum?.Year ?? DateTime.Now.Year;

        private void buildDate()
        {
            IsValid = null;
            if (
                (_monthSelected != 0)
             && (_daySelected != 0)
             //&& (_yearSelected >= YearMin)
             //&& (_yearSelected <= YearMax)
                )
            {
                if (_yearSelected == 0)
                {
                    Year = _defaultYear;
                    // will cause reentry, so quit now
                    return;
                }
                try
                {
                    Value = new DateTime(_yearSelected, _monthSelected, _daySelected);
                    IsValid = true;
                    return;
                }
                catch (ArgumentOutOfRangeException)
                {
                    IsValid = false;
                }
            }
            _preserveValues = true;
            Value = null;
            _preserveValues = false;
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(nameof(Day)));
                handler(this, new PropertyChangedEventArgs(nameof(Month)));
                handler(this, new PropertyChangedEventArgs(nameof(MonthDisplay)));
                handler(this, new PropertyChangedEventArgs(nameof(Year)));
                handler(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsValid { get; private set; }

        private bool _preserveValues;
        private int _defaultYear => DateTime.Now.Year - 21;

        public string DateFormat
        {
            get { return (string)GetValue(DateFormatProperty); }
            set { SetValue(DateFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateFormatProperty =
            DependencyProperty.Register("DateFormat", typeof(string), typeof(DatePicker), new PropertyMetadata("dmy",DateFormat_Changed));

        private static void DateFormat_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (d as DatePicker);
            var newValue = e.NewValue as string;
            if (newValue != null)
            {
                // get the first occurence of each character except space
                var format = string.Join("", ($"{newValue}DMY").ToUpper().Replace(" ", "").Distinct());
                datePicker.ColumnDay = (format.IndexOf('D') *2);
                datePicker.ColumnMonth = (format.IndexOf('M') *2);
                datePicker.ColumnYear = (format.IndexOf('Y') *2);
                var handler = datePicker.PropertyChanged;
                if (handler != null)
                { 
                    handler(datePicker, new PropertyChangedEventArgs(nameof(ColumnDay)));
                    handler(datePicker, new PropertyChangedEventArgs(nameof(ColumnMonth)));
                    handler(datePicker, new PropertyChangedEventArgs(nameof(ColumnYear)));
                }
            }
        }

        public int ColumnDay { get; private set; } = 0;
        public int ColumnMonth { get; private set; } = 2;
        public int ColumnYear { get; private set; } = 4;

    }
}
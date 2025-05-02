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
    public class TimePicker : Control, INotifyPropertyChanged
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public DateTimeOffset? Value
        {
            get { return (DateTimeOffset?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTimeOffset?), typeof(TimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Value_PropertyChanged,Value_Coerce));

        private static object Value_Coerce(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return null;
            }
            var picker = (d as TimePicker);
            if (picker == null)
            {
                return baseValue;
            }
            var time = (DateTimeOffset)baseValue;
            var minutes = time.Minute;
            foreach (var possibleMinute in picker.PossibleMinutes)
            {
                if (minutes >= possibleMinute)
                {
                    time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, possibleMinute, 0, time.Offset);
                }
            }
            return time;
        }

        private static void Value_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var handler = ( d as TimePicker ).PropertyChanged;
            if ( handler != null )
            {
                handler(d, new PropertyChangedEventArgs("Hour"));
                handler(d, new PropertyChangedEventArgs("Hour12"));
                handler(d, new PropertyChangedEventArgs("AmPm"));
                handler(d, new PropertyChangedEventArgs("Use24HourClock"));
                handler(d, new PropertyChangedEventArgs("Minute"));
                handler(d, new PropertyChangedEventArgs("Timezone"));
                handler(d, new PropertyChangedEventArgs("IsTimezoneRelevant"));
            }
        }



        public bool Use24HourClock
        {
            get { return (bool)GetValue(Use24HourClockProperty); }
            set { SetValue(Use24HourClockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Use24HourClock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Use24HourClockProperty =
            DependencyProperty.Register("Use24HourClock", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        public int? Hour
        {
            get { return Value?.Hour; }
            set
            {
                var oldHour = Value?.Hour ?? 0;
                var newHour = value.Value;
                if ( Value == null )
                { 
                    // default to now
                    Value = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day, newHour, 0, 0, TimeZoneInfo.Local.BaseUtcOffset); 
                }
                else if ( oldHour != newHour )
                {
                    // fancy logic to say 0-4am should increase the date, and after 4am should revert back to the previous day: 
                    //  if (old hour between 0-4) != (new hour between 0-4) then we need to change something
                    //  if we just went over 4am, then decrease the date
                    var newValue = Value;
                    if ((oldHour < 4) != (newHour < 4))
                    {
                        if (oldHour < 4)
                        {
                            // we were before 4, now we're after 4, so let's go back a day, then add the change in hours...
                            newValue = newValue?.AddDays(-1);
                        }
                        else
                        {
                            // we were after 4, now we're before 4, so let's go forward a day, then add the change in hours...
                            newValue = newValue?.AddDays(1);
                        }
                    }
                    Value = newValue?.AddHours(newHour - oldHour); 
                }
            }
        }

        public int? Hour12
        {
            get { 
                switch (Value?.Hour)
                {
                    case null: return null;
                    case 0: return 12;
                    case 12: return 12;
                    default: return (Value.Value.Hour % 12);
                }
            }
        }
        public string AmPm
        {
            get { 
                if (Value == null)
                {
                    return null;
                }
                if (Value.Value.TimeOfDay == TimeSpan.Zero)
                {
                    return "mdnt";
                }
                else if (Value.Value.TimeOfDay == TimeSpan.FromHours(12))
                {
                    return "noon";
                }
                else if (Value.Value.TimeOfDay < TimeSpan.FromHours(12))
                {
                    return "am";
                }
                else 
                {
                    return "pm";
                }
            }
        }

        public int? Minute
        {
            get { return Value?.Minute; }
            set
            {
                if ( Value == null )
                { Value = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day, DateTimeOffset.Now.Hour, value.Value, 0, TimeZoneInfo.Local.BaseUtcOffset); }
                else if ( Value?.Minute != value )
                { Value = Value?.AddMinutes(value.Value - Minute.Value); }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<int> _possibleHours = Enumerable.Range(0, 24).ToList();
        public List<int> PossibleHours => _possibleHours;
        private List<int> _possibleMinutes = Enumerable.Range(0, 6).Select(m => m * 10).ToList();
        public List<int> PossibleMinutes => _possibleMinutes;
    }
}

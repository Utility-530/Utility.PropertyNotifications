using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.Models.Templates
{

    public class ValueChangeConverter : IValueConverter
    {
        private static Kaos.Collections.RankedDictionary<DateTime, decimal> _previousValues = new(/*ComparerHelper.Create<DateTime>((a, b) => a.CompareTo(b))*/);


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            // Try to parse the current value
            if (!decimal.TryParse(value.ToString(), out decimal currentValue))
                return string.Empty;

            // Use the value itself as a key (or use DataContext if available)
            var key = DateTime.Now;

            // Check if we have a previous value
            if (_previousValues.Count != 0)
            {
                var previousValue = _previousValues.Last().Value;
                _previousValues[key] = currentValue;
                // Update the tracking
                if (previousValue != currentValue)
                {
                    if (currentValue > previousValue)
                    {
                        return ChangeType.Increase;
                    }
                    else if (currentValue < previousValue)
                    {
                        return ChangeType.Decrease;
                    }
                }
            }
            _previousValues[key] = currentValue;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum ChangeType
    {
        None,
        Increase,
        Decrease
    }

}

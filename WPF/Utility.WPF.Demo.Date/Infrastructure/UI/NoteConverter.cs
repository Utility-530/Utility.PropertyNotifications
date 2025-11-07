using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Demo.Date.Infrastructure.Repository;

namespace Utility.WPF.Demo.Date
{
    public class NoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime dateTime)
                return DependencyProperty.UnsetValue;

            return NotesRepository.Instance.FindMostRecent(dateTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.ViewModels;
using Utility.WPF.Demo.Date.Infrastructure.Repository;

namespace Utility.WPF.Demo.Date
{
    public class NotesConverter : IValueConverter
    {
        //private DateTime date;

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime date)
                return DependencyProperty.UnsetValue;

            if (App.CurrentDate != default)
                NotesRepository.Instance.Save(App.CurrentDate).WaitAsync(TimeSpan.FromSeconds(10));

            App.CurrentDate = date;

            var notesViewModel = new NotesViewModel("Notes");

            var _ = NotesRepository.Instance.FindAllAsync(date)
                .ToObservable()
                .Subscribe(notes =>
                {
                    notesViewModel.Replace(notes.ToArray(), notes.Last());
                });

            return notesViewModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
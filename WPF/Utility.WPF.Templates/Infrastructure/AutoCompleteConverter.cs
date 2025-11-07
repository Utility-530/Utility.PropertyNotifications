using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Subjects;
using System.Windows.Data;
using AutoCompleteTextBox.Editors;
using MintPlayer.ObservableCollection;
using Utility.Changes;
using Utility.Helpers;
using Type = System.Type;

namespace Utility.WPF.Templates
{
    public record SuggestionPrompt(string Filter, object Value);

    public class ComboSuggestionProvider : IComboSuggestionProvider, IObservable<SuggestionPrompt>
    {
        private ReplaySubject<SuggestionPrompt> replaySubject = new();

        private ObservableCollection<string> suggestions = new();

        public ComboSuggestionProvider(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public IEnumerable GetFullCollection()
        {
            return suggestions;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            replaySubject.OnNext(new(filter, Value));
            return suggestions;
        }

        public IDisposable Subscribe(IObserver<SuggestionPrompt> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }

    public class AutoCompleteConverter : IValueConverter, IObservable<SuggestionPrompt>, IObserver<Change>
    {
        private readonly Dictionary<object, ComboSuggestionProvider> dictionary = new();
        private ReplaySubject<SuggestionPrompt> replaySubject = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return dictionary.Get(value, (a) =>
            {
                var provider = new ComboSuggestionProvider(a);
                provider.Subscribe(replaySubject);
                return provider;
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static AutoCompleteConverter Instance { get; } = new AutoCompleteConverter();

        public IDisposable Subscribe(IObserver<SuggestionPrompt> observer)
        {
            return replaySubject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Change value)
        {
        }
    }
}
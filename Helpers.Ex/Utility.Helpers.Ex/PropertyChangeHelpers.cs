using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Fasterflect;
using Utility.Changes;
using Utility.Helpers;

namespace Utility.Helpers.Ex
{
    public static class PropertyChangeHelpers
    {
        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<PropertyChange> Changes<T>(this T source, bool startWithSource = false)
            where T : INotifyPropertyChanged
        {
            var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Select(e => new PropertyChange(source, e.EventArgs.PropertyName, source.GetPropertyValue(e.EventArgs.PropertyName)));
            return startWithSource ? obs.StartWith(new PropertyChange(source, default, default)) : obs;
        }

        public static IObservable<R?> Changes<T, R>(this T source, string name) where R : class
            where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                               .Where(a => a.EventArgs.PropertyName == name)
                            .Select(_ => source.GetPropertyRefValue<R>(xx));
        }

        public static IObservable<Tuple<T, R?>> OnPropertyChangeWithSource<T, R>(this T source, string name) where R : class
    where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                                .Where(a => a.EventArgs.PropertyName == name)
                                .Select(_ => Tuple.Create(source, source.GetPropertyRefValue<R>(xx)));
        }

        public static IObservable<Tuple<T, R?>> OnPropertyChangeWithSourceValue<T, R>(this T source, string name) where R : struct
where T : INotifyPropertyChanged
        {
            var xx = typeof(T).GetProperty(name);
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                                .Where(a => a.EventArgs.PropertyName == name)
                                .Select(_ => Tuple.Create(source, source.GetPropertyValue<R>(xx)));
        }
    }
}
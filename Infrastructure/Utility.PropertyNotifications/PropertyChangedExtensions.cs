using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Utility.PropertyNotifications
{
    public static class PropertyChangedExtensions
    {
        public static void RaisePropertyChanged<T, P>(this T sender, Expression<Func<T, P>> propertyExpression) where T : INotifyPropertyChanged
        {
            Raise(typeof(T), sender, (propertyExpression.Body as MemberExpression).Member.Name);
        }

        public static void RaisePropertyChanged(this INotifyPropertyChanged sender, [CallerMemberName] string prop = null)
        {
            Raise(sender.GetType(), sender, prop);
        }

        private static void Raise(Type targetType, INotifyPropertyChanged sender, string propName)
        {
            ((PropertyChangedEventHandler?)field(targetType).GetValue(sender))?.Invoke(sender, new PropertyChangedEventArgs(propName));

            static FieldInfo field(Type type)
            {
                return type.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic) ??
                    field(type.BaseType ?? throw new Exception("ubn 43"));
            }
        }


        private class PropertyObservable<T> : IObservable<T>
        {
            private readonly INotifyPropertyChanged _target;
            private readonly PropertyInfo? _info;
            private readonly bool _includeNulls;
            private readonly bool includeInitialValue;
            private const string constructor = ".ctor";
            public PropertyObservable(INotifyPropertyChanged target, PropertyInfo? info = null, bool includeNulls = false, bool includeInitialValue = true)
            {
                _target = target;
                _info = info;
                _includeNulls = includeNulls;
                this.includeInitialValue = includeInitialValue;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyChanged _target;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;
                private readonly bool _includeNulls;
                private Dictionary<string, PropertyInfo> dictionary = new();

                public Subscription(INotifyPropertyChanged target, PropertyInfo info, IObserver<T> observer, bool includeNulls, bool includeInitialValue)
                {
                    _target = target;
                    _info = info;
                    _observer = observer;
                    _includeNulls = includeNulls;
                    _target.PropertyChanged += OnPropertyChanged;
                    if (includeInitialValue)
                        raiseChange();
                }

                private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
                {
                    if (_info == null && e.PropertyName is string pName)
                    {
                        if (pName == constructor)
                        {
                            _observer.OnNext(default);
                        }
                        else
                        {
                            if (dictionary.ContainsKey(pName) == false)
                            {
                                dictionary[pName] = _target.GetType().GetProperty(pName);
                            }
                            var value = (T)dictionary[pName].GetValue(_target);
                            _observer.OnNext(value);
                        }
                    }
                    else if (e.PropertyName == _info.Name)
                        raiseChange();
                }

                public void Dispose()
                {
                    _target.PropertyChanged -= OnPropertyChanged;
                    _observer.OnCompleted();
                }

                private void raiseChange()
                {
                    var value = (T?)_info?.GetValue(_target);
                    if (value != null || _includeNulls)
                        _observer.OnNext(value);
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription(_target, _info, observer, _includeNulls, includeInitialValue);
            }
        }

        private class PropertyObservable : IObservable<PropertyChangedEventArgs>
        {
            private readonly INotifyPropertyChanged _target;
            private readonly PropertyInfo? _info;

            public PropertyObservable(INotifyPropertyChanged target, PropertyInfo? info = null)
            {
                _target = target;
                _info = info;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyChanged _target;
                private readonly PropertyInfo _info;
                private readonly IObserver<PropertyChangedEventArgs> _observer;

                public Subscription(INotifyPropertyChanged target, PropertyInfo info, IObserver<PropertyChangedEventArgs> observer)
                {
                    _target = target;
                    _info = info;
                    _observer = observer;
                    _target.PropertyChanged += OnPropertyChanged;

                }

                private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
                {
                    if (_info == null && e.PropertyName is string pName)
                    {
                        _observer.OnNext(e);
                    }
                    else if (e.PropertyName == _info.Name)
                        _observer.OnNext(e);
                }

                public void Dispose()
                {
                    _target.PropertyChanged -= OnPropertyChanged;
                    _observer.OnCompleted();
                }


            }

            public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
            {
                return new Subscription(_target, _info, observer);
            }
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model,
            Expression<Func<TModel, TRes>> expr, bool includeNulls = false, bool includeInitialValue = true) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TRes>(model, prop, includeNulls, includeInitialValue);
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model, bool includeNulls = false, bool includeInitialValue = true) where TModel : INotifyPropertyChanged
        {
            return new PropertyObservable<TRes>(model, null, includeNulls, includeInitialValue);
        }

        public static IObservable<PropertyChangedEventArgs> WithChanges<TModel>(this TModel model, Expression<Func<TModel, object>> expr) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable(model, prop);
        }

        public static IObservable<PropertyChangedEventArgs> WithChanges<TModel>(this TModel model) where TModel : INotifyPropertyChanged
        {
            return new PropertyObservable(model, null);
        }
    }
}

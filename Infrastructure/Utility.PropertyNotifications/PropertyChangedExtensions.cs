using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utility.Helpers.Reflection;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyNotifications
{
    public record PropertyChange(object Source, object Value, object PreviousValue, string Name);

    public static class PropertyChangedExtensions
    {
        static Dictionary<Type, Func<object, PropertyChangedEventHandler>> dictionary = new();

        public static void RaisePropertyChanged<T, P>(this T sender, Expression<Func<T, P>> propertyExpression) where T : INotifyPropertyChanged
        {
            Raise(sender, (propertyExpression.Body as MemberExpression).Member.Name, typeof(T));
        }

        public static void RaisePropertyChanged(this INotifyPropertyChanged sender, [CallerMemberName] string? prop = null)
        {
            Raise(sender, prop);
        }

        private static void Raise(INotifyPropertyChanged sender, string propName, Type? targetType = null, bool cache = true)
        {
            PropertyChangedEventHandler? handler = null;
            targetType ??= sender.GetType();
            if (cache == false)
                handler = ((PropertyChangedEventHandler?)field(targetType).GetValue(sender));
            else
                handler = dictionary.Get(targetType, t => field(targetType).ToGetter<PropertyChangedEventHandler>()).Invoke(sender);

            handler.Invoke(sender, new PropertyChangedEventArgs(propName));
            static FieldInfo field(Type type)
            {
                return type.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic) ??
                    field(type.BaseType ?? throw new Exception("ubn 43"));
            }
        }


        private class PropertyObservable<TModel, T> : IObservable<T>, IGetReference where TModel : INotifyPropertyChanged
        {
            private readonly TModel _target;
            private readonly PropertyInfo? _info;
            private readonly bool _includeNulls;
            private readonly bool includeInitialValue;
            private const string constructor = ".ctor";
            public PropertyObservable(TModel target, PropertyInfo? info = null, bool includeNulls = false, bool includeInitialValue = true)
            {
                _target = target;
                _info = info;
                _includeNulls = includeNulls;
                this.includeInitialValue = includeInitialValue;
            }

            public object Reference => _target;

            private class Subscription : IDisposable
            {
                private readonly TModel _target;
                private readonly Func<TModel, T> func;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;
                private readonly bool _includeNulls;
                private Dictionary<string, Func<object, T>> dictionary = new();

                public Subscription(TModel target, PropertyInfo? info, IObserver<T> observer, bool includeNulls, bool includeInitialValue)
                {
                    _target = target;
                    _info = info;
                    this.func = info?.ToGetter<TModel, T>();
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
                                var property = _target.GetType().GetProperty(pName);
                                if (property.PropertyType == typeof(T))
                                    dictionary[pName] = property.ToGetter<T>();
                            }
                            if (dictionary.ContainsKey(pName))
                            {
                                var value = dictionary[pName].Invoke(_target);
                                _observer.OnNext(value);
                            }
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
                    var value = func.Invoke(_target);
                    if (value != null || _includeNulls)
                        _observer.OnNext(value);
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription(_target, _info, observer, _includeNulls, includeInitialValue);
            }
        }

        private class PropertyObservable : IObservable<PropertyChange?>, IGetReference
        {
            private readonly INotifyPropertyChanged _target;
            private readonly PropertyInfo? _info;

            public PropertyObservable(INotifyPropertyChanged target, PropertyInfo? info = null)
            {
                _target = target;
                _info = info;
            }

            public object Reference => _target;

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyChanged _target;
                private readonly PropertyInfo? _info;
                private readonly Func<object, object>? getter;
                private object? value;
                private readonly IObserver<PropertyChange?> _observer;
                private Dictionary<string, Func<object, object>> dictionary = new();
                private const string constructor = ".ctor";

                public Subscription(INotifyPropertyChanged target, PropertyInfo? info, IObserver<PropertyChange?> observer)
                {
                    _target = target;
                    _info = info;
                    getter = info?.ToGetter<object>();
                    value = getter?.Invoke(_target);
                    _observer = observer;
                    _target.PropertyChanged += OnPropertyChanged;

                }

                private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
                {
                    if (_info == null && e.PropertyName is string pName)
                    {
                        if (pName == constructor)
                        {
                            _observer.OnNext(null);
                        }
                        else
                        {
                            var getter = dictionary.Get(pName, a => _target.GetType().GetProperty(pName).ToGetter<object>());
                            {
                                var previousValue = value;
                                value = getter.Invoke(_target);
                                _observer.OnNext(new PropertyChange(_target, value, previousValue, pName));
                            }
                        }
                    }
                    else if (e.PropertyName == _info?.Name)
                    {
                        var previousValue = value;
                        _observer.OnNext(new PropertyChange(_target, value = getter?.Invoke(_target), previousValue, e.PropertyName));
                    }
                }

                public void Dispose()
                {
                    _target.PropertyChanged -= OnPropertyChanged;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyChange?> observer)
            {
                return new Subscription(_target, _info, observer);
            }
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model,
            Expression<Func<TModel, TRes>>? expr = null, bool includeNulls = false, bool includeInitialValue = true) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TModel, TRes>(model, prop, includeNulls, includeInitialValue);
        }

        public static IObservable<PropertyChange> WhenChanged<TModel>(this TModel model, Expression<Func<TModel, object>>? expr = null) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var body = l?.Body;
            if (body is UnaryExpression unary)
            {
                body = unary.Operand;
            }
            if (body is MemberExpression memberExpr && memberExpr.Member is PropertyInfo propInfo)
            {
                return new PropertyObservable(model, propInfo);
            }
            else
            {
                //throw new ArgumentException("Expression is not a property access", nameof(expr));
                return new PropertyObservable(model);
            }
        }

        public class Observer<TM, T> : IObserver<T>
        {
            private readonly TM instance;
            private PropertyInfo prop;
            private readonly Action<TM, T> setter;

            public Observer(TM instance, Expression<Func<TM, T>> expr)
            {
                var l = (LambdaExpression)expr;
                var ma = (MemberExpression)l.Body;
                prop = (PropertyInfo)ma.Member;
                setter = prop.ToSetter<TM, T>();
                this.instance = instance;
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(T value)
            {
                setter.Invoke(instance, value);

            }
        }

        public static IDisposable Update<TModel2, TRes>(this IObservable<TRes> propertyObservable, TModel2 model2, Expression<Func<TModel2, TRes>> setExpr)
        {
            return propertyObservable
                .Subscribe(new Observer<TModel2, TRes>(model2, setExpr));
        }
    }
}

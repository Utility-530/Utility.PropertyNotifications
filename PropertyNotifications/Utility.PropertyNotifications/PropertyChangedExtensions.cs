using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyNotifications
{
    public record PropertyChange(object Source, object Value, object PreviousValue, string Name);

    public static class PropertyChangedExtensions
    {
        private static Dictionary<Type, Func<object, PropertyChangedEventHandler>> dictionary = new();

        public static bool RaisePropertyChanged<T, P>(this T sender, Expression<Func<T, P>> propertyExpression) where T : INotifyPropertyChanged
        {
            return Raise(sender, (propertyExpression.Body as MemberExpression).Member.Name, typeof(T));
        }

        public static bool RaisePropertyChanged(this INotifyPropertyChanged sender, [CallerMemberName] string? prop = null)
        {
            return Raise(sender, prop);
        }

        private static bool Raise(INotifyPropertyChanged sender, string propName, Type? targetType = null, bool cache = true)
        {
            if (sender is IRaiseChanges r)
            {
                return r.RaisePropertyChanged(propName);
            }
            PropertyChangedEventHandler? handler = null;
            targetType ??= sender.GetType();
            if (cache == false)
                handler = ((PropertyChangedEventHandler?)field(targetType).GetValue(sender));
            else
                handler = dictionary.Get(targetType, t => field(targetType).ToGetter<PropertyChangedEventHandler>()).Invoke(sender);

            if (handler != null)
            {
                handler.Invoke(sender, new PropertyChangedEventArgs(propName));
                return true;
            }
            return false;
            static FieldInfo field(Type type)
            {
                return type.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic) ??
                    field(type.BaseType ?? throw new Exception("ubn 43"));
            }
        }

        private class PropertyObservable<TModel, T> : IObservable<T>, IPropertyInfo, IGetReference where TModel : INotifyPropertyChanged
        {
            private readonly TModel _target;
            private readonly PropertyInfo? _info;
            private readonly bool _includeNulls;
            private readonly bool includeInitialValue;
            private readonly bool includeDefaultValues;
            private const string constructor = ".ctor";

            public PropertyObservable(TModel target, PropertyInfo? info = null, bool includeNulls = false, bool includeInitialValue = true, bool includeDefaultValues = true)
            {
                _target = target;
                _info = info;
                _includeNulls = includeNulls;
                this.includeInitialValue = includeInitialValue;
                this.includeDefaultValues = includeDefaultValues;
            }

            public object Reference => _target;

            public PropertyInfo? PropertyInfo => _info;

            private class Subscription : IDisposable
            {
                private readonly TModel _target;
                private readonly Func<TModel, T> func;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;
                private readonly bool includeNulls;
                private readonly bool includeInitialValue;
                private readonly bool includeDefaultValues;
                private Dictionary<string, Func<object, T>> dictionary = new();

                public Subscription(TModel target, PropertyInfo? info, IObserver<T> observer, bool includeNulls, bool includeInitialValue, bool includeDefaultValues)
                {
                    _target = target;
                    _info = info;
                    this.func = info?.ToGetter<TModel, T>();
                    _observer = observer;
                    this.includeNulls = includeNulls;
                    this.includeInitialValue = includeInitialValue;
                    this.includeDefaultValues = includeDefaultValues;
                    _target.PropertyChanged += OnPropertyChanged;
                    if(includeInitialValue)
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
                    if (Helpers.Include(value, includeNulls, includeDefaultValues))
                        _observer.OnNext(value);
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription(_target, _info, observer, _includeNulls, includeInitialValue, includeDefaultValues);
            }

            public override string ToString()
            {
                return _target.ToString() + " -> " + _info.Name;
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

            public override string ToString()
            {
                return _target.ToString() + " -> " + _info.Name;
            }
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model,
            Expression<Func<TModel, TRes>>? expr = null, bool includeNulls = false, bool includeInitialValue = true, bool includeDefaultValues = true) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return WithChangesTo<TModel, TRes>(model, prop, includeNulls, includeInitialValue, includeDefaultValues);
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model,
            PropertyInfo? propertyInfo = null, bool includeNulls = false, bool includeInitialValue = true, bool includeDefaultValues = true) where TModel : INotifyPropertyChanged
        {
            return new PropertyObservable<TModel, TRes>(model, propertyInfo, includeNulls, includeInitialValue, includeDefaultValues);
        }

        public static IObservable<object> WithChangesTo<TModel>(this TModel model,
            PropertyInfo? propertyInfo = null, bool includeNulls = false, bool includeInitialValue = true) where TModel : INotifyPropertyChanged
        {
            return new PropertyObservable<TModel, object>(model, propertyInfo, includeNulls, includeInitialValue);
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
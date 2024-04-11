using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Utility.PropertyNotifications
{
    public static class PropertyChangedExtensions
    {
        private class PropertyObservable<T> : IObservable<T>
        {
            private readonly INotifyPropertyChanged _target;
            private readonly PropertyInfo? _info;
            private readonly bool _includeNulls;
            public PropertyObservable(INotifyPropertyChanged target, PropertyInfo? info = null, bool includeNulls = false)
            {
                _target = target;
                _info = info;
                _includeNulls = includeNulls;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyChanged _target;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;
                private readonly bool _includeNulls;
                private Dictionary<string, PropertyInfo> dictionary = new();

                public Subscription(INotifyPropertyChanged target, PropertyInfo info, IObserver<T> observer, bool includeNulls)
                {
                    _target = target;
                    _info = info;
                    _observer = observer;
                    _includeNulls = includeNulls;
                    _target.PropertyChanged += OnPropertyChanged;
                    raiseChange();
                }

                private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
                {
                    if (_info == null && e.PropertyName is string pName)
                    {
                        if (dictionary.ContainsKey(pName) == false)
                        {
                            dictionary[pName] = _target.GetType().GetProperty(pName);
                        }
                        var value = (T?)dictionary[pName].GetValue(_target);
                        _observer.OnNext(value);
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
                return new Subscription(_target, _info, observer, _includeNulls);
            }
        }

        public static IObservable<TRes> WithChangesTo<TModel, TRes>(this TModel model,
            Expression<Func<TModel, TRes>> expr, bool includeNulls = false) where TModel : INotifyPropertyChanged
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TRes>(model, prop, includeNulls);
        }

        public static IObservable<object> WithChanges<TModel>(this TModel model, bool includeNulls = false) where TModel : INotifyPropertyChanged
        {
            return new PropertyObservable<object>(model, null, includeNulls);
        }


        public static IObservable<TRes> WithChanges<TModel, TRes>(this TModel model, bool includeNulls = false) where TModel : INotifyPropertyChanged
        {

            return new PropertyObservable<TRes>(model, null, includeNulls);
        }

        //public static IObservable<TRes> WhenAnyValue<TModel, T1, T2, TRes>(this TModel model,
        //    Expression<Func<TModel, T1>> v1,
        //    Expression<Func<TModel, T2>> v2,
        //    Func<T1, T2, TRes> cb,
        //    bool includeNulls = false) where TModel : INotifyPropertyChanged =>
        //    model.WithChangesTo(v1, includeNulls).CombineLatest(
        //        model.WithChangesTo(v2, includeNulls),
        //        cb);

        //public static IObservable<ValueTuple<T1, T2>> WhenAnyValue<TModel, T1, T2>(this TModel model,
        //    Expression<Func<TModel, T1>> v1,
        //    Expression<Func<TModel, T2>> v2,
        //    bool includeNulls = false) where TModel : INotifyPropertyChanged =>
        //    model.WhenAnyValue(v1, v2, (a1, a2) => (a1, a2), includeNulls);

        //public static IObservable<TRes> WhenAnyValue<TModel, T1, T2, T3, TRes>(this TModel model,
        //    Expression<Func<TModel, T1>> v1,
        //    Expression<Func<TModel, T2>> v2,
        //    Expression<Func<TModel, T3>> v3,
        //    Func<T1, T2, T3, TRes> cb,
        //    bool includeNulls = false) where TModel : INotifyPropertyChanged =>
        //    model.WithChangesTo(v1, includeNulls).CombineLatest(
        //        model.WithChangesTo(v2, includeNulls),
        //        model.WithChangesTo(v3, includeNulls),
        //        cb);

        //public static IObservable<ValueTuple<T1, T2, T3>> WhenAnyValue<TModel, T1, T2, T3>(this TModel model,
        //    Expression<Func<TModel, T1>> v1,
        //    Expression<Func<TModel, T2>> v2,
        //    Expression<Func<TModel, T3>> v3,
        //    bool includeNulls = false) where TModel : INotifyPropertyChanged =>
        //    model.WhenAnyValue(v1, v2, v3, (a1, a2, a3) => (a1, a2, a3), includeNulls);
        //    private void RaisePropertyChanged(object instance, PropertyInfo propertyInfo)
        //    {
        //        if (instance is INotifyPropertyChanged propertyChanged)
        //        {
        //            var args = new PropertyChangedEventArgs(propertyInfo.Name);

        //            if (viewModelToServiceConnections.TryGetValue(propertyInfo.DeclaringType, out var viewModelValue) == false || viewModelValue?.PropertyChangedEventHandler == null)
        //            {
        //                var baseType = propertyInfo.DeclaringType;
        //                FieldInfo fieldInfo = baseType.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic);
        //                while (fieldInfo == null)
        //                {
        //                    baseType = baseType.BaseType;
        //                    fieldInfo = baseType.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic);
        //                }
        //                (viewModelValue = viewModelToServiceConnections.GetValueOrNew(baseType)).PropertyChangedEventHandler = (PropertyChangedEventHandler)fieldInfo.GetValue(instance);
        //            }
        //            viewModelValue.PropertyChangedEventHandler?.Invoke(instance, args);
        //        }
        //    }
        //}
    }
}

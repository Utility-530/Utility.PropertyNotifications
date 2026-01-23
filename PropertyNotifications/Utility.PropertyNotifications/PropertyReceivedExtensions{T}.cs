using System.Linq.Expressions;
using System.Reflection;
using Utility.Helpers.Reflection;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyNotifications
{
    public static partial class PropertyReceivedExtensions
    {

        private class PropertyObservable<TTarget, T> : PropertyObservable, IObservable<T>, IGetReference where TTarget : INotifyPropertyReceived
        {
            private readonly TTarget _target;
            private readonly PropertyInfo _info;
            private readonly bool includeInitial;
            private readonly bool includeNulls;
            private readonly bool includeDefaultValues;

            public PropertyObservable(TTarget target, PropertyInfo info, bool includeInitial = true, bool includeNulls = true, bool includeDefaultValues = true) : base(target, includeNulls, includeDefaultValues)
            {
                _target = target;
                _info = info;
                this.includeInitial = includeInitial;
                this.includeNulls = includeNulls;
                this.includeDefaultValues = includeDefaultValues;
            }

            public PropertyInfo PropertyInfo => _info;

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription<T>(_target, _info, observer, includeInitial, includeNulls, includeDefaultValues);
            }
        }

        public static IObservable<PropertyReception> WhenReceivedFrom<TModel>(this TModel model, bool includeNulls = true) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model, includeNulls);
        }

        public static IObservable<TRes> WhenReceivedFrom<TModel, TRes>(this TModel model, Expression<Func<TModel, TRes>> expr, bool includeInitialValue = true, bool includeNulls = true, bool includeDefaultValues) where TModel : INotifyPropertyReceived
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TModel, TRes>(model, prop, includeInitialValue, includeNulls, includeDefaultValues);
        }
    }

    public class Subscription<T> : IDisposable
    {
        private readonly INotifyPropertyReceived _target;
        private readonly PropertyInfo _info;
        private readonly Func<object, T> _getter;
        private readonly Action<T, object> _setter;
        private readonly IObserver<T> _observer;
        private readonly bool includeNulls;
        private readonly bool includeDefaultValues;
        private T value;

        public Subscription(INotifyPropertyReceived target, PropertyInfo info, IObserver<T> observer, bool includeInitial = true, bool includeNulls = true, bool includeDefaultValues = true)
        {
            _target = target;
            _info = info;
            _getter = info.ToGetter<T>();
            //_setter = info.ToSetter<T>();
            _observer = observer;
            this.includeNulls = includeNulls;
            this.includeDefaultValues = includeDefaultValues;
            _target.PropertyReceived += onPropertyReceived;

            if (includeInitial)
            {
                var value = _getter.Invoke(_target);
                if (Helpers.Include(value, includeNulls, includeDefaultValues))
                {
                    _observer.OnNext(value);
                    return;
                }
            }
        }

        private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
        {
            if (e.PropertyName == _info.Name)
            {
                var value = _getter.Invoke(e.Source ?? _target);
                if (Helpers.Include(value, includeNulls, includeDefaultValues))
                {
                    _observer.OnNext(value);
                    return;
                }
            }
        }

        public T Value { get => _getter.Invoke(_target); /*set => _setter.Invoke(value, _target);*/ }

        public void Dispose()
        {
            _target.PropertyReceived -= onPropertyReceived;
            _observer.OnCompleted();
        }
    }

}
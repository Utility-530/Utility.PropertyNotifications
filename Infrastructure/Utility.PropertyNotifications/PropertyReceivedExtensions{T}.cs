using System.Linq.Expressions;
using System.Reflection;
using Utility.Interfaces;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyNotifications
{
    public static partial class PropertyReceivedExtensions
    {
        private class PropertyObservable<TTarget, T> : IObservable<T>, IGetReference where TTarget: INotifyPropertyReceived
        {
            private readonly TTarget _target;
            private readonly PropertyInfo _info;
            private readonly bool includeInitial;
            private readonly bool includeNulls;

            public PropertyObservable(TTarget target, PropertyInfo info, bool includeInitial = true, bool includeNulls = true)
            {
                _target = target;
                _info = info;
                this.includeInitial = includeInitial;
                this.includeNulls = includeNulls;
            }

            public object Reference => _target;

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyReceived _target;
                private readonly PropertyInfo _info;
                private readonly Func<object, T> _getter;
                private readonly IObserver<T> _observer;
                private readonly bool includeNulls;

                public Subscription(INotifyPropertyReceived target, PropertyInfo info, IObserver<T> observer, bool includeInitial = true, bool includeNulls = true)
                {
                    _target = target;
                    _info = info;
                    _getter = info.ToGetter<T>();
                    _observer = observer;
                    this.includeNulls = includeNulls;
                    _target.PropertyReceived += onPropertyReceived;

                    if (includeInitial)
                    {
                        var value = _getter.Invoke(_target);
                        if (includeNulls || value != null)
                            _observer.OnNext(value);

                    }
                }

                private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
                {
                    if (e.PropertyName == _info.Name)
                    {
                        var value = _getter.Invoke(e.Source ?? _target);
                        if (includeNulls || value != null)
                        {
                            _observer.OnNext(value);
                        }
                    }
                }

                public void Dispose()
                {
                    _target.PropertyReceived -= onPropertyReceived;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription(_target, _info, observer, includeInitial, includeNulls);
            }
        }

        public static IObservable<PropertyReception> WhenReceivedFrom<TModel>(this TModel model, bool includeNulls = true) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model, includeNulls);
        }

        public static IObservable<TRes> WhenReceivedFrom<TModel, TRes>(this TModel model, Expression<Func<TModel, TRes>> expr, bool includeInitialValue = true, bool includeNulls = true) where TModel : INotifyPropertyReceived
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TModel, TRes>(model, prop, includeInitialValue, includeNulls);
        }
    }
}
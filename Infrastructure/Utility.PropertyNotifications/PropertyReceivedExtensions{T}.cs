using System.Linq.Expressions;
using System.Reflection;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{

    public static partial class PropertyReceivedExtensions
    {
        private class PropertyObservable<T> : IObservable<T>
        {
            private readonly INotifyPropertyReceived _target;
            private readonly PropertyInfo _info;
            private readonly bool includeInitial;

            public PropertyObservable(INotifyPropertyReceived target, PropertyInfo info)
            {
                _target = target;
                _info = info;
                this.includeInitial = includeInitial;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyReceived _target;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;

                public Subscription(INotifyPropertyReceived target, PropertyInfo info, IObserver<T> observer, bool includeInitial = true)
                {
                    _target = target;
                    _info = info;
                    _observer = observer;
                    _target.PropertyReceived += onPropertyReceived;

                    if (includeInitial)
                    {
                        var value = (T)_info.GetValue(_target);
                        _observer.OnNext(value);
                    }

                }

                private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
                {
                    if (e.PropertyName == _info.Name)
                    {
                        var value = (T)_info.GetValue(e.Source ?? _target);
                        _observer.OnNext(value);
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
                return new Subscription(_target, _info, observer);
            }
        }

        public static IObservable<PropertyReception> WhenReceivedFrom<TModel>(this TModel model) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model);
        }


        public static IObservable<TRes> WhenReceivedFrom<TModel, TRes>(this TModel model, Expression<Func<TModel, TRes>> expr) where TModel : INotifyPropertyReceived
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TRes>(model, prop);
        }
    }
}
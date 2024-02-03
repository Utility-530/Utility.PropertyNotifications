using Utility.PropertyNotifications;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Utility.PropertyNotifications
{

    public static partial class PropertyCalledExtensions
    {
        private class PropertyObservable<T> : IObservable<T>
        {
            private readonly INotifyPropertyCalled _target;
            private readonly PropertyInfo _info;

            public PropertyObservable(INotifyPropertyCalled target, PropertyInfo info, bool includeNonInitial)
            {
                _target = target;
                _info = info;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyCalled _target;
                private readonly PropertyInfo _info;
                private readonly IObserver<T> _observer;
      

                public Subscription(INotifyPropertyCalled target, PropertyInfo info, IObserver<T> observer)
                {
                    _target = target;
                    _info = info;
                    _observer = observer;
                    _target.PropertyCalled += onPropertyCalled;
                }

                private void onPropertyCalled(object sender, PropertyCalledEventArgs e)
                {
                    if (e.PropertyName == _info.Name)
                    {
              
                            var value = (T)_info.GetValue(_target);
                            _observer.OnNext(value);
                        
                    }
                }

                public void Dispose()
                {
                    _target.PropertyCalled -= onPropertyCalled;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return new Subscription(_target, _info, observer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="model"></param>
        /// <param name="expr"></param>
        /// <param name="includeNonInitial">include changes to value after the initial change</param>
        /// <returns></returns>
        public static IObservable<TRes> WhenCalledFrom<TModel, TRes>(this TModel model,
            Expression<Func<TModel, TRes>> expr, bool includeNonInitial = false) where TModel : INotifyPropertyCalled
        {
            var l = (LambdaExpression)expr;
            var ma = (MemberExpression)l.Body;
            var prop = (PropertyInfo)ma.Member;
            return new PropertyObservable<TRes>(model, prop, includeNonInitial);
        }

    }
}
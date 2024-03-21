using System;
using System.Linq.Expressions;
using System.Reflection;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public record PropertyCall(object Target, object Value, string Name);

    public static partial class PropertyCalledExtensions
    {
        private class PropertyObservable : IObservable<PropertyCall>
        {
            private readonly INotifyPropertyCalled _target;

            public PropertyObservable(INotifyPropertyCalled target)
            {
                _target = target;    
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyCalled _target;
                private readonly IObserver<PropertyCall> _observer;

                public Subscription(INotifyPropertyCalled target, IObserver<PropertyCall> observer)
                {
                    _target = target;
                    _observer = observer;
                    _target.PropertyCalled += onPropertyCalled;
                }

                private void onPropertyCalled(object sender, PropertyCalledEventArgs e)
                {
                    _observer.OnNext(new(_target, e.Value, e.PropertyName));
                }

                public void Dispose()
                {
                    _target.PropertyCalled -= onPropertyCalled;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyCall> observer)
            {
                return new Subscription(_target, observer);
            }
        }

        public static IObservable<PropertyCall> WhenCalled<TModel>(this TModel model,
           bool includeNonInitial = true) where TModel : INotifyPropertyCalled
        {
            return new PropertyObservable(model);
        }
    }
}
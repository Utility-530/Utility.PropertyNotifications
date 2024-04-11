using System;
using System.Reflection;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public record PropertyReception(object Target, object Value, string Name) ;

    public static partial class PropertyReceivedExtensions
    {
        private class PropertyObservable : IObservable<PropertyReception>
        {
            private readonly INotifyPropertyReceived _target;

            public PropertyObservable(INotifyPropertyReceived target)
            {
                _target = target;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyReceived _target;
                private readonly IObserver<PropertyReception> _observer;

                public Subscription(INotifyPropertyReceived target, IObserver<PropertyReception> observer)
                {
                    _target = target;
                    _observer = observer;
                    _target.PropertyReceived += onPropertyReceived;
                }

                private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
                {
                    _observer.OnNext(new(_target, e.Value, e.PropertyName));
                }

                public void Dispose()
                {
                    _target.PropertyReceived -= onPropertyReceived;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyReception> observer)
            {
                return new Subscription(_target, observer);
            }
        }

        public static IObservable<PropertyReception> WhenReceived<TModel>(this TModel model) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model);
        }
    }
}
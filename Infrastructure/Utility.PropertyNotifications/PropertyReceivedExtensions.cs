using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public record PropertyReception(object Target, object Value, object OldValue, string Name);

    public static partial class PropertyReceivedExtensions
    {
        private class PropertyObservable : IObservable<PropertyReception>
        {
            private readonly INotifyPropertyReceived _target;
            private readonly bool includeNulls;

            public PropertyObservable(INotifyPropertyReceived target, bool includeNulls = true)
            {
                _target = target;
                this.includeNulls = includeNulls;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyReceived _target;
                private readonly IObserver<PropertyReception> _observer;
                private readonly bool includeNulls;

                public Subscription(INotifyPropertyReceived target, IObserver<PropertyReception> observer, bool includeNulls)
                {
                    _target = target;
                    _observer = observer;
                    this.includeNulls = includeNulls;
                    _target.PropertyReceived += onPropertyReceived;
                }

                private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
                {
                    if (includeNulls || e.Value != null)
                        _observer.OnNext(new(e.Source ?? _target, e.Value, e.OldValue, e.PropertyName));
                }

                public void Dispose()
                {
                    _target.PropertyReceived -= onPropertyReceived;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyReception> observer)
            {
                return new Subscription(_target, observer, includeNulls);
            }
        }

        public static IObservable<PropertyReception> WhenReceived<TModel>(this TModel model) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model);
        }
    }
}
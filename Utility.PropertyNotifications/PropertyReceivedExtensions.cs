using Utility.Helpers.Reflection;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyNotifications
{
    public record PropertyReception(object Target, object Value, object OldValue, string Name);

    public static partial class PropertyReceivedExtensions
    {

        public static IObservable<PropertyReception> WhenReceived<TModel>(this TModel model) where TModel : INotifyPropertyReceived
        {
            return new PropertyObservable(model);
        }
    }

    public class PropertyObservable : IObservable<PropertyReception>, IGetReference
    {
        private readonly INotifyPropertyReceived _target;
        private readonly bool includeDefaultValues;

        public PropertyObservable(INotifyPropertyReceived target, bool includeDefaultValues = true)
        {
            _target = target;
            this.includeDefaultValues = includeDefaultValues;
        }

        public object Reference => _target;

        public IDisposable Subscribe(IObserver<PropertyReception> observer)
        {
            return new Subscription(_target, observer, includeDefaultValues);
        }
    }


    public class Subscription : IDisposable
    {
        private readonly INotifyPropertyReceived _target;
        private readonly IObserver<PropertyReception> _observer;
        private readonly bool includeDefaultValues;

        public Subscription(INotifyPropertyReceived target, IObserver<PropertyReception> observer, bool includeDefaultValues)
        {
            _target = target;
            _observer = observer;
            this.includeDefaultValues = includeDefaultValues;
            _target.PropertyReceived += onPropertyReceived;
        }

        private void onPropertyReceived(object sender, PropertyReceivedEventArgs e)
        {
            if (Helpers.Proceed(e.Value, includeDefaultValues))
                _observer.OnNext(new(e.Source ?? _target, e.Value, e.OldValue, e.PropertyName));
        }

        public void Dispose()
        {
            _target.PropertyReceived -= onPropertyReceived;
            _observer.OnCompleted();
        }
    }
}
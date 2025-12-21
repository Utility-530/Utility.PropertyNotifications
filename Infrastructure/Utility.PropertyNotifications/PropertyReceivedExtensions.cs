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
        private readonly bool includeNulls;

        public PropertyObservable(INotifyPropertyReceived target, bool includeNulls = true)
        {
            _target = target;
            this.includeNulls = includeNulls;
        }

        public object Reference => _target;

        public IDisposable Subscribe(IObserver<PropertyReception> observer)
        {
            return new Subscription(_target, observer, includeNulls);
        }
    }


    public class Subscription : IDisposable
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
            if (includeNulls || !Comparison.IsDefaultValue(e.Value))
                _observer.OnNext(new(e.Source ?? _target, e.Value, e.OldValue, e.PropertyName));
        }

        public void Dispose()
        {
            _target.PropertyReceived -= onPropertyReceived;
            _observer.OnCompleted();
        }
    }
}
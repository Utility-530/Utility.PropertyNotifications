using System.Reactive.Disposables;
using Utility.Interfaces.Exs;
using Utility.Trees.Abstractions;

namespace Utility.Models
{
    public class BasicModelResolver : IModelResolver
    {
        private readonly Dictionary<string, Lazy<IReadOnlyTree>> _factories = new Dictionary<string, Lazy<IReadOnlyTree>>();

        public void Register(string key, Func<IReadOnlyTree> factory)
        {
            _factories[key] = new(factory);
        }

        public IDisposable Subscribe(IObserver<IReadOnlyTree> observer)
        {
            foreach(var factory in _factories.Values)
            {
                observer.OnNext(factory.Value);
            }
            return Disposable.Empty;
        }

        public IReadOnlyTree this[string key] => _factories[key].Value;
    }
}

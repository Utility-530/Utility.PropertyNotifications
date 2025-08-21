using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class ModelResolver : IModelResolver
    {
        private readonly Dictionary<string, Lazy<IModel>> _factories = new Dictionary<string, Lazy<IModel>>();

        public void Register(string key, Func<IModel> factory)
        {
            _factories[key] = new(factory);
        }

        public IDisposable Subscribe(IObserver<IModel> observer)
        {
            foreach(var factory in _factories.Values)
            {
                observer.OnNext(factory.Value);
            }
            return Disposable.Empty;
        }

        public IModel this[string key] => _factories[key].Value;
    }
}

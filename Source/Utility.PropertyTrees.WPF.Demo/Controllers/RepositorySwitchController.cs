using Utility.Infrastructure;
using Utility.Models;
using static Utility.PropertyTrees.Events;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using Utility.Repos;
using System;

namespace Utility.PropertyTrees.Services
{
    internal class RepositorySwitchController : BaseObject
    {
        public override Key Key => new Key<RepositorySwitchController>(Guids.RepositorySwitch);

        public IObservable<RepositorySwitchResponse> OnNext(RepositorySwitchRequest request)
        {
            return Create<RepositorySwitchResponse>(observer =>
            {
                if (request.ItemKey is Key { Type: Type type })
                {
                    if (typeof(Event).IsAssignableFrom(type))
                    {
                        observer.OnNext(new RepositorySwitchResponse(new Key<InMemoryRepository>(Utility.Repos.Guids.InMemory)));
                        return Disposer.Empty;
                    }
                }

                observer.OnNext(new RepositorySwitchResponse(new Key<SqliteRepository>(Utility.Repos.Guids.SQLite)));
                return Disposer.Empty;
            });
        }
    }
}

using Utility.Infrastructure;
using Utility.Models;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using Utility.Repos;
using System;

namespace Utility.PropertyTrees.Services
{
    internal class RepositorySwitchController : BaseObject
    {
        public override Key Key => new Key<RepositorySwitchController>(Guids.RepositorySwitch);

        public Interfaces.Generic.IObservable<RepositorySwitchResponse> OnNext(RepositorySwitchRequest request)
        {
            return Create<RepositorySwitchResponse>(observer =>
            {

                observer.OnNext(new RepositorySwitchResponse(new Key<InMemoryRepository>(Utility.Guids.InMemory)));
                return Disposer.Empty;
            });
        }
    }
}

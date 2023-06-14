using Akavache;
using ReactiveUI;
using System;
using System.Reactive;

namespace Utility.WPF.SuspensionDriver
{
    public class AkavacheSuspensionDriver<TAppState> : ISuspensionDriver where TAppState : class
    {
        private const string AppStateKey = "appState";

        public AkavacheSuspensionDriver() => BlobCache.ApplicationName = "WearScar";

        public IObservable<Unit> InvalidateState() => BlobCache.UserAccount.InvalidateObject<TAppState>(AppStateKey);

        public IObservable<object> LoadState() => BlobCache.UserAccount.GetObject<TAppState>(AppStateKey);

        public IObservable<Unit> SaveState(object state) => BlobCache.UserAccount.InsertObject(AppStateKey, (TAppState)state);
    }
}
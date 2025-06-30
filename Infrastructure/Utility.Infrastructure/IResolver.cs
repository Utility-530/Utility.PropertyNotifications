using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure
{
    public interface IResolver
    {
        //void Clear();
        void Initialise();
        void Send(GuidValue @base);
        Interfaces.Reactive.IObservable<TOutput> Register<TInput, TOutput>(Key key, TInput tInput) where TInput : IGetGuid;
    }
}
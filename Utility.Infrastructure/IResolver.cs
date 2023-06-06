using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure
{
    public interface IResolver
    {
        //void Clear();
        void Initialise();
        void OnBase(IBase @base);
        Interfaces.Generic.IObservable<TOutput> Register<TInput, TOutput>(IBase baseObject, TInput tInput) where TInput : IGuid;
    }
}
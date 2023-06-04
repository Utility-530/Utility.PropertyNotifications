using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure
{
    public interface IResolver
    {
        void Clear();
        void Initialise();
        void OnBase(IBase @base);
        Interfaces.Generic.IObservable<TOutput> Register<TOutput, TInput>(IBase baseObject, TInput tInput) where TInput : IGuid;
    }
}
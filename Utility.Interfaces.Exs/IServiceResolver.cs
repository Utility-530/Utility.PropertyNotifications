using System;

namespace Utility.Interfaces.Exs
{
    public interface IServiceResolver
    {
        void Connect<TParam, TParam2>()
            where TParam : IMethodParameter
            where TParam2 : IMethodParameter;
        void Observe<TParam>(IObservable<object> observable) where TParam : IMethodParameter;
        void ReactTo<TParam>(IObserver<object> observer) where TParam : IMethodParameter;
    }
}
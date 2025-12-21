using System;
using Utility.Interfaces.Methods;

namespace Utility.Interfaces.Exs
{
    public interface IServiceResolver
    {
        void Connect<TParam, TParam2>()
            where TParam : IParameter
            where TParam2 : IParameter;

        void Observe<TParam>(IObservable<object> observable) where TParam : IParameter;

        void ReactTo<TParam, TOutput>(IObserver<object> observer) where TParam : IParameter;
    }
}
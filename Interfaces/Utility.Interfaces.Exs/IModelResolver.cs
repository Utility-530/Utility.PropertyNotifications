using System;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces.Exs
{
    public interface IModelResolver : IObservable<IReadOnlyTree>
    {
        void Register(string key, Func<IReadOnlyTree> factory);

        IReadOnlyTree this[string key] { get; }
    }
}